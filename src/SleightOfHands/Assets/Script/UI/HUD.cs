using UnityEngine;
using UnityEngine.UI;

public class HUD : UserInterface
{
    [SerializeField] private Text turn;
    [SerializeField] private Button endTurnButton;

    private void Start()
    {
        if (LevelManager.Instance.CurrentRound == Round.Player)
            HandleCurrentPhaseChangeForPlayer(LevelManager.Instance.CurrentPhase);
        else
            endTurnButton.interactable = false;

        endTurnButton.onClick.AddListener(EndCurrentTurn);

        UpdateTurnText(LevelManager.Instance.CurrentTurn);

        LevelManager.Instance.playerController.onCurrentPlayerStateChange.AddListener(HandleCurrentPlayerStateChange);
        LevelManager.Instance.OnCurrentPhaseChangeForPlayer.AddListener(HandleCurrentPhaseChangeForPlayer);
        LevelManager.Instance.onCurrentTurnChange.AddListener(UpdateTurnText);
    }

    private void EndCurrentTurn()
    {
        LevelManager.Instance.EndPlayerActionPhase();
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnCurrentPhaseChangeForPlayer.RemoveListener(HandleCurrentPhaseChangeForPlayer);
        LevelManager.Instance.onCurrentTurnChange.RemoveListener(UpdateTurnText);
    }

    private void UpdateTurnText(int n)
    {
        turn.text = n.ToString();
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
