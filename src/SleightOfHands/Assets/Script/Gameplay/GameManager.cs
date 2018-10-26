using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameState : int
{
    Default = 0,
    Exploration,
    Idle,
    MovementPlanning,
    MovementConfirmation,
    Move,
}

public class GameManager : MonoBehaviour
{
    //private static GameManager singleton = new GameManager();
    //public static GameManager Singleton
    //{
    //    get
    //    {
    //        return singleton;
    //    }
    //}

    public static GameManager Singleton { get; private set; }

    //public class EventOnGameState : UnityEvent<GameState> {}
    //public EventOnGameState OnCurrentGameStateReset = new EventOnGameState();

    public class EventOnGameStateChange : UnityEvent<GameState, GameState> {}
    public EventOnGameStateChange OnCurrentGameStateChange = new EventOnGameStateChange();

    public class EventOnWayPointsChange : UnityEvent<Path<Tile>> {}
    public EventOnWayPointsChange OnPathChange = new EventOnWayPointsChange();

    [SerializeField] private GameState initialState = (GameState)1;

    private Path<Tile> path;

    private GameState currentGameState;
    public GameState CurrentGameState
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
                    case GameState.MovementPlanning:
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

                GameState previousState = CurrentGameState;
                currentGameState = value;

                // After entering the new state
                switch (currentGameState)
                {
                    case GameState.Exploration:
                        MouseInputManager.Singleton.OnObjectClicked.AddListener(HandleMouseClick);
                        MouseInputManager.Singleton.OnEndDragging.AddListener(HandleEndDragging);
                        MouseInputManager.Singleton.OnCurrentMouseTargetChange.AddListener(HandleMouseTargetChange);
                        ResetToIdle();
                        break;
                    case GameState.Idle:
                        if (previousState != GameState.Move)
                        {
                            path = null;
                            OnPathChange.Invoke(path);
                        }
                        break;
                    case GameState.MovementPlanning:
                        path = new Path<Tile>(GridManager.Instance.TileFromWorldPoint(GridManager.Instance.Player.transform.position));
                        OnPathChange.Invoke(path);
                        break;
                    case GameState.MovementConfirmation:
                        // TODO: Show ListMenu
                        break;
                    case GameState.Move:
                        path = null;
                        OnPathChange.Invoke(path);
                        ActionManager.Singleton.Execute(ResetToIdle);
                        break;
                }

                OnCurrentGameStateChange.Invoke(previousState, value);
            }
        }
    }

    private GameManager() {}

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
        CurrentGameState = GameState.Idle;
    }

    private void InitiatePlayerMovement()
    {
        for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
            ActionManager.Singleton.Add(new Movement(GridManager.Instance.Player, tile));

        CurrentGameState = GameState.Move;
    }

    private void HandleMouseClick(MouseInteractable obj)
    {
        switch (currentGameState)
        {
            case GameState.Idle:
                if (obj.GetComponent<player>() && GridManager.Instance.Player.ActionPoint > 0)
                    CurrentGameState = GameState.MovementPlanning;
                break;
            case GameState.MovementPlanning:
                if (obj.GetComponent<player>())
                    CurrentGameState = GameState.Idle;
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
            case GameState.MovementPlanning:
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
                case GameState.MovementPlanning:
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
