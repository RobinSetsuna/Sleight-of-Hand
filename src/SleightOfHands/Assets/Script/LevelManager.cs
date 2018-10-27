﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    private static LevelManager instance;
    public static LevelManager Instance {
        get {
            if (instance == null) instance = GameObject.Find("Level Manager").GetComponent<LevelManager>();
            return instance;
        }
    }

    public string levelFolderPath;
    public string levelFilename;

    [SerializeField]
    private LevelData debug;

    private void Start() {
        LoadLevel("test_level");
    }

	public void LoadLevel(string levelFilename) {

        string jsonPath = Path.Combine(Application.streamingAssetsPath, levelFolderPath);
        jsonPath = Path.Combine(jsonPath, levelFilename + ".json");

        string json = File.ReadAllText(jsonPath);

        LevelData levelData = LevelData.CreateFromJSON(json);
        debug = levelData;

        GridManager.Instance.GenerateMap(levelData);

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
        public int id;
        public Vector2Int position;
        public string[] settings;
    }

}
