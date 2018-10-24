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
                    MouseInputManager.Singleton.OnNewClickedObject.AddListener(HandleMouseClick);
                    CurrentGameState = GameState.Idle;
                    break;
                case GameState.MovementConfirmation:
                    // TODO: Show ListMenu
                    break;
                case GameState.Move:
                    // TODO: Make player character move
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

    private void HandleMouseClick(GameObject clickedObject)
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
                //else if (clickedObject.GetComponent<Tile>())
                    //TODO: Add destination
                break;
        }
    }
}
