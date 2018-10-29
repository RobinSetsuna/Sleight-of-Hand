using UnityEngine;

public enum PlayerState : int
{
    Default = 0,
    Exploration,
    Idle,
    MovementPlanning,
    MovementConfirmation,
    Move,
}

public class playerController : MonoBehaviour
{
    //private static GameManager singleton = new GameManager();
    //public static GameManager Singleton
    //{
    //    get
    //    {
    //        return singleton;
    //    }
    //}

    public static playerController Singleton { get; private set; }

    //public class EventOnGameState : UnityEvent<GameState> {}
    //public EventOnGameState OnCurrentGameStateReset = new EventOnGameState();

    public EventOnDataChange2<PlayerState> OnCurrentGameStateChange = new EventOnDataChange2<PlayerState>();
    public EventOnDataChange1<Path<Tile>> OnPathChange = new EventOnDataChange1<Path<Tile>>();

    [SerializeField] private PlayerState initialState = (PlayerState)1;

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
            Debug.LogFormat("[GameManager] Make a transition to {0}.", value);
#endif

            // Reset current state
            if (value == currentGameState)
            {
                switch (currentGameState)
                {
                    case PlayerState.MovementPlanning:
                        path.Clear();
                        OnPathChange.Invoke(path);
                        break;
                }
            }
            else
            {
                // Before leaving the previous state
                //switch (currentGameState)
                //{
                //}

                PlayerState previousState = CurrentGameState;
                currentGameState = value;

                // After entering the new state
                switch (currentGameState)
                {
                    case PlayerState.Exploration:
                        MouseInputManager.Singleton.OnObjectClicked.AddListener(HandleMouseClick);
                        MouseInputManager.Singleton.OnEndDragging.AddListener(HandleEndDragging);
                        MouseInputManager.Singleton.OnCurrentMouseTargetChange.AddListener(HandleMouseTargetChange);
                        ResetToIdle();
                        break;
                    case PlayerState.Idle:
                        if (previousState != PlayerState.Move)
                        {
                            path = null;
                            OnPathChange.Invoke(path);
                        }
                        break;
                    case PlayerState.MovementPlanning:
                        path = new Path<Tile>(GridManager.Instance.TileFromWorldPoint(GridManager.Instance.Player.transform.position));
                        OnPathChange.Invoke(path);
                        break;
                    case PlayerState.MovementConfirmation:
                        // TODO: Show ListMenu
                        break;
                    case PlayerState.Move:
                        path = null;
                        OnPathChange.Invoke(path);
                        ActionManager.Singleton.Execute(ResetToIdle);
                        break;
                }

                OnCurrentGameStateChange.Invoke(previousState, value);
            }
        }
    }

    private playerController() { }

    private void OnEnable()
    {
        if (!Singleton)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (this != Singleton)
            Destroy(gameObject);

        CurrentGameState = initialState;
    }

    private void AddWayPoint(Tile tile)
    {
        path.AddLast(tile);

        OnPathChange.Invoke(path);
    }

    private void RemoveWayPoint()
    {
        path.RemoveLast();

        OnPathChange.Invoke(path);
    }

    private void ResetToIdle()
    {
        CurrentGameState = PlayerState.Idle;
    }

    private void InitiatePlayerMovement()
    {
        for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
            ActionManager.Singleton.Add(new Movement(GridManager.Instance.Player, tile));

        CurrentGameState = PlayerState.Move;
    }

    private void HandleMouseClick(MouseInteractable obj)
    {
        switch (currentGameState)
        {
            case PlayerState.Idle:
                if (obj.GetComponent<player>() && GridManager.Instance.Player.ActionPoint > 0)
                    CurrentGameState = PlayerState.MovementPlanning;
                break;
            case PlayerState.MovementPlanning:
                if (obj.GetComponent<player>())
                    CurrentGameState = PlayerState.Idle;
                else if (obj.GetComponent<Tile>())
                {
                    Tile tile = obj.GetComponent<Tile>();

                    player _player = GridManager.Instance.Player;
                    Tile playerTile = GridManager.Instance.TileFromWorldPoint(_player.transform.position);

                    if (MathUtility.ManhattanDistance(tile.x, tile.y, playerTile.x, playerTile.y) <= _player.ActionPoint)
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
                if (obj.GetComponent<player>() && path.Count > 0)
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
                    if (obj.GetComponent<player>())
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
                            else if (GridManager.Instance.IsAdjacent(tile, path.Last.Value) && path.Count < GridManager.Instance.Player.ActionPoint)
                                AddWayPoint(tile);
                        }
                        else if (GridManager.Instance.IsAdjacent(tile, GridManager.Instance.TileFromWorldPoint(GridManager.Instance.Player.transform.position)))
                            AddWayPoint(tile);
                    }
                    break;
            }
    }
}
