using UnityEngine;

public enum PlayerState : int
{
    Idle,
    MovementPlanning,
    MovementConfirmation,
    Move,
}

public class PlayerController : MouseInteractable
{
    public EventOnDataChange<PlayerState> OnCurrentGameStateChange = new EventOnDataChange<PlayerState>();
    public EventOnDataUpdate<Path<Tile>> OnPathUpdate = new EventOnDataUpdate<Path<Tile>>();

    public player Player { get; private set; }

    private bool isEnabled = false;
    private Path<Tile> path;

    private PlayerState currentGameState;
    public PlayerState CurrentGameState
    {
        get
        {
            return currentGameState;
        }

        private set
        {
#if UNITY_EDITOR
            LogUtility.PrintLogFormat("PlayerController", "Made a transition to {0}.", value);
#endif

            // Reset current state
            if (value == currentGameState)
            {
                switch (currentGameState)
                {
                    case PlayerState.MovementPlanning:
                        path.Clear();
                        OnPathUpdate.Invoke(path);
                        break;
                }
            }
            else
            {
                // Before leaving the previous state
                //switch (currentGameState)
                //{
                //}

                PlayerState previousPlayerState = CurrentGameState;
                currentGameState = value;

                // After entering the new state
                switch (currentGameState)
                {
                    case PlayerState.Idle:
                        if (previousPlayerState != PlayerState.Move)
                        {
                            path = null;
                            OnPathUpdate.Invoke(path);
                        }
                        break;
                    case PlayerState.MovementPlanning:
                        path = new Path<Tile>(GridManager.Instance.TileFromWorldPoint(Player.transform.position));
                        OnPathUpdate.Invoke(path);
                        break;
                    case PlayerState.MovementConfirmation:
                        // TODO: Show ListMenu
                        break;
                    case PlayerState.Move:
                        path = null;
                        OnPathUpdate.Invoke(path);
                        ActionManager.Singleton.Execute(ResetToIdle);
                        break;
                }

                OnCurrentGameStateChange.Invoke(previousPlayerState, currentGameState);
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
            MouseInputManager.Singleton.OnObjectClicked.AddListener(HandleMouseClick);
            MouseInputManager.Singleton.OnEndDragging.AddListener(HandleEndDragging);
            MouseInputManager.Singleton.OnCurrentMouseTargetChange.AddListener(HandleMouseTargetChange);

            isEnabled = true;
        }
    }

    internal void Disable()
    {
        if (isEnabled)
        {
            MouseInputManager.Singleton.OnObjectClicked.RemoveListener(HandleMouseClick);
            MouseInputManager.Singleton.OnEndDragging.RemoveListener(HandleEndDragging);
            MouseInputManager.Singleton.OnCurrentMouseTargetChange.RemoveListener(HandleMouseTargetChange);

            isEnabled = false;
        }
    }

    private void AddWayPoint(Tile tile)
    {
        path.AddLast(tile);

        OnPathUpdate.Invoke(path);
    }

    private void RemoveWayPoint()
    {
        path.RemoveLast();

        OnPathUpdate.Invoke(path);
    }

    private void ResetToIdle()
    {
        CurrentGameState = PlayerState.Idle;
    }

    private void InitiatePlayerMovement()
    {
        for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
            ActionManager.Singleton.Add(new Movement(GetComponent<player>(), tile));

        CurrentGameState = PlayerState.Move;
    }

    private void HandleMouseClick(MouseInteractable obj)
    {
        switch (currentGameState)
        {
            case PlayerState.Idle:
                if (obj == this && Player.ActionPoint > 0)
                    CurrentGameState = PlayerState.MovementPlanning;
                break;
            case PlayerState.MovementPlanning:
                if (obj == this)
                    CurrentGameState = PlayerState.Idle;
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
        switch (currentGameState)
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
            switch (currentGameState)
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
