﻿using System;
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
        SpawnEntities();
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

    private void SpawnEntities()
    {
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
                    Player = Instantiate(ResourceUtility.GetPrefab<player>("PlayerDummy"), spawnPosition, spawnRotation, GridManager.Instance.environmentHolder);
                    break;

                case SpawnData.Type.Guard:
                    Instantiate(ResourceUtility.GetPrefab<Enemy>("GuardDummy"), spawnPosition, spawnRotation, GridManager.Instance.environmentHolder);
                    break;

            }

        }

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
                return GetEnumFromString<Type>(typeString);
            }
        }

        public string typeString;
        public Vector2Int position;
        public string[] settings;

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

        public E GetEnumFromString<E>(string enumString) where E : struct {
            return (E)Enum.Parse(typeof(E), enumString, true);
        }

    }
}
