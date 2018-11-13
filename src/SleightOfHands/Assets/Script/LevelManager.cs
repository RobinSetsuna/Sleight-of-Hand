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

    public EventOnDataUpdate<Effects> onNewTurnUpdateAttribute = new EventOnDataUpdate<Effects>();

    public EventOnDataUpdate<int> onCurrentTurnChange = new EventOnDataUpdate<int>();
    public EventOnDataUpdate<int> onRoundNumberChange = new EventOnDataUpdate<int>();

    public EventOnDataUpdate<Phase> OnCurrentPhaseChangeForPlayer = new EventOnDataUpdate<Phase>();
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
                    // for the Environment round, each unit will have a single start, action, end phase, so add it differently
                    // RoundNumber++;
                    // onRoundNumberChange.Invoke(RoundNumber);
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
                    if (CurrentRound == Round.Player)
                    {
                        CurrentPhase = Phase.Action;
                    }
                    break;
                case Phase.Action:
//                    if (CurrentRound == Round.Environment)
//                        CurrentPhase = Phase.End;
                    break;
                case Phase.End:
                    if (CurrentRound == Round.Player)
                    {
                        RoundNumber++;
                        onRoundNumberChange.Invoke(RoundNumber);
                        CurrentPhase = Phase.Start;
                    }
                    Debug.Log("added,current round: " + CurrentRound);
                    break;
            }
        }
    }

    public string levelFolderPath;
    public string levelFilename;
    //EnemyList
    public List<Enemy> Enemies;
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
        SpawnEntities();
        GridManager.Instance.Initialize();

        CardManager.Instance.InitCardDeck();
        CardManager.Instance.RandomGetCard(3);

        RoundNumber = 0;
        CurrentPhase = Phase.Start;

        UIManager.Singleton.Open("HUD", UIManager.UIMode.PERMANENT);
    }

    internal void EndPlayerActionPhase()
    {
        if (CurrentPhase == Phase.Action && CurrentRound == Round.Player)
            CurrentPhase = Phase.End;
    }
    internal void EndEnvironmentActionPhase()
    {
        if (CurrentPhase == Phase.Action && CurrentRound == Round.Environment)
            CurrentPhase = Phase.End;
    }
    internal void StartEnvironmentActionPhase()
    {
        if (CurrentPhase == Phase.Start && CurrentRound == Round.Environment)
            CurrentPhase = Phase.Action;
    }
    internal void StartNextPhaseTurn()
    {
        if (CurrentPhase == Phase.End)
            CurrentPhase = Phase.Start;
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
        Enemies = new List<Enemy>();
        //int index = 0;
        foreach (SpawnData spawnData in currentLevel.spawns) {
            Vector3 spawnPosition = GridManager.Instance.GetWorldPosition(spawnData.position.x, spawnData.position.y) + new Vector3(0, 1, 0);

            Quaternion spawnRotation = Quaternion.identity;
            string heading = spawnData.GetSetting("heading");
            if (heading != null) {
                float yRot = 0;
                switch (heading) {
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

            switch (spawnData.SpawnType) {

                case SpawnData.Type.Player:
                    Player = GridManager.Instance.Spawn(ResourceUtility.GetPrefab<player>("player_temp"), spawnPosition, spawnRotation); //Instantiate(ResourceUtility.GetPrefab<player>("player_temp"), spawnPosition, spawnRotation, GridManager.Instance.EnvironmentRoot);
                    // Player.GetComponent<player>().initializeEventListener();
                    GameObject.FindGameObjectWithTag("Player").AddComponent<Effects>();
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Effects>().SetOwner("Player");
                    break;

                case SpawnData.Type.Guard:
                    var temp = GridManager.Instance.Spawn(ResourceUtility.GetPrefab<GameObject>("GuardDummy"), spawnPosition, spawnRotation); //Instantiate(ResourceUtility.GetPrefab<GameObject>("GuardDummy"), spawnPosition, spawnRotation, GridManager.Instance.EnvironmentRoot);
                    temp.AddComponent<Effects>();
                    temp.GetComponent<Effects>().SetOwner("Enemy");
                    temp.tag = "Enemy";
                    temp.GetComponent<Enemy>().SetPathList(spawnData.GetPath());
                    temp.GetComponent<Enemy>().SetDetectionState(EnemyDetectionState.Normal); // set default detection state
//                    temp.ID = index;
//                    index++;
                    Enemies.Add(temp.GetComponent<Enemy>());
                    break;
            }

        }
        EnemyManager.Instance.setEnemies(Enemies);
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
