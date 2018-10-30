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
    public static LevelManager Instance {
        get {
            if (instance == null) instance = GameObject.Find("Level Manager").GetComponent<LevelManager>();
            return instance;
        }
    }

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
            Round currentRound = CurrentRound;
            // Before leaving the previous phase
            switch (currentPhase)
            {
                case Phase.Action:
                    switch (currentRound)
                    {
                        case Round.Player:
                            playerController.Disable();
                            break;
                    }
                    break;
            }

            currentPhase = value;

            // After entering a new phase
            switch (currentPhase)
            {
                case Phase.Action:
                    switch (currentRound)
                    {
                        case Round.Player:
                            playerController.Enable();
                            break;
                    }
                    break;
            }

            switch (currentRound)
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
                    if (currentRound == Round.Environment)
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

    //card related
    public GameObject Smoke;
    public GameObject Haste;
    public GameObject Glue;
    GameObject canvas;
    float canvasWidth;
    float canvasHeight;
    int cardsNumberOnCanvas = 0;

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

        CardManager.Instance.InitCardDeck();
        IntiCanvas();
        InstantiateCard(CardManager.Instance.RandomGetCard());
        InstantiateCard(CardManager.Instance.RandomGetCard());
        InstantiateCard(CardManager.Instance.RandomGetCard());

        round = 0;
        CurrentPhase = Phase.Start;
    }

    public void InstantiateCard(Card card)
    {
        switch (card.cardName)
        {
            case "Smoke":
                InstantiateOnCanvas(Smoke);
                Smoke.GetComponent<CardInstance>().InitialCard(card);
                break;
            case "Haste":
                InstantiateOnCanvas(Haste);
                Haste.GetComponent<CardInstance>().InitialCard(card);
                break;
            case "Glue Trap":
                InstantiateOnCanvas(Glue);
                Glue.GetComponent<CardInstance>().InitialCard(card);
                break;
        }
    }

    void IntiCanvas()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvasWidth = canvas.gameObject.GetComponent<RectTransform>().rect.width;
        canvasHeight = canvas.gameObject.GetComponent<RectTransform>().rect.height;
    }
    void InstantiateOnCanvas(GameObject obj)
    {     
        float imageWidth = obj.gameObject.GetComponent<RectTransform>().rect.width;
        float imageHeight = obj.gameObject.GetComponent<RectTransform>().rect.height;
        GameObject imageSpawn = Instantiate(obj) as GameObject;

        imageSpawn.transform.SetParent(canvas.transform);
        imageSpawn.transform.localRotation = canvas.transform.rotation;

        if (cardsNumberOnCanvas == 0)
        {
            // instantiate at the right bottom corner of the canvas
            imageSpawn.transform.localPosition = new Vector3((canvasWidth / 2 - imageWidth / 2), (-canvasHeight / 2 + imageHeight / 2), 0);
            cardsNumberOnCanvas = 1;
        }
        else if (cardsNumberOnCanvas == 1)
        {
            imageSpawn.transform.localPosition = new Vector3((canvasWidth / 2 - imageWidth / 2) - 115, (-canvasHeight / 2 + imageHeight / 2), 0);
            cardsNumberOnCanvas = 2;
        }
        else if (cardsNumberOnCanvas == 2)
        {
            imageSpawn.transform.localPosition = new Vector3((canvasWidth / 2 - imageWidth / 2) - 230, (-canvasHeight / 2 + imageHeight / 2), 0);
            cardsNumberOnCanvas = 3;
        }


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
