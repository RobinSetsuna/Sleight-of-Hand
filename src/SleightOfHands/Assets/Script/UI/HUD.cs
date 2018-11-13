﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : UIWindow
{
    [SerializeField] private Text turn;
    [SerializeField] private Text banner;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private UIList cardList;
    [SerializeField] private Text ap;
    [SerializeField] private Text hp;

    private Dictionary<Card, GameObject> uiCards = new Dictionary<Card, GameObject>();

    public override void OnOpen(params object[] args)
    {
        UpdateAll();

        CardManager.Instance.onHandChange.AddListener(HandleHandChange);

        LevelManager.Instance.Player.onAttributeChange.AddListener(HandlePlayerStatisticChange);
        LevelManager.Instance.playerController.onCurrentPlayerStateChange.AddListener(HandleCurrentPlayerStateChange);
        LevelManager.Instance.OnCurrentPhaseChangeForPlayer.AddListener(HandleCurrentPhaseChangeForPlayer);
        LevelManager.Instance.onCurrentTurnChange.AddListener(HandleTurnChange);
    }

    public override void OnClose()
    {
        CardManager.Instance.onHandChange.RemoveListener(HandleHandChange);

        LevelManager.Instance.Player.onAttributeChange.RemoveListener(HandlePlayerStatisticChange);
        LevelManager.Instance.playerController.onCurrentPlayerStateChange.RemoveListener(HandleCurrentPlayerStateChange);
        LevelManager.Instance.OnCurrentPhaseChangeForPlayer.RemoveListener(HandleCurrentPhaseChangeForPlayer);
        LevelManager.Instance.onCurrentTurnChange.RemoveListener(HandleTurnChange);
    }

    override public void UpdateAll()
    {
        if (LevelManager.Instance.CurrentRound == Round.Player)
            HandleCurrentPhaseChangeForPlayer(LevelManager.Instance.CurrentPhase);
        else
            endTurnButton.interactable = false;

        player Player = LevelManager.Instance.Player;

        UpdateTurn(LevelManager.Instance.CurrentTurn);
        UpdateAp(Player.Ap);
        UpdateHp(Player.Hp);
        UpdateHand(CardManager.Instance.hand);
    }

    private void UpdateTurn(int turn)
    {
        this.turn.text = turn.ToString();
    }

    private void UpdateAp(int ap)
    {
        this.ap.text = ap.ToString();
    }

    private void UpdateHp(int hp)
    {
        this.hp.text = hp.ToString();
    }

    private void UpdateHand(List<Card> hand)
    {
        Transform listTransform = cardList.transform;

        uiCards.Clear();

        int numCards = hand.Count;
        int numExistedListItems = listTransform.childCount;
        for (int i = 0; i < Math.Max(numExistedListItems, numCards); i++)
        {
            if (i < numCards)
            {
                Card card = hand[i];

                GameObject uiCard;

                if (i < numExistedListItems)
                    uiCard = listTransform.GetChild(i).gameObject;
                else
                    uiCard = Instantiate(ResourceUtility.GetPrefab("Card"), listTransform);

                uiCards.Add(card, uiCard);

                UpdateCard(uiCard, card);
            }
            else
                listTransform.GetChild(i).gameObject.SetActive(false);
        }

        cardList.Refresh();
    }

    private void UpdateCard(GameObject uiCard, Card card)
    {
        uiCard.SetActive(true);
        uiCard.GetComponent<UICard>().Refresh(card);
    }

    private void ShowBanner(string content)
    {
        banner.text = content;
        banner.gameObject.SetActive(true);
    }

    public void ToggleMenu()
    {
        UIManager.Singleton.Toggle("IngameMenu");
    }

    public void EndCurrentTurn()
    {
        LevelManager.Instance.EndPlayerActionPhase();
    }

    private void HandleTurnChange(int currentTurn)
    {
        UpdateTurn(currentTurn);
        ShowBanner("Turn " + currentTurn);
    }

    private void HandleHandChange(ChangeType change, Card card)
    {
        switch (change)
        {
            case ChangeType.Incremental:
                Transform listTransform = cardList.transform;
                int i = uiCards.Count;
                GameObject uiCard;
                if (i < listTransform.childCount)
                    uiCard = listTransform.GetChild(i).gameObject;
                else
                    uiCard = Instantiate(ResourceUtility.GetPrefab("Card"), listTransform);
                uiCards.Add(card, uiCard);
                UpdateCard(uiCard, card);
                cardList.Refresh();
                break;

            case ChangeType.Decremental:
                uiCards[card].SetActive(false);
                uiCards.Remove(card);
                cardList.Refresh();
                break;
        }
    }

    private void HandlePlayerStatisticChange(StatisticType statistic, float originalValue, float currentValue)
    {
        switch (statistic)
        {
            case StatisticType.Hp:
                UpdateHp(Math.Max(0, Mathf.RoundToInt(currentValue)));
                break;

            case StatisticType.Ap:
                UpdateAp(Mathf.RoundToInt(currentValue));
                break;
        }
    }

    private void HandleCurrentPlayerStateChange(PlayerState previousState, PlayerState currentState)
    {
        switch (previousState)
        {
            case PlayerState.Move:
                endTurnButton.interactable = true;
                break;
        }

        switch (currentState)
        {
            case PlayerState.Move:
                endTurnButton.interactable = false;
                break;
        }
    }

    private void HandleCurrentPhaseChangeForPlayer(Phase currentPhase)
    {
        switch (currentPhase)
        {
            case Phase.Action:
                endTurnButton.interactable = true;
                break;
            case Phase.End:
                endTurnButton.interactable = false;
                break;
        }
    }
}
