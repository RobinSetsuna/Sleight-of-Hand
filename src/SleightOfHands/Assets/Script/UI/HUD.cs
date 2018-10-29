using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Text turn;
    [SerializeField] private Button endTurnButton;

    private void Start()
    {
        if (LevelManager.Instance.CurrentRound == Round.Player)
            HandleCurrentPhaseChangeForPlayer(LevelManager.Instance.CurrentPhase);
        else
            endTurnButton.enabled = false;

        endTurnButton.onClick.AddListener(EndCurrentTurn);

        UpdateTurnText(LevelManager.Instance.CurrentTurn);

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

    private void HandleCurrentPhaseChangeForPlayer(Phase currentPhase)
    {
        switch (currentPhase)
        {
            case Phase.Action:
                endTurnButton.enabled = true;
                break;
            case Phase.End:
                endTurnButton.enabled = false;
                break;
        }
    }
}
