using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The state of the player character
/// </summary>
public enum PlayerState : int
{
    Uncontrollable,
    Idle,

    MovementPlanning,
    MovementConfirmation,
    Move,

    CardBrowsing,
    CardUsagePlanning,
    CardUsageConfirmation,
    UseCard,
}

/// <summary>
/// A FSM for the player character recieving user inputs to control the player character
/// </summary>
public class PlayerController : MouseInteractable
{
    /// <summary>
    /// An event triggered whenever the state of the player character changes
    /// </summary>
    public EventOnDataChange<PlayerState> onCurrentPlayerStateChange = new EventOnDataChange<PlayerState>();

    /// <summary>
    /// An event triggered whenever the planned path is changed by the player
    /// </summary>
    public EventOnDataUpdate<Path<Tile>> onPathUpdate = new EventOnDataUpdate<Path<Tile>>();

    public EventOnDataUpdate<Card> onCardToUseUpdate = new EventOnDataUpdate<Card>();
    
    /// <summary>
    /// The player controlled by this controller
    /// </summary>
    public player Player { get; private set; }

    private bool isEnabled = false;

    private Path<Tile> path;
    private Path<Tile> Path
    {
        set
        {
            if (value != path)
            {
                path = value;
                onPathUpdate.Invoke(path);
            }
        }
    }

    private Card cardToUse;
    private Card CardToUse
    {
        set
        {
            if (value != cardToUse)
            {
                cardToUse = value;
                onCardToUseUpdate.Invoke(cardToUse);
            }
        }
    }

    private Tile targetTile;

    private PlayerState currentPlayerState;

    /// <summary>
    /// The current state of the player character
    /// </summary>
    public PlayerState CurrentPlayerState
    {
        get
        {
            return currentPlayerState;
        }

        private set
        {
#if UNITY_EDITOR
            LogUtility.PrintLogFormat("PlayerController", "Made a transition to {0}.", value);
#endif

            // Reset current state
            if (value == currentPlayerState)
            {
                switch (currentPlayerState)
                {
                    case PlayerState.MovementPlanning:
                        path.Clear();
                        onPathUpdate.Invoke(path);
                        break;
                }
            }
            else
            {
                // Before leaving the previous state
                switch (currentPlayerState)
                {
                    case PlayerState.Uncontrollable:
                        Enable();
                        break;
                }

                PlayerState previousPlayerState = CurrentPlayerState;
                currentPlayerState = value;

                // After entering the new state
                switch (currentPlayerState)
                {
                    case PlayerState.Uncontrollable:
                        Path = null;
                        Disable();
                        break;

                    case PlayerState.Idle:
                        if (previousPlayerState == PlayerState.MovementPlanning)
                            Path = null;
                        break;

                    case PlayerState.MovementPlanning:
                        Path = new Path<Tile>(GridManager.Instance.GetTile(Player.transform.position));
                        break;

                    case PlayerState.MovementConfirmation:
                        Vector3 tileCenter = GridManager.Instance.GetWorldPosition(path.Destination);
                        tileCenter.y += GridManager.Instance.TileSize;
                        UIManager.Singleton.Open("ListMenu", UIManager.UIMode.DEFAULT, UIManager.Singleton.GetCanvasPosition(Camera.main.WorldToScreenPoint(tileCenter)), "MOVE", (UnityAction)InitiateMovement