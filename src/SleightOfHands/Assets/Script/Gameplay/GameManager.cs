using UnityEngine;

/// <summary>
/// The state of the game
/// </summary>
public enum GameState : int
{
    Start = 0,
    MainMenu,
    Preparation,
    Exploration,
    End = 0,
}

/// <summary>
/// A FSM for the whole game at the highest level
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The unique instance
    /// </summary>
    public static GameManager Singleton { get; private set; }

    /// <summary>
    /// An event triggered whenever the state of the game changes
    /// </summary>
    public EventOnDataChange<GameState> onCurrentGameStateChange = new EventOnDataChange<GameState>();

    [SerializeField] private GameState initialState = (GameState)1;

    private GameState currentGameState;

    /// <summary>
    /// The current state of the game
    /// </summary>
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

                onCurrentGameStateChange.Invoke(previousGameState, currentGameState);
            }
        }
    }

    private GameManager() {}

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    private void Awake()
    {
        if (!Singleton)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (this != Singleton)
            Destroy(gameObject);
   }

    private void Start()
    {
        CurrentGameState = initialState;
    }
}
