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

    private Path<Vector2Int> path;

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
                    ResetToIdle();
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

    private void HandleMouseClick(MouseInteractable clickedObject)
    {
        switch (currentGameState)
        {
            case GameState.Idle:
                if (clickedObject.GetComponent<player>())
                    CurrentGameState = GameState.MovementPlanning;
                break;
            case GameState.MovementPlanning:
                if (clickedObject.GetComponent<player>())
                    CurrentGameState = GameState.Idle;
                else if (clickedObject.GetComponent<Tile>())
                {
                    player _player = GridManager.Instance.Player;
                    Path<Tile> path = Navigation.FindPath(GridManager.Instance, GridManager.Instance.TileFromWorldPoint(_player.transform.position), clickedObject.GetComponent<Tile>());

                    Debug.Log(path);

                    for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
                        ActionManager.Singleton.Add(new Movement(_player, tile));

                    CurrentGameState = GameState.Move;
                }
                break;
        }
    }
}
