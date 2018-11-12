using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileCollection", menuName = "Level/Tile Collection")]
public class Tileset : ScriptableObject {

	public TileCollection fallback;
    public TileCollection surrounded;
    public TileCollection surrounded1;
    public TileCollection surrounded2Adjacent;
    public TileCollection surrounded2Diagonal;
    public TileCollection surrounded3;
    public TileCollection straight;
    public TileCollection peninsula;
    public TileCollection corner;
    public TileCollection cornerFill;
    public TileCollection t;
    public TileCollection tFill;
    public TileCollection tFillLeft;
    public TileCollection tFillRight;

    [Serializable]
    public class TileCollection {

        public TileCollectionObject[] objects;

        public GameObject GetRandom() {

            // Grab random prefab
            float totalWeight = GetTotalWeight();
            float rndm = UnityEngine.Random.Range(0f, totalWeight);
            foreach (TileCollectionObject tco in objects) {
                rndm -= tco.weight;
                if (rndm <= 0) {
                    return tco.prefab;
                }
            }

            // Default if random failed
            if (objects.Length > 0) {
                return objects[0].prefab;
            }

            return null;

        }

        private float GetTotalWeight() {
            float totalWeight = 0;
            foreach (TileCollectionObject tco in objects) {
                totalWeight += tco.weight;
            }
            return totalWeight;
        }

        [Serializable]
        public class TileCollectionObject {
            public GameObject prefab;
            public float weight;
        }

    }

}
