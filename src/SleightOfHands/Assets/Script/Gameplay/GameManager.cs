using UnityEngine;

public enum GameState : int
{
    Start = 0,
    MainMenu,
    Preparation,
    Exploration,
    End = 0,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; }

    public EventOnDataChange<GameState> OnCurrentGameStateChange = new EventOnDataChange<GameState>();

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
            LogUtility.PrintLogFormat("GameManager", "Made a transition to {0}.", value);
#endif

            // Reset current state
            if (value == currentGameState)
            {
                //switch (currentGameState)
                //{

                //}
            }
            else
            {
                // Before leaving the previous state
                //switch (currentGameState)
                //{
                //}

                GameState previousGameState = CurrentGameState;
                currentGameState = value;

                // After entering the new state
                switch (currentGameState)
                {
                    case GameState.Exploration:
                        LevelManager.Instance.StartLevel("test_level");
                        break;
                }

                OnCurrentGameStateChange.Invoke(previousGameState, currentGameState);
            }
        }
    }

    private GameManager() {}

    public void Quit()
    {
        Application.Quit();
    }

    private void Start()
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
}
