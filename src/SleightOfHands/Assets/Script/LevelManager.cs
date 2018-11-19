using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public enum Phase : int
{
    Start = 0,
    Action,
    End,
    Success,
    Failure,
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

    public UnityEvent onGameEnd = new UnityEvent();
    public EventOnDataUpdate<Effects> onNewTurnUpdateAttribute = new EventOnDataUpdate<Effects>();

    public EventOnDataUpdate<int> onCurrentTurnChange = new EventOnDataUpdate<int>();
    public EventOnDataUpdate<int> onRoundNumberChange = new EventOnDataUpdate<int>();

    public EventOnDataUpdate<Phase> onCurrentPhaseChangeForPlayer = new EventOnDataUpdate<Phase>();
    public EventOnDataUpdate<Phase> OnCurrentPhaseChangeForEnvironment = new EventOnDataUpdate<Phase>();

    // public List<Unit> units;
    public player Player { get; private set; }

    internal PlayerController playerController
    {
        get
        {
            return Player.GetComponent<PlayerController>();
        }
    }

    public int RoundNumber { get; private set; }

    public Round CurrentRound
    {
        get
        {
            return (Round)(RoundNumber % 2);
        }
    }

    public int CurrentTurn
    {
        get
        {
            return RoundNumber / 2 + 1;
        }
    }

    private Phase currentPhase;
    public Phase CurrentPhase
    {
        get
        {
            return currentPhase;
        }

        private set
        {
#if UNITY_EDITOR
            Debug.Log(LogUtility.MakeLogString("LevelManager", string.Format("Made a transition to {0}.", value)));
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
                    // for the Environment round, each unit will have a single start, action, end phase, so add it differently
                    RoundNumber++;
                    onRoundNumberChange.Invoke(RoundNumber);
                    if (CurrentRound == Round.Player)
                        onCurrentTurnChange.Invoke(CurrentTurn);
                    break;

                case Phase.Success:
                    UIManager.Singleton.Open("ExplorationSuccess");
                    ActionManager.Singleton.Clear();
                    onGameEnd.Invoke();
                    return;

                case Phase.Failure:
                    UIManager.Singleton.Open("ExplorationFailure");
                    ActionManager.Singleton.Clear();
                    onGameEnd.Invoke();
                    return;
            }

            switch (CurrentRound)
            {
                case Round.Player:
                    onCurrentPhaseChangeForPlayer.Invoke(currentPhase);
                    break;

                case Round.Environment:
                    OnCurrentPhaseChangeForEnvironment.Invoke(currentPhase);
                    break;
            }

            // In new phase
            switch (currentPhase)
            {
                case Phase.Start:
                    //if (CurrentRound == Round.Player)
                    //{
                    CurrentPhase = Phase.Action;
                    //}
                    break;

                case Phase.Action:
                    if (CurrentRound == Round.Environment)
                        EnemyManager.Instance.StartEnemyRound(CurrentTurn, EndActionPhase);
                    break;

                case Phase.End:
                    //if (CurrentRound == Round.Player)
                    //{
                    //    RoundNumber++;
                    //    onRoundNumberChange.Invoke(RoundNumber);
                    CurrentPhase = Phase.Start;
                    //}
                    break;
            }
        }
    }

    public string levelFolderPath;
    public string levelFilename;

    //EnemyList
    public List<Enemy> Enemies = new List<Enemy>();

    //Card related
    public GameObject Smoke;
    public GameObject Haste;
    public GameObject Glue;
    GameObject canvas;
    float canvasWidth;
    float canvasHeight;
    int cardsNumberOnCanvas = 0;

    [SerializeField] private LevelData currentLevel;
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
        InitializeLevel();
        UIManager.Singleton.Open("HUD", UIManager.UIMode.PERMANENT);
    }

    public void Restart()
    {
        InitializeLevel();
        UIManager.Singleton.Redraw("HUD");
    }

    private void InitializeLevel()
    {
        SpawnEntities();

        GridManager.Instance.Initialize();
        
        
        //light map initialize
        Instantiate(ResourceUtility.GetPrefab<GameObject>("LightMap"), Vector3.zero, Quaternion.identity);
        //change when light config implemented.

        CardManager.Instance.Initialize();
        CardManager.Instance.RandomGetCard(3);

        RoundNumber = -1;
        CurrentPhase = Phase.Start;
    }

    public void NotifySuccess()
    {
        CurrentPhase = Phase.Success;
    }

    internal void EndActionPhase()
    {
        CurrentPhase = Phase.End;
    }

    //internal void EndEnvironmentActionPhase()
    //{
    //    if (CurrentPhase == Phase.Action && CurrentRound == Round.Environment)
    //        CurrentPhase = Phase.End;
    //}

    //internal void StartEnvironmentActionPhase()
    //{
    //    if (CurrentPhase == Phase.Start && CurrentRound == Round.Environment)
    //        CurrentPhase = Phase.Action;
    //}

    //internal void StartNextPhaseTurn()
    //{
    //    if (CurrentPhase == Phase.End)
    //        CurrentPhase = Phase.Start;
    //}

    bool isRestartButtonDown = false;
    void Update()
    {
        if (!Input.GetKeyDown("1"))
            isRestartButtonDown = false;
        else if (!isRestartButtonDown)
        {
            isRestartButtonDown = true;
            Restart();
        }
    }

    private void LoadLevel(string levelFilename)
    {
        string jsonPath = Path.Combine(Application.streamingAssetsPath, levelFolderPath);
        jsonPath = Path.Combine(jsonPath, levelFilename + ".json");

        string json = File.ReadAllText(jsonPath);

        LevelData levelData = LevelData.CreateFromJSON(json);
        currentLevel = levelData;

        GridManager.Instance.GenerateMap(levelData);
    }

    private void SpawnEntities()
    {
        if (Player)
            Destroy(Player.gameObject);

        foreach (Enemy enemy in Enemies)
            Destroy(enemy.gameObject);

        Enemies.Clear();

        List<EnemyController> enemies = new List<EnemyController>();
        int enemyUID = 0;

        foreach (SpawnData spawnData in currentLevel.spawns)
        {
            Vector3 spawnPosition = GridManager.Instance.GetWorldPosition(spawnData.position.x, spawnData.position.y) + new Vector3(0, 1, 0);
            Quaternion spawnRotation = Quaternion.identity;

            string heading = spawnData.GetSetting("heading");

            if (heading != null)
            {
                float yRot = 0;

                switch (heading)
                {
                    case "north":
                        yRot = 0;
                        break;
                    case "east":
                        yRot = 90;
                        break;
                    case "west":
                        yRot = 270;
                        break;
                    case "south":
                        yRot = 180;
                        break;
                }

                spawnRotation = Quaternion.Euler(0, yRot, 0);
            }

            switch (spawnData.SpawnType)
            {
                case SpawnData.Type.Player:
                    Player = GridManager.Instance.Spawn(ResourceUtility.GetPrefab<player>("player_temp"), spawnPosition, spawnRotation); //Instantiate(ResourceUtility.GetPrefab<player>("player_temp"), spawnPosition, spawnRotation, GridManager.Instance.EnvironmentRoot);
                    Player.onStatisticChange.AddListener(HandlePlayerAttributeChange);
                    break;

                case SpawnData.Type.Guard:
                    Enemy enemy = GridManager.Instance.Spawn(ResourceUtility.GetPrefab<Enemy>("GuardDummy"), spawnPosition, spawnRotation); //Instantiate(ResourceUtility.GetPrefab<GameObject>("GuardDummy"), spawnPosition, spawnRotation, GridManager.Instance.EnvironmentRoot);
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();
                    enemyController.SetWayPoints(spawnData.GetPath());
                    enemyController.Mode = EnemyMode.Patrolling;
                    enemyController.UID = enemyUID++;
                    Enemies.Add(enemy);
                    enemies.Add(enemyController);
                    break;
            }
        }

        EnemyManager.Instance.Initialize(enemies);
    }

    public void NextRound()
    {
        RoundNumber++;
        onRoundNumberChange.Invoke(RoundNumber);
    }

    public void RemoveEnemy(Enemy obj)
    {
        Enemies.Remove(obj);
    }

    private void HandlePlayerAttributeChange(StatisticType statistic, float previousValue, float currentValue)
    {
        if (statistic == StatisticType.Hp && currentValue <= 0)
            CurrentPhase = Phase.Failure;
    }

    [System.Serializable]
    public class LevelData {

        public string name;
        public int width;
        public int[] tiles;
        public SpawnData[] spawns;
        public int[] endingPoints;

        public static LevelData CreateFromJSON(string jsonFile) {
            LevelData levelInfo = JsonUtility.FromJson<LevelData>(jsonFile);
            return levelInfo;
        }

        public int GetTile(int x, int y) {
            int i = x + y * width;
            return tiles[i];
        }

        public Vector2Int GetSize() {
            int height = (tiles.Length / width);
            if (tiles.Length % width != 0) height++;
            return new Vector2Int(width, height);
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
                return GetEnumFromString<Type>(typeString);
            }
        }

        public string typeString;
        public Vector2Int position;
        public string[] settings;
        public string[] path;

        public string GetSetting(string settingString) {
            if (settings == null) return null;
            settingString = settingString.ToLower();
            for (int i = 0; i < settings.Length; i += 2) {
                if (settingString.Equals(settings[i])) {
                    return settings[i + 1];
                }
            }
            return null;
        }


        public List<Vector2Int> GetPath()
        {
            List<Vector2Int> pathList = new List<Vector2Int>();
            if (path == null) return null;
            for (int i = 0; i < path.Length; i++) {
                string[] pos_str = path[i].Split(',');
                Vector2Int temp = new Vector2Int(Int32.Parse(pos_str[0]),Int32.Parse(pos_str[1]));
                pathList.Add(temp);
            }
            return pathList;
        }

        public E GetEnumFromString<E>(string enumString) where E : struct {
            return (E)Enum.Parse(typeof(E), enumString, true);
        }
    }
}
