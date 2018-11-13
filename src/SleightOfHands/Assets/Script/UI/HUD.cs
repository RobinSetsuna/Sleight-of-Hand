using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : UIWindow
{
    [SerializeField] private Text turn;
    [SerializeField] private Text banner;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private UIList cardList;
    [SerializeField] private Text actionPoint;

    private Dictionary<Card, GameObject> hand = new Dictionary<Card, GameObject>();

    public override void OnOpen(params object[] args)
    {
        if (LevelManager.Instance.CurrentRound == Round.Player)
            HandleCurrentPhaseChangeForPlayer(LevelManager.Instance.CurrentPhase);
        else
            endTurnButton.interactable = false;

        endTurnButton.onClick.AddListener(EndCurrentTurn);

        UpdateTurnText(LevelManager.Instance.CurrentTurn);
        UpdateActionPointText(LevelManager.Instance.Player.Ap);

        foreach (Card card in CardManager.Instance.hand)
            HandleHandChange(ChangeType.Incremental, card);

        CardManager.Instance.onHandChange.AddListener(HandleHandChange);

        LevelManager.Instance.Player.onAttributeChange.AddListener(HandlePlayerStatisticChange);
        LevelManager.Instance.playerController.onCurrentPlayerStateChange.AddListener(HandleCurrentPlayerStateChange);
        LevelManager.Instance.OnCurrentPhaseChangeForPlayer.AddListener(HandleCurrentPhaseChangeForPlayer);
        LevelManager.Instance.onCurrentTurnChange.AddListener(UpdateTurnText);
    }

    public override void OnClose()
    {
        CardManager.Instance.onHandChange.RemoveListener(HandleHandChange);

        LevelManager.Instance.Player.onAttributeChange.RemoveListener(HandlePlayerStatisticChange);
        LevelManager.Instance.playerController.onCurrentPlayerStateChange.RemoveListener(HandleCurrentPlayerStateChange);
        LevelManager.Instance.OnCurrentPhaseChangeForPlayer.RemoveListener(HandleCurrentPhaseChangeForPlayer);
        LevelManager.Instance.onCurrentTurnChange.RemoveListener(UpdateTurnText);
    }

    public void ToggleMenu()
    {
        UIManager.Singleton.Toggle("IngameMenu");
    }

    private void EndCurrentTurn()
    {
        LevelManager.Instance.EndPlayerActionPhase();
    }

    private void UpdateTurnText(int currentTurn)
    {
        turn.text = currentTurn.ToString();

        ShowBanner("Turn " + currentTurn);
    }

    private void UpdateActionPointText(int ap)
    {
        actionPoint.text = ap.ToString();
    }

    private void ShowBanner(string content)
    {
        banner.text = content;
        banner.gameObject.SetActive(true);
    }

    private void HandleHandChange(ChangeType change, Card card)
    {
        switch (change)
        {
            case ChangeType.Incremental:
                Transform listTransform = cardList.transform;
                int index = hand.Count;
                int numExistedListItems = listTransform.childCount;
                GameObject listItem;
                if (index < numExistedListItems)
                    listItem = listTransform.GetChild(index).gameObject;
                else
                    listItem = Instantiate(ResourceUtility.GetPrefab("Card"), listTransform);
                listItem.SetActive(true);
                listItem.GetComponent<UICard>().Refresh(card);
                hand.Add(card, listItem);
                cardList.Refresh();
                break;

            case ChangeType.Decremental:
                hand[card].SetActive(false);
                hand.Remove(card);
                cardList.Refresh();
                break;
        }
    }

    private void HandlePlayerStatisticChange(StatisticType statistic, float originalValue, float currentValue)
    {
        switch (statistic)
        {
            case StatisticType.Ap:
                UpdateActionPointText(Mathf.RoundToInt(currentValue));
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
