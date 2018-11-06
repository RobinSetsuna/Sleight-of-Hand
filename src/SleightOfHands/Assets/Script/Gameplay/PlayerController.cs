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
    CardChoosing,
    PositionChoosing
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
                        if (previousPlayerState != PlayerState.Move)
                            Path = null;
                        break;

                    case PlayerState.MovementPlanning:
                        Path = new Path<Tile>(GridManager.Instance.GetTile(Player.transform.position));
                        break;

                    case PlayerState.MovementConfirmation:
                        Vector3 tileCenter = GridManager.Instance.GetWorldPosition(path.Destination);
                        tileCenter.y += GridManager.Instance.TileSize;
                        UIManager.Singleton.Open("ListMenu", UIManager.UIMode.DEFAULT, UIManager.Singleton.GetCanvasPosition(Camera.main.WorldToScreenPoint(tileCenter)), "MOVE", (UnityAction)InitiateMovement, "CANCEL", (UnityAction)ResetMovement);
                        break;

                    case PlayerState.Move:
                        for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
                            ActionManager.Singleton.Add(new Movement(GetComponent<player>(), tile));
                        Path = null;
                        ActionManager.Singleton.Execute(ResetToIdle);
                        break;
                }

                onCurrentPlayerStateChange.Invoke(previousPlayerState, currentPlayerState);
            }
        }
    }

    private PlayerController() {}

    private void Awake()
    {
        Player = GetComponent<player>();

        CurrentPlayerState = 0;

        LevelManager.Instance.OnCurrentPhaseChangeForPlayer.AddListener(HandleCurrentPhaseChange);
    }

    private void OnDestroy()
    {
        Disable();

        LevelManager.Instance.OnCurrentPhaseChangeForPlayer.RemoveListener(HandleCurrentPhaseChange);
    }

    /// <summary>
    /// Start to recieve user inputs by adding listeners to events in MouseInputManager
    /// </summary>
    private void Enable()
    {
        if (!isEnabled)
        {
            MouseInputManager.Singleton.onMouseClick.AddListener(HandleMouseClick);
            MouseInputManager.Singleton.onDragEnd.AddListener(HandleMouseDragEnd);
            MouseInputManager.Singleton.onMouseEnter.AddListener(HandleMouseTargetChange);

            isEnabled = true;
        }
    }

    /// <summary>
    /// End recieving user inputs by removing added listeners to events in MouseInputManager
    /// </summary>
    private void Disable()
    {
        if (isEnabled)
        {
            MouseInputManager.Singleton.onMouseClick.RemoveListener(HandleMouseClick);
            MouseInputManager.Singleton.onDragEnd.RemoveListener(HandleMouseDragEnd);
            MouseInputManager.Singleton.onMouseEnter.RemoveListener(HandleMouseTargetChange);

            isEnabled = false;
        }
    }

    /// <summary>
    /// Add a way point to Path
    /// </summary>
    /// <param name="tile"></param>
    private void AddWayPoint(Tile tile)
    {
        path.AddLast(tile);

        onPathUpdate.Invoke(path);
    }

    /// <summary>
    /// Remove the last way point added to Path
    /// </summary>
    private void RemoveWayPoint()
    {
        path.RemoveLast();

        onPathUpdate.Invoke(path);
    }

    /// <summary>
    /// Make a transition to PlayerState.Move
    /// </summary>
    private void ResetToIdle()
    {
        CurrentPlayerState = PlayerState.Idle;
    }

    /// <summary>
    /// Make a transition to PlayerState.MovementPlanning
    /// </summary>
    private void ResetMovement()
    {
        CurrentPlayerState = PlayerState.MovementPlanning;
    }

    /// <summary>
    /// Make a transition to PlayerState.Move
    /// </summary>
    private void InitiateMovement()
    {
        CurrentPlayerState = PlayerState.Move;
    }

    /// <summary>
    /// An event listener for MouseInputManager.Singleton.onMouseClick
    /// </summary>
    /// <param name="obj"> The clicked object </param>
    private void HandleMouseClick(MouseInteractable obj)
    {
        switch (currentPlayerState)
        {
            case PlayerState.Idle:
                if (obj == this || (obj.GetComponent<Tile>() == GridManager.Instance.GetTile(Player.transform.position)))
                    CurrentPlayerState = PlayerState.MovementPlanning;
                else if (obj.GetComponent<Enemy>())
                    obj.GetComponent<Enemy>().hightlightDetection();
                break;

            case PlayerState.MovementPlanning:
                if (obj == this)
                    CurrentPlayerState = PlayerState.Idle;
                else if (obj.GetComponent<Tile>())
                {
                    Tile tile = obj.GetComponent<Tile>();

                    if (tile == path.Start)
                        CurrentPlayerState = PlayerState.Idle;
                    else
                    {
                        Tile playerTile = GridManager.Instance.GetTile(Player.transform.position);

                        if (tile.IsHighlighted(Tile.HighlightColor.Blue))
                        {
                            Path = Navigation.FindPath(GridManager.Instance, playerTile, tile);
                            CurrentPlayerState = PlayerState.MovementConfirmation;
                        }
                    }
                }
                break;

            case PlayerState.CardChoosing:
               
                CurrentPlayerState = PlayerState.PositionChoosing;
                break;
        }
    }

    /// <summary>
    /// An event listener for MouseInputManager.Singleton.onDragEnd
    /// </summary>
    /// <param name="obj"> The object from which the player starts to drag </param>
    private void HandleMouseDragEnd(MouseInteractable obj)
    {
        switch (currentPlayerState)
        {
            case PlayerState.MovementPlanning:
                if (path.Count > 0 && (obj == this || obj.GetComponent<Tile>() == path.Start))
                    CurrentPlayerState = PlayerState.MovementConfirmation;
                break;
        }
    }

    /// <summary>
    /// An event listener for MouseInputManager.Singleton.onMouseEnter
    /// </summary>
    /// <param name="obj"> The object at which the mouse is pointing at </param>
    private void HandleMouseTargetChange(MouseInteractable obj)
    {
        if (MouseInputManager.Singleton.IsMouseDragging)
            switch (currentPlayerState)
            {
                case PlayerState.MovementPlanning:
                    if (obj == this)
                    {
                        if (path.Count == 1)
                            RemoveWayPoint();
                    }
                    else if (obj.GetComponent<Tile>())
                    {
                        Tile tile = obj.GetComponent<Tile>();

                        if (path.Count > 0)
                        {
                            if (tile == path.Last.Previous.Value)
                                RemoveWayPoint();
                            else if (GridManager.Instance.IsAdjacent(tile, path.Last.Value) && path.Count < Player.ActionPoint && !path.Contains(tile))
                                AddWayPoint(tile);
                        }
                        else if (GridManager.Instance.IsAdjacent(tile, GridManager.Instance.GetTile(Player.transform.position)))
                            AddWayPoint(tile);
                    }
                    break;
            }
    }

    /// <summary>
    /// An event listener for LevelManager.Instance.onCurrentPhaseChangeForPlayer
    /// </summary>
    /// <param name="currentPhase"> The phase which the LevelManager just entered </param>
    private void HandleCurrentPhaseChange(Phase currentPhase)
    {
        switch (currentPhase)
        {
            case Phase.Action:
                CurrentPlayerState = PlayerState.Idle;
                break;

            case Phase.End:
                CurrentPlayerState = PlayerState.Uncontrollable;
                break;
        }
    }
}
