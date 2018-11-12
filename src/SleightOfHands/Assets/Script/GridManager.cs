using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

///	<summary/>
/// GridManager - Grid Manager class
/// tile highlight in range, store player designed path, generate path.
/// store grid[,] to manage all the tile in the map.
/// Usage: GridManager._instance.FUNCTION_NAME()
/// </summary>
public class GridManager : MonoBehaviour, INavGrid<Tile>
{
    // The unique instance
    public static GridManager Instance { get; private set; }

    /// <summary>
    /// An event type for GridManager.Instance.onUnitMove
    /// </summary>
    public class EventOnUnitMove : UnityEvent<Unit, Vector2Int, Vector2Int> {}

    /// <summary>
    /// An event triggered whenever a unit moves
    /// </summary>
    public EventOnUnitMove onUnitMove = new EventOnUnitMove();

    //public LayerMask unwalkableMask;
    [SerializeField] private float tileSize;
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private GameObject[] wallPrefabs;
    [SerializeField] private GameObject[] roadTilePrefabs;
    [SerializeField] private GameObject[] environmentTilePrefabs;
    [SerializeField] private TilesetElement[] tilesetElements;
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] [Range(0,1)] private float outlinePercent;

    //[SerializeField] private float time_intervals;

    //private float last_mouse_down;

    private Unit[,] units;
    private Tile[,] grid;

    //public bool dragging;
    //public bool ok_to_drag;
    //private HashSet<Tile>  recheck_list;
    //private int range;
    private int numHighlightedTiles = 0;

    //public Tile[] generatedPath;
    //private int action_point;

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

    public float TileSize
    {
        get
        {
            return tileSize;
        }
    }

    //private void Start()
    //{
    //    ok_to_drag = false;
    //    action_point = 5;
    //}

    //private void Update()
    //{
    //    if (Input.GetMouseButton(0)&&ok_to_drag)
    //    {
    //        dragging = true;
    //        last_mouse_down = Time.unscaledTime;
    //        //Debug.Log("hold!");
    //    }
    //   // print("time: "+ (Time.unscaledTime -last_mouse_down));
    //    if (Time.unscaledTime -last_mouse_down > time_intervals)
    //    {
    //        //Debug.Log("NOT hold!");
    //        dragging = false;
    //    }
    //}

    /// <summary>
    /// Get the tile on a grid position
    /// </summary>
    /// <param name="x"> The x value of the grid position to concern </param>
    /// <param name="y"> The y value of the grid position to concern </param>
    /// <returns> A Tile object representing the tile </returns>
    public Tile GetTile(int x, int y)
    {
        return grid[x, y];
    }

    /// <summary>
    /// Get the tile on a grid position
    /// </summary>
    /// <param name="gridPosition"> The grid position to concern </param>
    /// <returns> A Tile instance representing the tile </returns>
    public Tile GetTile(Vector2Int gridPosition)
    {
        return GetTile(gridPosition.x, gridPosition.y);
    }

    /// <summary>
    /// Get the tile accordant to a world position
    /// </summary>
    /// <param name="worldPosition"> The world position to concern </param>
    /// <returns> A Tile instance representing the tile </returns>
    public Tile GetTile(Vector3 worldPosition)
    {
        return grid[Mathf.FloorToInt(worldPosition.x / (2 * tileSize)), Mathf.FloorToInt(worldPosition.z / (2 * tileSize))];
    }

    /// <summary>
    /// Get the grid position of a tile
    /// </summary>
    /// <param name="tile"> The tile to concern </param>
    /// <returns> A Vector2Int struct representing the grid position </returns>
    public Vector2Int GetGridPosition(Tile tile)
    {
        return tile.gridPosition;
    }

    /// <summary>
    /// Get the world position from a grid position
    /// </summary>
    /// <param name="x"> The x value of the grid position to concern </param>
    /// <param name="y"> The y value of the grid position to concern </param>
    /// <returns> A Vector3 struct representing the world position </returns>
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3((x * 2 + 1) * tileSize, 0, (y * 2 + 1) * tileSize);
    }

    /// <summary>
    /// Get the world position from a grid position
    /// </summary>
    /// <param name="gridPosition"> The grid position to concern </param>
    /// <returns> A Vector3 struct representing the world position </returns>
    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        return GetWorldPosition(gridPosition.x, gridPosition.y);
    }

    /// <summary>
    /// Get the world position of the tile
    /// </summary>
    /// <param name="tile"> The tile to concern </param>
    /// <returns> A Vector3 struct representing the world position </returns>
    public Vector3 GetWorldPosition(Tile tile)
    {
        return GetWorldPosition(tile.gridPosition);
    }

    public Unit GetUnit(int x, int y)
    {
        return units[x, y];
    }

    public Unit GetUnit(Vector2Int gridPosition)
    {
        return GetUnit(gridPosition.x, gridPosition.y);
    }

    public Unit GetUnit(Tile tile)
    {
        return GetUnit(tile.gridPosition);
    }

    /// <summary>
    /// Evalueate whether two tiles are considered to be adjacent
    /// </summary>
    /// <param name="A"> The first tile to concern </param>
    /// <param name="B"> The second tile to concern </param>
    /// <returns> A boolean indicating the result of evaluation </returns>
    public bool IsAdjacent(Tile A, Tile B)
    {
        return MathUtility.ManhattanDistance(A.x, A.y, B.x, B.y) == 1;
    }

    /// <summary>
    /// Evaluate whether the grid position is accessible
    /// </summary>
    /// <param name="x"> The x value of the grid position to concern </param>
    /// <param name="y"> The y value of the grid position to concern </param>
    /// <returns> A boolean indicating the result of evaluation </returns>
    public bool IsAccessible(int x, int y)
    {
        return grid[x, y].walkable;
    }

    public bool IsWalkable(int x, int y)
    {
        return grid[x, y].walkable;
    }
    
    public bool HasUnitOn(int x, int y)
    {
        return units[x, y]!=null;
    }

    /// <summary>
    /// Get all accessible positions surrounding a given grid position
    /// </summary>
    /// <param name="x"> The x value of the grid position to concern </param>
    /// <param name="y"> The y value of the grid position to concern </param>
    /// <returns> A List of Vector2Int containing all grid positions </returns>
    public List<Vector2Int> GetAdjacentGridPositions(int x, int y)
    {
        List<Vector2Int> list = new List<Vector2Int>();

        if (x + 1 < Length && grid[x + 1, y])
            list.Add(new Vector2Int(x + 1, y));

        if (x - 1 >= 0 && grid[x - 1, y])
            list.Add(new Vector2Int(x - 1, y));

        if (y + 1 < Width && grid[x, y + 1])
            list.Add(new Vector2Int(x, y + 1));

        if (y - 1 >= 0 && grid[x, y - 1])
            list.Add(new Vector2Int(x, y - 1));

        return list;
    }

    /// <summary>
    /// generated grid map
    /// map_size : the number of cube in row and column
    /// node_radius: the cube radius
    /// parameter: None
    /// </summary>
    public void GenerateMap(LevelManager.LevelData levelData)
    {
        if (!root)
            root = transform.Find("GridRoot");

        if (!root)
        {
            root = new GameObject("GridRoot").transform;
            root.parent = transform;
        }

        // Setup tilesets
        Dictionary<int, TilesetElement> tElements = new Dictionary<int, TilesetElement>();
        foreach (TilesetElement te in tilesetElements) {
            tElements.Add(te.id, te);
        }

        // Extract data from levelData
        if (levelData != null)
            mapSize = new Vector2Int(levelData.width, Mathf.CeilToInt(levelData.tiles.Length / levelData.width));

        // new grid with size [mapSize.x,mapSize.y]
        grid = new Tile[mapSize.x, mapSize.y];
        units = new Unit[mapSize.x, mapSize.y];

        int numExistedTiles = root.childCount;

        for (int x = 0; x < mapSize.x; x ++)
            for (int y = 0; y < mapSize.y; y ++)
            {
                int i = x + y * levelData.tiles.Length;
                int tileType = levelData.GetTile(x, y);
                TilesetElement te;
                tElements.TryGetValue(tileType, out te);

                // parse position for tile
                Vector3 tilePosition = GetWorldPosition(x, y);

                Transform tileTransform = i < numExistedTiles ? root.GetChild(i) : Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90), root);

                // initiate outline
                tileTransform.localScale = Vector3.one * (1 - outlinePercent);

                // set tile value
                Tile tile = tileTransform.GetComponent<Tile>();
                tile.walkable = (te != null ? te.walkable : true);
                tile.gridPosition = new Vector2Int(x, y);
                
                // insertion
                grid[x,y] = tile;

                // Tile display
                if (te != null) {
                    
                    Tileset.TileCollection tileCollection = null;
                    float envTileRotation;
                    GetEnvTileType(levelData, x, y, te.tileset, out tileCollection, out envTileRotation);
                    if (tileCollection.objects.Length == 0) tileCollection = te.tileset.fallback;

                    GameObject envTilePrefab = tileCollection.GetRandom();
                    if (envTilePrefab == null) {
                        envTilePrefab = te.tileset.fallback.GetRandom();
                    }
                    Vector3 envTilePosition = tilePosition;
                    envTilePosition.y = -0.55f;

                    Instantiate(envTilePrefab, envTilePosition, Quaternion.Euler(0, envTileRotation, 0));

                }
                
            }
    }

    private void GetEnvTileType(LevelManager.LevelData levelData, int x, int y, Tileset tileset, out Tileset.TileCollection tileCollection, out float rotation) {

        tileCollection = tileset.fallback;
        rotation = 0;

        // True = same :: False = not same
        bool up = false;
        bool down = false;
        bool left = false;
        bool right = false;

        Vector2Int size = levelData.GetSize();
        int value = levelData.GetTile(x, y);

        if (y > 0 && levelData.GetTile(x, y - 1) == value) {
            up = true;
        }

        if (y < mapSize.y - 1 && levelData.GetTile(x, y + 1) == value) {
            down = true;
        }

        if (x > 0 && levelData.GetTile(x - 1, y) == value) {
            left = true;
        }

        if (x < mapSize.x - 1 && levelData.GetTile(x + 1, y) == value) {
            right = true;
        }

        bool upleft = false;
        bool upright = false;
        bool downleft = false;
        bool downright = false;

        if (up && left && levelData.GetTile(x - 1, y - 1) == value) {
            upleft = true;
        }

        if (up && right && levelData.GetTile(x + 1,y - 1) == value) {
            upright = true;
        }

        if (down && left && levelData.GetTile(x - 1, y + 1) == value) {
            downleft = true;
        }

        if (down && right && levelData.GetTile(x + 1, y + 1) == value) {
            downright = true;
        }

        if (up && down && left && right) {
            tileCollection = tileset.surrounded;
            rotation = 0;
            if (!upleft) {
                tileCollection = tileset.surrounded1;
                rotation = 180;
            } else if (!upright) {
                tileCollection = tileset.surrounded1;
                rotation = 90;
            } else if (!downright) {
                tileCollection = tileset.surrounded1;
                rotation = 0;
            } else if (!downleft) {
                tileCollection = tileset.surrounded1;
                rotation = 270;
            }
            if (tileCollection.objects.Length == 0) {
                tileCollection = tileset.surrounded;
                rotation = 0;
            }
        } else if (left && up && right) {
            if (upleft && upright) {
                tileCollection = tileset.tFill;
            } else if (upleft) {
                tileCollection = tileset.tFillRight;
            } else if (upright) {
                tileCollection = tileset.tFillLeft;
            } else {
                tileCollection = tileset.t;
            }
            rotation = 0;
        } else if (up && right && down) {
            if (upright && downright) {
                tileCollection = tileset.tFill;
            } else if (upright) {
                tileCollection = tileset.tFillRight;
            } else if (downright) {
                tileCollection = tileset.tFillLeft;
            } else {
                tileCollection = tileset.t;
            }
            rotation = 270;
        } else if (right && down && left) {
            if (downright && downleft) {
                tileCollection = tileset.tFill;
            } else if (downright) {
                tileCollection = tileset.tFillRight;
            } else if (downleft) {
                tileCollection = tileset.tFillLeft;
            } else {
                tileCollection = tileset.t;
            }
            rotation = 180;
        } else if (down && left && up) {
            if (downleft && upleft) {
                tileCollection = tileset.tFill;
            } else if (downleft) {
                tileCollection = tileset.tFillRight;
            } else if (upleft) {
                tileCollection = tileset.tFillLeft;
            } else {
                tileCollection = tileset.t;
            }
            rotation = 90;
        } else if (up && down) {
            tileCollection = tileset.straight;
            rotation = 90;
        } else if (left && right) {
            tileCollection = tileset.straight;
            rotation = 0;
        } else if (up && right) {
            if (upright) {
                tileCollection = tileset.cornerFill;
            } else {
                tileCollection = tileset.corner;
            }
            rotation = 0;
        } else if (right && down) {
            if (downright) {
                tileCollection = tileset.cornerFill;
            } else {
                tileCollection = tileset.corner;
            }
            rotation = 270;
        } else if (down && left) {
            if (downleft) {
                tileCollection = tileset.cornerFill;
            } else {
                tileCollection = tileset.corner;
            }
            rotation = 180;
        } else if (left && up) {
            if (upleft) {
                tileCollection = tileset.cornerFill;
            } else {
                tileCollection = tileset.corner;
            }
            rotation = 90;
        } else if (up) {
            tileCollection = tileset.peninsula;
            rotation = 90;
        } else if (right) {
            tileCollection = tileset.peninsula;
            rotation = 180;
        } else if (down) {
            tileCollection = tileset.peninsula;
            rotation = 270;
        } else if (left) {
            tileCollection = tileset.peninsula;
            rotation = 0;
        }

        // Fallback
        if (tileCollection == null) {
            tileCollection = tileset.fallback;
            rotation = 0;
        }

    }

    /// <summary>
    /// Initialize the manager by starting to listen to important events
    /// </summary>
    public void Initialize()
    {
        PlayerController playerController = LevelManager.Instance.playerController;

        playerController.onPathUpdate.AddListener(HandlePathChange);
        playerController.onCurrentPlayerStateChange.AddListener(HandleCurrentPlayerStateChange);

        playerController.onCardToUseUpdate.AddListener(HandleCardToUseChange);
    }

    //public void wipeTiles()
    //{
    //    if (numHighlightedTiles > 0)
    //    {
    //        numHighlightedTiles = 0;
    //        foreach (Tile tile in grid)
    //        {
    //            //reset all tiles
    //            tile.Wipe();
    //        }
    //    }
    //}

    /// <summary>
    /// High light the tile with a given color
    /// </summary>
    /// <param name="tile"> The tile to highlight </param>
    /// <param name="color"> The color to highlight with </param>
    public void Highlight(Tile tile, Tile.HighlightColor color, bool isAdditive = true)
    {
        Highlight(tile, 0, 0, int.MinValue, color, isAdditive);
    }

    /// <summary>
    /// High light tiles within a certain range of a central tile with a given color
    /// </summary>
    /// <param name="center"> The central tile to concern </param>
    /// <param name="range"> The range to concern </param>
    /// <param name="color"> The color to highlight with </param>
    /// <param name="skipUnmasked"> [optional] </param>
    public void Highlight(Tile center, int range, Tile.HighlightColor color, bool isAdditive = true, bool skipUnmasked = false)
    {
        Highlight(center, 0, range, int.MinValue, color, isAdditive, skipUnmasked);
    }

    /// <summary>
    /// High light tiles filtered by a certain mask within a certain range of a central tile with a given color
    /// </summary>
    /// <param name="center"> The central tile to concern </param>
    /// <param name="range"> The range to concern </param>
    /// <param name="mask"> The mask for filtering </param>
    /// <param name="color"> The color to highlight with </param>
    /// <param name="skipUnmasked"> [optional] </param>
    public void Highlight(Tile center, int range, int mask, Tile.HighlightColor color, bool isAdditive = true, bool skipUnmasked = false)
    {
        Highlight(center, 0, range, mask, color, isAdditive, skipUnmasked);
    }

    /// <summary>
    /// High light tiles filtered by a certain mask within a certain interval of distance to a central tile with a given color
    /// </summary>
    /// <param name="center"> The central tile to concern </param>
    /// <param name="lower"> The lower boundary of the distance </param>
    /// <param name="upper"> The higher boundary of the distance </param>
    /// <param name="mask"> The mask for filtering </param>
    /// <param name="color"> The color to highlight with </param>
    /// <param name="skipUnmasked"> [optional] </param>
    public void Highlight(Tile center, int lower, int upper, int mask, Tile.HighlightColor color, bool isAdditive = true, bool skipUnmasked = false)
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
                tile.Highlight(color, isAdditive);
                numHighlightedTiles++;
            }

            int x = tile.x;
            int y = tile.y;

            isVisited[x, y] = true;

            if (++distance <= upper)
                //{
                foreach (Vector2Int coordinate in GetAdjacentGridPositions(x, y))
                {
                    int xi = coordinate.x;
                    int yi = coordinate.y;

                    if (!isVisited[xi, yi])
                    {
                        tile = grid[xi, yi];

                        if (!skipUnmasked || (tile.Mark & mask) != 0)
                            q.Enqueue(new KeyValuePair<Tile, int>(tile, distance));
                        else
                            isVisited[xi, yi] = true;
                    }
                }

            //    if (x + 1 < Length && !isVisited[x + 1, y])
            //    {
            //        tile = grid[x + 1, y];

            //        if (!skipUnmasked || (tile.Mark & mask) != 0)
            //            q.Enqueue(new KeyValuePair<Tile, int>(tile, distance));
            //        else
            //            isVisited[x, y] = true;
            //    }

            //    if (x - 1 >= 0 && !isVisited[x - 1, y])
            //    {
            //        tile = grid[x - 1, y];

            //        if (!skipUnmasked || (tile.Mark & mask) != 0)
            //            q.Enqueue(new KeyValuePair<Tile, int>(tile, distance));
            //        else
            //            isVisited[x, y] = true;
            //    }

            //    if (y + 1 < Width && !isVisited[x, y + 1])
            //    {
            //        tile = grid[x, y + 1];

            //        if (!skipUnmasked || (tile.Mark & mask) != 0)
            //            q.Enqueue(new KeyValuePair<Tile, int>(tile, distance));
            //        else
            //            isVisited[x, y] = true;
            //    }

            //    if (y - 1 >= 0 && !isVisited[x, y - 1])
            //    {
            //        tile = grid[x, y - 1];

            //        if (!skipUnmasked || (tile.Mark & mask) != 0)
            //            q.Enqueue(new KeyValuePair<Tile, int>(tile, distance));
            //        else
            //            isVisited[x, y] = true;
            //    }
            //}
        }
    }

    /// <summary>
    /// Dehighlight all tiles
    /// </summary>
    public void DehighlightAll()
    {
        if (numHighlightedTiles > 0)
        {
            foreach (Tile tile in grid)
                if (tile)
                    tile.Dehighlight();

            numHighlightedTiles = 0;
        }
    }

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void HandleCurrentPlayerStateChange(PlayerState previousState, PlayerState currentState)
    {
        switch (currentState)
        {
            case PlayerState.MovementConfirmation:
                foreach (Tile tile in grid)
                    if (tile && tile.IsHighlighted(Tile.HighlightColor.Blue))
                    {
                        tile.Dehighlight();
                        numHighlightedTiles++;
                    }
                break;
        }
    }

    private void HandlePathChange(Path<Tile> path)
    {
        DehighlightAll();

        if (path != null)
        {
            player _player = LevelManager.Instance.Player;


            if (path.Count == 0)
                Highlight(GetTile(_player.transform.position), _player.Ap, Tile.HighlightColor.Blue, true, true);
            else
                Highlight(path.Destination, _player.Ap - path.Count, Tile.HighlightColor.Blue, true, true);

            foreach (Tile wayPoint in path)
                Highlight(wayPoint, Tile.HighlightColor.Green, false);
        }
    }

    private void HandleCardToUseChange(Card cardToUse)
    {
        // TODO: Change highlights according to card's range
        if (cardToUse != null)
            Highlight(GetTile(LevelManager.Instance.Player.transform.position), Tile.HighlightColor.Green, false);
        else
            DehighlightAll();
    }

    internal void NotifyUnitPositionChange(Unit unit, Vector2Int previousGridPosition, Vector2Int currentGridPosition)
    {
        if (previousGridPosition.x >= 0)
            units[previousGridPosition.x, previousGridPosition.y] = null;

        units[currentGridPosition.x, currentGridPosition.y] = unit;

        onUnitMove.Invoke(unit, previousGridPosition, currentGridPosition);
    }

    /// <summary>
    /// Find the cloest tile to target destination in a certain range, use for AI moving in Detected mode
    /// </summary>
    private void FindCloestTileToDes(Tile start, int range)
    {

    }

    [Serializable]
    public class TilesetElement {
        public string name;
        public int id;
        public bool walkable;
        public Tileset tileset;
    }

}
