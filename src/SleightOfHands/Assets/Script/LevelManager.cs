using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum Phase : int
{
    Start = 0,
    Action,
    End,
}

public enum Round : int
{
    Player = 0,
    Environment = 1,
}

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance {
        get {
            if (instance == null) instance = GameObject.Find("Level Manager").GetComponent<LevelManager>();
            return instance;
        }
    }

    public EventOnDataChange<Phase> OnCurrentPhaseChange = new EventOnDataChange<Phase>();

    public List<Unit> units;
    public player Player { get; private set; }

    internal PlayerController playerController
    {
        get
        {
            return Player.GetComponent<PlayerController>();
        }
    }

    private int round;
    private Phase currentPhase;

    public Round CurrentRound
    {
        get
        {
            return (Round)(round % 2);
        }
    }

    public int CurrentTurn
    {
        get
        {
            return round / 2 + 1;
        }
    }
    
    public Phase CurrentPhase
    {
        get
        {
            return currentPhase;
        }

        private set
        {
#if UNITY_EDITOR
            LogUtility.PrintLogFormat("LevelManager", "Made a transition to {0}.", value);
#endif

            // Before leaving the previous state
            switch (currentPhase)
            {
                case Phase.Action:
                    switch (CurrentRound)
                    {
                        case Round.Player:
                            playerController.Disable();
                            break;
                    }
                    break;
            }

            Phase previousPhase = currentPhase;
            currentPhase = value;

            // After entering the new state
            switch (currentPhase)
            {
                case Phase.Action:
                    switch (CurrentRound)
                    {
                        case Round.Player:
                            playerController.Enable();
                            break;
                    }
                    break;
            }

            OnCurrentPhaseChange.Invoke(previousPhase, currentPhase);

            // In phase
            switch (currentPhase)
            {
                case Phase.Start:
                    CurrentPhase = Phase.Action;
                    break;
                case Phase.Action:
                    if (CurrentRound == Round.Environment)
                        CurrentPhase = Phase.End;
                    break;
                case Phase.End:
                    round++;
                    CurrentPhase = Phase.Start;
                    break;
            }
        }
    }

    public string levelFolderPath;
    public string levelFilename;

    [SerializeField]
    private LevelData currentLevel;
    public LevelData CurrentLevel
    {
        get
        {
            return currentLevel;
        }
    }

    //private void Start() {
    //    LoadLevel("test_level");
    //}

    public void StartLevel(string level)
    {
        LoadLevel(level);
        SpawnPlayer();
        GridManager.Instance.Initialize();

        round = 0;
        CurrentPhase = Phase.Start;
    }

	private void LoadLevel(string levelFilename)
    {
        string jsonPath = Path.Combine(Application.streamingAssetsPath, levelFolderPath);
        jsonPath = Path.Combine(jsonPath, levelFilename + ".json");

        string json = File.ReadAllText(jsonPath);

        LevelData levelData = LevelData.CreateFromJSON(json);
        currentLevel = levelData;

        GridManager.Instance.GenerateMap(levelData, out units);
    }

    private void SpawnPlayer()
    {
        Player = Instantiate(ResourceUtility.GetPrefab<player>("PlayerDummy"), GridManager.Instance.GetWorldPosition(currentLevel.playerBirthplaceX, currentLevel.playerBirthplaceY) + new Vector3(0, 1, 0), Quaternion.identity, GridManager.Instance.environmentHolder);
        units.Add(Player);
    }

    [System.Serializable]
    public class LevelData {

        public string name;
        public int width;
        public int playerBirthplaceX;
        public int playerBirthplaceY;
        public int[] tiles;
        public SpawnData[] spawns;

        public static LevelData CreateFromJSON(string jsonFile) {
            LevelData levelInfo = JsonUtility.FromJson<LevelData>(jsonFile);
            return levelInfo;
        }

    }

    [System.Serializable]
    public class SpawnData {
        public int id;
        public Vector2Int position;
        public string[] settings;
    }
}
