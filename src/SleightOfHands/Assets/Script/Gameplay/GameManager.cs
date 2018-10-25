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

    public class EventOnGameStateChange : UnityEvent<GameState, GameState> {}
    EventOnGameStateChange OnCurrentGameStateChange = new EventOnGameStateChange();

    [SerializeField] private GameState initialState = (GameState)1;

    private LinkedList<Tile> wayPoints;

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

            if (currentGameState == value)
                return;

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
                    wayPoints = null;
                    break;
                case GameState.MovementPlanning:
                    wayPoints = new LinkedList<Tile>();
                    break;
                case GameState.MovementConfirmation:
                    // TODO: Show ListMenu
                    break;
                case GameState.Move:
                    ActionManager.Singleton.Execute(ResetToIdle);
                    break;
            }

            OnCurrentGameStateChange.Invoke(previousState, value);
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
        else
            Destroy(gameObject);

        CurrentGameState = initialState;
    }

    private void ResetToIdle()
    {
        CurrentGameState = GameState.Idle;
    }

    private void InitiatePlayerMovement(Path<Tile> path)
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
                if (obj.GetComponent<player>())
                    CurrentGameState = GameState.MovementPlanning;
                break;
            case GameState.MovementPlanning:
                if (obj.GetComponent<player>())
                    CurrentGameState = GameState.Idle;
                else if (obj.GetComponent<Tile>())
                    InitiatePlayerMovement(Navigation.FindPath(GridManager.Instance, GridManager.Instance.TileFromWorldPoint(GridManager.Instance.Player.transform.position), obj.GetComponent<Tile>()));
                break;
        }
    }

    private void HandleEndDragging(MouseInteractable obj)
    {
        switch (currentGameState)
        {
            case GameState.MovementPlanning:
                if (obj.GetComponent<player>() && wayPoints.Count > 0)
                    InitiatePlayerMovement(new Path<Tile>(GridManager.Instance.TileFromWorldPoint(GridManager.Instance.Player.transform.position), wayPoints));
                break;
        }
    }

    private void HandleMouseTargetChange(MouseInteractable obj)
    {
        if (MouseInputManager.Singleton.IsMouseDragging)
            switch (currentGameState)
            {
                case GameState.MovementPlanning:
                    if (obj.GetComponent<Tile>())
                    {
                        Tile tile = obj.GetComponent<Tile>();
                        
                        if (wayPoints.Count > 0)
                        {
                            if (wayPoints.Count > 1 && tile == wayPoints.Last.Previous.Value)
                                wayPoints.RemoveLast();
                            else if (GridManager.Instance.IsAdjacent(tile, wayPoints.Last.Value))
                                wayPoints.AddLast(tile);
                        }
                        else if (GridManager.Instance.IsAdjacent(tile, GridManager.Instance.TileFromWorldPoint(GridManager.Instance.Player.transform.position)))
                            wayPoints.AddLast(tile);
                    }
                    break;
            }
    }
}
