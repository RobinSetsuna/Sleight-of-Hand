using System;
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
    [SerializeField] private Button cancelButton;

    private Dictionary<Card, UICard> uiCards = new Dictionary<Card, UICard>();
    private UICard selectedUICard;
    private UITransform cardListTransformEffect;
    private Vector3 cardListOriginalLocalScale;
    private Vector3 cardListOriginalLocalPosition;

    public override void OnOpen(params object[] args)
    {
        UpdateAll();
        AddEventListeners();
    }

    public override void OnClose()
    {
        RemoveEventListeners();
    }

    public override void Redraw()
    {
        RemoveEventListeners();
        AddEventListeners();

        UpdateAll();
    }

    public override void UpdateAll()
    {
        UpdateButton(LevelManager.Instance.playerController.CurrentPlayerState);

        UpdateTurn(LevelManager.Instance.CurrentTurn);

        player Player = LevelManager.Instance.Player;
        UpdateAp(Player.Ap);
        UpdateHp(Player.Hp);

        UpdateHand(CardManager.Instance.hand);
    }

    private void UpdateButton(PlayerState currentPlayerState)
    {
        switch (currentPlayerState)
        {
            case PlayerState.Uncontrollable:
                endTurnButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                break;

            case PlayerState.Idle:
                endTurnButton.gameObject.SetActive(true);
                cancelButton.gameObject.SetActive(false);
                break;

            case PlayerState.MovementConfirmation:
                endTurnButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                break;

            case PlayerState.Move:
                endTurnButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                break;

            case PlayerState.CardUsageConfirmation:
                endTurnButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                break;

            case PlayerState.UseCard:
                endTurnButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                break;

            default:
                endTurnButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(true);
                break;
        }
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

                UICard uiCard;

                if (i < numExistedListItems)
                    uiCard = listTransform.GetChild(i).GetComponent<UICard>();
                else
                    uiCard = Instantiate(ResourceUtility.GetPrefab<UICard>("Card"), listTransform);

                uiCards.Add(card, uiCard);

                UpdateCard(uiCard, card);
            }
            else
                listTransform.GetChild(i).gameObject.SetActive(false);
        }

        cardList.Refresh();
    }

    private void UpdateCard(UICard uiCard, Card card)
    {
        uiCard.gameObject.SetActive(true);
        uiCard.Refresh(card);
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

    public void EndTurn()
    {
        LevelManager.Instance.EndActionPhase();
    }

    public void Cancel()
    {
        LevelManager.Instance.playerController.Back();
    }

    private void Start()
    {
        Transform cardListTransform = cardList.transform;

        cardListTransformEffect = cardListTransform.GetComponent<UITransform>();

        if (!cardListTransformEffect)
            cardListTransformEffect = cardListTransform.gameObject.AddComponent<UITransform>();

        cardListOriginalLocalScale = cardListTransform.localScale;
        cardListOriginalLocalPosition = cardListTransform.localPosition;
    }

    private void AddEventListeners()
    {
        CardManager.Instance.onHandChange.AddListener(HandleHandChange);

        LevelManager.Instance.Player.onStatisticChange.AddListener(HandlePlayerStatisticChange);

        LevelManager.Instance.playerController.onCurrentPlayerStateChange.AddListener(HandleCurrentPlayerStateChange);
        LevelManager.Instance.playerController.onCardToUseUpdate.AddListener(HandleCardToUseChange);

        LevelManager.Instance.onCurrentPhaseChangeForPlayer.AddListener(HandleCurrentPhaseChangeForPlayer);
        LevelManager.Instance.onCurrentTurnChange.AddListener(HandleTurnChange);
    }

    private void RemoveEventListeners()
    {
        CardManager.Instance.onHandChange.RemoveListener(HandleHandChange);

        LevelManager.Instance.Player.onStatisticChange.RemoveListener(HandlePlayerStatisticChange);

        LevelManager.Instance.playerController.onCurrentPlayerStateChange.RemoveListener(HandleCurrentPlayerStateChange);
        LevelManager.Instance.playerController.onCardToUseUpdate.RemoveListener(HandleCardToUseChange);

        LevelManager.Instance.onCurrentPhaseChangeForPlayer.RemoveListener(HandleCurrentPhaseChangeForPlayer);
        LevelManager.Instance.onCurrentTurnChange.RemoveListener(HandleTurnChange);
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
                UICard uiCard;
                if (i < listTransform.childCount)
                    uiCard = listTransform.GetChild(i).GetComponent<UICard>();
                else
                    uiCard = Instantiate(ResourceUtility.GetPrefab<UICard>("Card"), listTransform);
                uiCards.Add(card, uiCard);
                UpdateCard(uiCard, card);
                cardList.Refresh();
                break;

            case ChangeType.Decremental:
                uiCards[card].gameObject.SetActive(false);
                uiCards.Remove(card);
                cardList.Refresh();
                break;
        }
    }

    private void HandlePlayerStatisticChange(Statistic statistic, float originalValue, float currentValue)
    {
        switch (statistic)
        {
            case Statistic.Hp:
                UpdateHp(Mathf.RoundToInt(currentValue));
                break;

            case Statistic.Ap:
                UpdateAp(Mathf.RoundToInt(currentValue));
                break;
        }
    }

    private void HandleCurrentPlayerStateChange(PlayerState previousState, PlayerState currentState)
    {
        UpdateButton(currentState);

        switch (previousState)
        {
            case PlayerState.CardBrowsing:
                cardListTransformEffect.targetLocalScale = cardListOriginalLocalScale;
                cardListTransformEffect.targetLocalPosition = cardListOriginalLocalPosition;
                cardListTransformEffect.enabled = false;
                cardListTransformEffect.enabled = true;
                break;
        }

        switch (currentState)
        {
            case PlayerState.CardBrowsing:
                cardListTransformEffect.targetLocalScale = new Vector3(3, 3, 1);
                cardListTransformEffect.targetLocalPosition = new Vector3(-cardList.Length * 3 / 2, cardList.Width * 3 / 2, 0);
                cardListTransformEffect.enabled = false;
                cardListTransformEffect.enabled = true;
                break;
        }
    }

    private void HandleCardToUseChange(Card cardToUse)
    {
        if (cardToUse == null)
        {
            selectedUICard.ToggleSelection();
            selectedUICard = null;
        }
        else
        {
            if (selectedUICard)
                selectedUICard.ToggleSelection();

            selectedUICard = uiCards[cardToUse];
            selectedUICard.ToggleSelection();
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
