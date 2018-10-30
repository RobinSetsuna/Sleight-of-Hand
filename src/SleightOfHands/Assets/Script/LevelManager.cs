using System;
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
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = GameObject.Find("Level Manager");

                if (go)
                    instance = go.GetComponent<LevelManager>();
            }
                

            return instance;
        }
    }

    public EventOnDataUpdate<int> onCurrentTurnChange = new EventOnDataUpdate<int>();
    public EventOnDataUpdate<Phase> OnCurrentPhaseChangeForPlayer = new EventOnDataUpdate<Phase>();
    public EventOnDataUpdate<Phase> OnCurrentPhaseChangeForEnvironment = new EventOnDataUpdate<Phase>();

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
            // Before leaving the previous phase
            //switch (currentPhase)
            //{
            //}

            currentPhase = value;

            // After entering a new phase
            switch (currentPhase)
            {
                case Phase.Start:
                    round++;
                    if (CurrentRound == Round.Player)
                        onCurrentTurnChange.Invoke(CurrentTurn);
                    break;
            }

            switch (CurrentRound)
            {
                case Round.Player:
                    OnCurrentPhaseChangeForPlayer.Invoke(currentPhase);
                    break;
                case Round.Environment:
                    OnCurrentPhaseChangeForEnvironment.Invoke(currentPhase);
                    break;
            }
            
            // In new phase
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

    public void StartLevel(string level)
    {
        LoadLevel(level);
        SpawnPlayer();
        GridManager.Instance.Initialize();

        round = -1;
        CurrentPhase = Phase.Start;

        UIManager.Singleton.Open("HUD", UIManager.UIMode.PERMANENT);
    }

    internal void EndPlayerActionPhase()
    {
        if (CurrentPhase == Phase.Action && CurrentRound == Round.Player)
            CurrentPhase = Phase.End;
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
        SpawnData playerSpawnData = null;
        foreach (SpawnData spawnData in currentLevel.spawns) {
            if (spawnData.SpawnType == SpawnData.Type.Player) {
                playerSpawnData = spawnData;
            }
        }
        if (playerSpawnData == null) return;

        Vector3 spawnPosition = GridManager.Instance.GetWorldPosition(playerSpawnData.position.x, playerSpawnData.position.y) + new Vector3(0, 1, 0);
        Player = Instantiate(ResourceUtility.GetPrefab<player>("PlayerDummy"), spawnPosition, Quaternion.identity, GridManager.Instance.environmentHolder);
    }

    [System.Serializable]
    public class LevelData {

        public string name;
        public int width;
        public int[] tiles;
        public SpawnData[] spawns;

        public static LevelData CreateFromJSON(string jsonFile) {
            LevelData levelInfo = JsonUtility.FromJson<LevelData>(jsonFile);
            return levelInfo;
        }

    }

    [System.Serializable]
    public class SpawnData {

        public enum Type {
            None,
            Player,
            Guard
        }
        public Type SpawnType {
            get {
                return (Type)Enum.Parse(typeof(Type), typeString, true);
            }
        }

        public string typeString;
        public Vector2Int position;
        public string[] settings;

        public string GetSetting(string settingString) {
            for (int i = 0; i < settings.Length; i += 2) {
                if (settingString.Equals(settings[i])) {
                    return settings[i + 1];
                }
            }
            return null;
        }

    }
}
