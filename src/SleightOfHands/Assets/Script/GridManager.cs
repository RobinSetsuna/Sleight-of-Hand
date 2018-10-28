using System;
using UnityEngine;
using System.Collections.Generic;

///	<summary/>
/// GridManager - Grid Manager class 
/// tile highlight in range, store player designed path, generate path.
/// store grid[,] to manage all the tile in the map.
/// Usage: GridManager._instance.FUNCTION_NAME()
/// </summary>
public class GridManager : MonoBehaviour, INavGrid<Tile>
{
    private static GridManager instance;
    public static GridManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.Find("GridManager").GetComponent<GridManager>();
            return instance;
        }
    }

    public LayerMask unwalkableMask;
    public float nodeRadius;
    public Transform tilePrefab;
    public GameObject wallPrefab;
    public Vector2Int mapSize;
    [Range(0,1)] public float outlinePercent;
    
    [SerializeField] private float time_intervals;
    private float last_mouse_down;
    private Tile[,] grid;
    public bool dragging;
    public bool ok_to_drag;
    private HashSet<Tile>  recheck_list;
    private int range;
    public int checktimes;
    public Tile[] generatedPath;
    private int action_point; // temp use, replace later

    private Transform root;

    [Header("References")]
    public Transform environmentHolder;

    public int Length
    {
        get
        {
            return mapSize.x;
        }
    }

    public int Width
    {
        get
        {
            return mapSize.y;
        }
    }

    private void Start()
    {
        checktimes = 0;
        ok_to_drag = false;
        action_point = 5;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)&&ok_to_drag)
        {
            dragging = true;
            last_mouse_down = Time.unscaledTime;
            //Debug.Log("hold!");
        }
       // print("time: "+ (Time.unscaledTime -last_mouse_down));
        if (Time.unscaledTime -last_mouse_down > time_intervals)
        {
            //Debug.Log("NOT hold!");
            dragging = false;
        }
    }

    public bool IsAdjacent(Tile A, Tile B)
    {
        return MathUtility.ManhattanDistance(A.x, A.y, B.x, B.y) == 1;
    }

    public Tile TileFromWorldPoint(Vector3 worldPosition)
    {
        return grid[Mathf.FloorToInt(worldPosition.x / (2 * nodeRadius)), Mathf.FloorToInt(worldPosition.z / (2 * nodeRadius))];
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3((x * 2 + 1) * nodeRadius, 0, (y * 2 + 1) * nodeRadius);
    }

    public Vector3 GetWorldPosition(Vector2Int coordinates)
    {
        return GetWorldPosition(coordinates.x, coordinates.y);
    }

    /// <GenerateMap>
    /// generated grid map
    /// map_size : the number of cube in row and column
    /// node_radius: the cube radius
    /// parameter: None
    /// </summary>
    public void GenerateMap(LevelManager.LevelData levelData, out List<Unit> units)
    {
        if (!root)
            root = transform.Find("GridRoot");

        if (!root)
        {
            root = new GameObject("GridRoot").transform;
            root.parent = transform;
        }

        // Extract data from levelData
        if (levelData != null)
            mapSize = new Vector2Int(levelData.width, Mathf.CeilToInt(levelData.tiles.Length / levelData.width));
        
        // new grid with size [mapSize.x,maoSize.y]
        grid = new Tile[mapSize.x, mapSize.y];

        int numExistedTiles = root.childCount;

        units = new List<Unit>();

        for (int x = 0; x < mapSize.x; x ++)
            for (int y = 0; y < mapSize.y; y ++)
            {
                int i = x + y * Length;
                int tileType = levelData.tiles[i];

                // parse position for tile
                // Vector3 tilePosition = new Vector3(-mapSize.x/2 +nodeRadius + x + transform.position.x, 2, -mapSize.y/2 + nodeRadius + y + transform.position.z);
                Vector3 tilePosition = GetWorldPosition(x, y);
                
                Transform tileTransform = i < numExistedTiles ? root.GetChild(i) : Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90), root);

                // initiate outline
                tileTransform.localScale = Vector3.one * (1 - outlinePercent);

                // set tile value
                Tile tile = tileTransform.GetComponent<Tile>();
                tile.walkable = tileType == 0;
                tile.gridPosition = new Vector2Int(x, y);

                // insertion
                grid[x,y] = tile;

                switch (tileType)
                {
                    case 1:
                        Instantiate(wallPrefab, tilePosition, Quaternion.identity, environmentHolder);
                        break;
                }
            }
    }

    public void Initialize()
    {
        LevelManager.Instance.playerController.OnPathUpdate.AddListener(HandlePathChange);
    }

    public void wipeTiles()
    {
        if (checktimes > 0)
        {
            checktimes = 0;
            foreach (Tile tile in grid)
            {
                //reset all tiles
                tile.Wipe();
            }
        }
    }

    public void DehighlightAll()
    {
        if (checktimes > 0)
        {
            foreach (Tile tile in grid)
                tile.Dehighlight();

            checktimes = 0;
        }
    }

    private void HandlePathChange(Path<Tile> path)
    {
        DehighlightAll();

        if (path != null)
        {
            player _player = LevelManager.Instance.Player;

            if (path.Count == 0)
                Highlight(TileFromWorldPoint(_player.transform.position), _player.ActionPoint, Tile.HighlightColor.Blue, true);
            else
                Highlight(path.Destination, _player.ActionPoint - path.Count, Tile.HighlightColor.Blue, true);

            foreach (Tile wayPoint in path)
                Highlight(wayPoint, Tile.HighlightColor.Green);
        }
    }

    public void Highlight(Tile center, Tile.HighlightColor color)
    {
        Highlight(center, 0, 0, int.MinValue, color);
    }

    public void Highlight(Tile center, int range, Tile.HighlightColor color, bool skipUnmasked = false)
    {
        Highlight(center, 0, range, int.MinValue, color, skipUnmasked);
    }

    public void Highlight(Tile center, int range, int mask, Tile.HighlightColor color, bool skipUnmasked = false)
    {
        Highlight(center, 0, range, mask, color, skipUnmasked);
    }

    public void Highlight(Tile center, int lower, int upper, int mask, Tile.HighlightColor color, bool skipUnmasked = false)
    {
        bool[,] isVisited = new bool[Length, Width];

        Queue<KeyValuePair<Tile, int>> q = new Queue<KeyValuePair<Tile, int>>();

        q.Enqueue(new KeyValuePair<Tile, int>(center, 0));

        while (q.Count > 0)
        {
            KeyValuePair<Tile, int> pair = q.Dequeue();

            Tile tile = pair.Key;
            int distance = pair.Value;

            if (distance >= lower && distance <= upper && (skipUnmasked || (center.Mark & mask) != 0))
            {
                tile.Highlight(color);
                checktimes++;
            }

            int x = tile.x;
            int y = tile.y;

            isVisited[x, y] = true;

            if (++distance <= upper)
            {
                if (x + 1 < Length && !isVisited[x + 1, y])
                {
                    tile = grid[x + 1, y];

                    if (!skipUnmasked || (tile.Mark & mask) != 0)
                        q.Enqueue(new KeyValuePair<Tile, int>(tile, distance));
                    else
                        isVisited[x, y] = true;
                }

                if (x - 1 >= 0 && !isVisited[x - 1, y])
                {
                    tile = grid[x - 1, y];

                    if (!skipUnmasked || (tile.Mark & mask) != 0)
                        q.Enqueue(new KeyValuePair<Tile, int>(tile, distance));
                    else
                        isVisited[x, y] = true;
                }

                if (y + 1 < Width && !isVisited[x, y + 1])
                {
                    tile = grid[x, y + 1];

                    if (!skipUnmasked || (tile.Mark & mask) != 0)
                        q.Enqueue(new KeyValuePair<Tile, int>(tile, distance));
                    else
                        isVisited[x, y] = true;
                }

                if (y - 1 >= 0 && !isVisited[x, y - 1])
                {
                    tile = grid[x, y - 1];

                    if (!skipUnmasked || (tile.Mark & mask) != 0)
                        q.Enqueue(new KeyValuePair<Tile, int>(tile, distance));
                    else
                        isVisited[x, y] = true;
                }
            }
        }
    }

    public Vector2Int GetIndices(Tile position)
    {
        return position.gridPosition;
    }

    public Tile GetTile(Vector2Int indices)
    {
        return grid[indices.x, indices.y];
    }

    public bool IsAccessible(int x, int y)
    {
        return grid[x, y].walkable;
    }

    public List<Vector2Int> GetAdjacentIndices(int x, int y)
    {
        List<Vector2Int> list = new List<Vector2Int>();

        if (x + 1 < Length && grid[x + 1, y].walkable)
            list.Add(new Vector2Int(x + 1, y));

        if (x - 1 >= 0 && grid[x - 1, y].walkable)
            list.Add(new Vector2Int(x - 1, y));

        if (y + 1 < Width && grid[x, y + 1].walkable)
            list.Add(new Vector2Int(x, y + 1));

        if (y - 1 >= 0 && grid[x, y - 1].walkable)
            list.Add(new Vector2Int(x, y - 1));

        return list;
    }
}
