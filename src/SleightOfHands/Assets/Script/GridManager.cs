using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour {

    public LayerMask unwalkableMask;
    public float nodeRadius;
    Node[,] grid;
    public Transform tilePrefab;
    public Vector2 mapSize;
    public static GridManager _instance;

    [Range(0,1)]
    public float outlinePercent;

    public bool dragging = false;
    
    void Start() {
        //GenerateMap ();
        _instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
            //Debug.Log("hold!");
        }
    }

//    public Node NodeFromWorldPoint(Vector3 worldPosition) {
//        float percentX = (worldPosition.x + mapSize.x/2) / mapSize.x;
//        float percentY = (worldPosition.z + mapSize.y/2) / mapSize.y;
//        percentX = Mathf.Clamp01(percentX);
//        percentY = Mathf.Clamp01(percentY);
//
//        int x = Mathf.RoundToInt((mapSize.x-1) * percentX);
//        int y = Mathf.RoundToInt((mapSize.y-1) * percentY);
//        return grid[x,y];
//    }
    public void GenerateMap() {

        string holderName = "Generated Map";
        if (transform.Find(holderName)) {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject (holderName).transform;
        //mapHolder.parent = transform;
        
        grid = new Node[Mathf.RoundToInt(mapSize.x),Mathf.RoundToInt(mapSize.y)];
        for (int x = 0; x < mapSize.x; x ++) {
            for (int y = 0; y < mapSize.y; y ++) {
                Vector3 tilePosition = new Vector3(-mapSize.x/2 +0.5f + x, 2, -mapSize.y/2 + 0.5f + y);
                bool walkable = !(Physics.CheckSphere(tilePosition,nodeRadius,unwalkableMask));
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one * (1-outlinePercent);
                newTile.parent = mapHolder;
                grid[x,y] = new Node(walkable,tilePosition);
            }
        }
    }
}

public class Node {
	
    public bool walkable;
    public Vector3 worldPosition;
	
    public Node(bool _walkable, Vector3 _worldPos) {
        walkable = _walkable;
        worldPosition = _worldPos;
    }
}