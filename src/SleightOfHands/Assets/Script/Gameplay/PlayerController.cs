public enum PlayerState : int
{
    Idle,
    MovementPlanning,
    MovementConfirmation,
    Move,
}

public class PlayerController : MouseInteractable
{
    public EventOnDataChange<PlayerState> onCurrentPlayerStateChange = new EventOnDataChange<PlayerState>();
    public EventOnDataUpdate<Path<Tile>> onPathUpdate = new EventOnDataUpdate<Path<Tile>>();

    public player Player { get; private set; }

    private bool isEnabled = false;
    private Path<Tile> path;

    private PlayerState currentPlayerState;
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
                //switch (currentGameState)
                //{
                //}

                PlayerState previousPlayerState = CurrentPlayerState;
                currentPlayerState = value;

                // After entering the new state
                switch (currentPlayerState)
                {
                    case PlayerState.Idle:
                        if (previousPlayerState != PlayerState.Move)
                        {
                            path = null;
                            onPathUpdate.Invoke(path);
                        }
                        break;
                    case PlayerState.MovementPlanning:
                        path = new Path<Tile>(GridManager.Instance.TileFromWorldPoint(Player.transform.position));
                        onPathUpdate.Invoke(path);
                        break;
                    case PlayerState.MovementConfirmation:
                        // TODO: Show ListMenu
                        break;
                    case PlayerState.Move:
                        path = null;
                        onPathUpdate.Invoke(path);
                        ActionManager.Singleton.Execute(ResetToIdle);
                        break;
                }

                onCurrentPlayerStateChange.Invoke(previousPlayerState, currentPlayerState);
            }
        }
    }

    private PlayerController() {}

    private void Start()
    {
        Player = GetComponent<player>();

        ResetToIdle();
    }

    private void OnDestroy()
    {
        Disable();
    }

    internal void Enable()
    {
        if (!isEnabled)
        {
            MouseInputManager.Singleton.onMouseClick.AddListener(HandleMouseClick);
            MouseInputManager.Singleton.onDragEnd.AddListener(HandleEndDragging);
            MouseInputManager.Singleton.onMouseEnter.AddListener(HandleMouseTargetChange);

            isEnabled = true;
        }
    }

    internal void Disable()
    {
        if (isEnabled)
        {
            MouseInputManager.Singleton.onMouseClick.RemoveListener(HandleMouseClick);
            MouseInputManager.Singleton.onDragEnd.RemoveListener(HandleEndDragging);
            MouseInputManager.Singleton.onMouseEnter.RemoveListener(HandleMouseTargetChange);

            isEnabled = false;
        }
    }

    private void AddWayPoint(Tile tile)
    {
        path.AddLast(tile);

        onPathUpdate.Invoke(path);
    }

    private void RemoveWayPoint()
    {
        path.RemoveLast();

        onPathUpdate.Invoke(path);
    }

    private void ResetToIdle()
    {
        CurrentPlayerState = PlayerState.Idle;
    }

    private void InitiatePlayerMovement()
    {
        for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
            ActionManager.Singleton.Add(new Movement(GetComponent<player>(), tile));

        CurrentPlayerState = PlayerState.Move;
    }

    private void HandleMouseClick(MouseInteractable obj)
    {
        switch (currentPlayerState)
        {
            case PlayerState.Idle:
                if (obj == this && Player.ActionPoint > 0)
                    CurrentPlayerState = PlayerState.MovementPlanning;
                else if (obj.GetComponent<Enemy>()) {
                    obj.GetComponent<Enemy>().hightlightDetection();
                }
                break;
            case PlayerState.MovementPlanning:
                if (obj == this)
                    CurrentPlayerState = PlayerState.Idle;
                else if (obj.GetComponent<Tile>())
                {
                    Tile tile = obj.GetComponent<Tile>();
                    Tile playerTile = GridManager.Instance.TileFromWorldPoint(Player.transform.position);

                    if (MathUtility.ManhattanDistance(tile.x, tile.y, playerTile.x, playerTile.y) <= Player.ActionPoint)
                    {
                        path = Navigation.FindPath(GridManager.Instance, playerTile, tile);
                        InitiatePlayerMovement();
                    }
                }
                break;
        }
    }

    private void HandleEndDragging(MouseInteractable obj)
    {
        switch (currentPlayerState)
        {
            case PlayerState.MovementPlanning:
                if (obj == this && path.Count > 0)
                    InitiatePlayerMovement();
                break;
        }
    }

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
                            else if (GridManager.Instance.IsAdjacent(tile, path.Last.Value) && path.Count < Player.ActionPoint)
                                AddWayPoint(tile);
                        }
                        else if (GridManager.Instance.IsAdjacent(tile, GridManager.Instance.TileFromWorldPoint(Player.transform.position)))
                            AddWayPoint(tile);
                    }
                    break;
            }
    }
}
