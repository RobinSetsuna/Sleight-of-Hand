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

    private int[,] detection;

    private int[,] detectionHighlights;
    private HashSet<int> highlightedDetectionAreas = new HashSet<int>();

    private HashSet<Tile>[] detectionAreas = new HashSet<Tile>[32];

    //public bool dragging;
    //public bool ok_to_drag;
    //private HashSet<Tile>  recheck_list;
    //private int range;
    private int numHighlightedTiles = 0;

    //public Tile[] generatedPath;
    //private int action_point;

    private Transform gridRoot;

    [Header("References")]
    [SerializeField] private Transform environmentRoot;
    public Transform EnvironmentRoot
    {
        get
        {
            return environmentRoot;
        }
    }

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
    /// Get the grid position of a tile
    /// </summary>
    /// <param name="worldPosition"> The world position to concern </param>
    /// <returns> A Vector2Int struct representing the grid position </returns>
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        return GetTile(worldPosition).gridPosition;
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
    public bool IsAccessible(Unit accessor, int x, int y)
    {
        Unit unit = units[x, y];
        return grid[x, y].walkable && (unit == null || unit == accessor);
    }

    public bool IsAccessible(Unit accessor, Tile tile)
    {
        Unit unit = units[tile.x, tile.y];
        return tile.walkable && (unit == null || unit == accessor);
    }

    public bool IsWalkable(int x, int y)
    {
        return IsWalkable(grid[x, y]);
    }

    public bool IsWalkable(Tile tile)
    {
        return tile.walkable;
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
        if (!gridRoot)
            gridRoot = transform.Find("GridRoot");

        if (!gridRoot)
        {
            gridRoot = new GameObject("GridRoot").transform;
            gridRoot.parent = transform;
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

        detection = new int[mapSize.x, mapSize.y];
        detectionHighlights = new int[mapSize.x, mapSize.y];

        int numExistedTiles = gridRoot.childCount;

        for (int x = 0; x < mapSize.x; x ++)
            for (int y = 0; y < mapSize.y; y ++)
            {
                int i = x + y * levelData.tiles.Length;
                int tileType = levelData.GetTile(x, y);
                TilesetElement te;
                tElements.TryGetValue(tileType, out te);

                // parse position for tile
                Vector3 tilePosition = GetWorldPosition(x, y);

                Transform tileTransform = i < numExistedTiles ? gridRoot.GetChild(i) : Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90), gridRoot);

                // initiate outline
                tileTransform.localScale = Vector3.one * (1 - outlinePercent);

                // set tile value
                Tile tile = tileTransform.GetComponent<Tile>();
                tile.walkable = (te != null ? te.walkable : true);
                tile.gridPosition = new Vector2Int(x, y);
                
                // insertion
                grid[x,y] = tile;

                // Tile display
                if (te != null)
                {
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

                    Spawn(envTilePrefab, envTilePosition, Quaternion.Euler(0, envTileRotation, 0));
                }
            }

        for (int i = 0; i < levelData.endingPoints.Length; i += 2)
            Spawn(ResourceUtility.GetPrefab("EndingPoint"), GetWorldPosition(levelData.endingPoints[i], levelData.endingPoints[i + 1]) + new Vector3(0, TileSize * 0.9f, 0), Quaternion.identity);
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

    public T Spawn<T>(T obj, Tile tile) where T : UnityEngine.Object
    {
        return Spawn(obj, GetWorldPosition(tile) + new Vector3(0,0.5f,0), Quaternion.identity);
    }

    public T Spawn<T>(T obj, Vector3 position, Quaternion rotation) where T : UnityEngine.Object
    {
        return Instantiate(obj, position, rotation, EnvironmentRoot);
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

    public void Highlight(Tile tile, Tile.HighlightColor color, bool isAdditive = true)
    {
        tile.Highlight(color, isAdditive);
        numHighlightedTiles++;
    }

    /// <summary>
    /// High light tiles within a certain range of a central tile with a given color
    /// </summary>
    /// <param name="center"> The central tile to concern </param>
    /// <param name="range"> The range to concern </param>
    /// <param name="color"> The color to highlight with </param>
    /// <param name="skipUnmatched"> [optional] </param>
    public void Highlight(Tile center, int range, Tile.HighlightColor color, bool isAdditive = true, bool skipUnmatched = false)
    {
        Highlight(center, 0, range, tile => true, color, isAdditive, skipUnmatched);
    }

    public void Highlight(Tile center, int range, Tile.HighlightColor color, bool isAdditive = true, int skipUnmatchedAfter = int.MaxValue)
    {
        Highlight(center, 0, range, tile => true, color, isAdditive, skipUnmatchedAfter);
    }

    /// <summary>
    /// High light tiles filtered by a certain mask within a certain range of a central tile with a given color
    /// </summary>
    /// <param name="center"> The central tile to concern </param>
    /// <param name="range"> The range to concern </param>
    /// <param name="mask"> The mask for filtering </param>
    /// <param name="color"> The color to highlight with </param>
    /// <param name="skipUnmatched"> [optional] </param>
    public void Highlight(Tile center, int range, Predicate<Tile> Match, Tile.HighlightColor color, bool isAdditive = true, bool skipUnmatched = false)
    {
        Highlight(center, 0, range, Match, color, isAdditive, skipUnmatched);
    }

    public void Highlight(Tile center, int range, Predicate<Tile> Match, Tile.HighlightColor color, bool isAdditive = true, int skipUnmatchedAfter = int.MaxValue)
    {
        Highlight(center, 0, range, Match, color, isAdditive, skipUnmatchedAfter);
    }

    /// <summary>
    /// High light tiles filtered by a certain mask within a certain interval of distance to a central tile with a given color
    /// </summary>
    /// <param name="center"> The central tile to concern </param>
    /// <param name="lower"> The lower boundary of the distance </param>
    /// <param name="upper"> The higher boundary of the distance </param>
    /// <param name="mask"> The mask for filtering </param>
    /// <param name="color"> The color to highlight with </param>
    /// <param name="skipUnmatched"> [optional] </param>
    public void Highlight(Tile center, int lower, int upper, Predicate<Tile> Match, Tile.HighlightColor color, bool isAdditive = true, bool skipUnmatched = false)
    {
        Highlight(center, lower, upper, Match, color, isAdditive, skipUnmatched ? 0 : int.MaxValue);
    }

    public void Highlight(Tile center, int lower, int upper, Predicate<Tile> Match, Tile.HighlightColor color, bool isAdditive = true, int skipUnmatchedAfter = int.MaxValue)
    {
        foreach (Tile tile in BreadthFirstSearch(center, lower, upper, Match, skipUnmatchedAfter))
            Highlight(tile, color, isAdditive);
    }

    public List<Tile> BreadthFirstSearch(Tile center, int range)
    {
        return BreadthFirstSearch(center, 0, range);
    }

    public List<Tile> BreadthFirstSearch(Tile center, int lowerRange, int upperRange)
    {
        return BreadthFirstSearch(center, lowerRange, upperRange, tile => true, false);
    }

    public List<Tile> BreadthFirstSearch(Tile center, int lowerRange, int upperRange, Predicate<Tile> Match, bool skipUnmatched = false)
    {
        return BreadthFirstSearch(center, lowerRange, upperRange, Match, skipUnmatched ? 0 : int.MaxValue);
    }

    public List<Tile> BreadthFirstSearch(Tile center, int lowerRange, int upperRange, Predicate<Tile> Match, int skipUnmatchedAfter = int.MaxValue)
    {
        List<Tile> list = new List<Tile>();

        bool neverSkipUnmatched = skipUnmatchedAfter >= upperRange;
        bool canMarkVisitedBeforehead = skipUnmatchedAfter == 0 || neverSkipUnmatched;

        bool[,] isVisited = new bool[Length, Width];

        Queue<KeyValuePair<Tile, int[]>> q = new Queue<KeyValuePair<Tile, int[]>>();
        q.Enqueue(new KeyValuePair<Tile, int[]>(center, new int[2] { 0, 0 }));

        while (q.Count > 0)
        {
            KeyValuePair<Tile, int[]> pair = q.Dequeue();

            Tile tile = pair.Key;
            int[] values = pair.Value;

            int distance = values[0];
            int numUnmatched = values[1];

            if (distance >= lowerRange && (Match(tile) || !neverSkipUnmatched))
                list.Add(tile);

            int x = tile.x;
            int y = tile.y;

            isVisited[x, y] = true;

            if (++distance <= upperRange)
                foreach (Vector2Int coordinate in GetAdjacentGridPositions(x, y))
                {
                    int xi = coordinate.x;
                    int yi = coordinate.y;

                    if (!isVisited[xi, yi])
                    {
                        tile = grid[xi, yi];

                        if (Match(tile))
                            q.Enqueue(new KeyValuePair<Tile, int[]>(tile, new int[2] { distance, numUnmatched }));
                        else if (numUnmatched < skipUnmatchedAfter)
                            q.Enqueue(new KeyValuePair<Tile, int[]>(tile, new int[2] { distance, numUnmatched + 1 }));
                        else if (canMarkVisitedBeforehead)
                            isVisited[xi, yi] = true;
                    }
                }
        }

        return list;
    }

    public void ToggleDetectionArea(int uid)
    {
        if (highlightedDetectionAreas.Contains(uid))
        {
            foreach (Tile tile in detectionAreas[uid])
            {
                BitOperationUtility.WriteBit(ref detectionHighlights[tile.x, tile.y], uid, 0);

                if (detectionHighlights[tile.x, tile.y] == 0)
                    tile.Dehighlight(Tile.HighlightColor.Red);
            }

            highlightedDetectionAreas.Remove(uid);
        }
        else
        {
            foreach (Tile tile in detectionAreas[uid])
            {
                if (detectionHighlights[tile.x, tile.y] == 0)
                    tile.Highlight(Tile.HighlightColor.Red);

                BitOperationUtility.WriteBit(ref detectionHighlights[tile.x, tile.y], uid, 1);
            }

            highlightedDetectionAreas.Add(uid);
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
                    tile.DehighlightAll();

            for (int x = 0; x < mapSize.x; x++)
                for (int y = 0; y < mapSize.y; y++)
                    detectionHighlights[x, y] = 0;

            highlightedDetectionAreas.Clear();

            numHighlightedTiles = 0;
        }
    }

    internal void NotifyUnitPositionChange(Unit unit, Vector2Int previousGridPosition, Vector2Int currentGridPosition)
    {
        bool isNotInitialization = previousGridPosition.x >= 0;

        if (isNotInitialization)
            units[previousGridPosition.x, previousGridPosition.y] = null;

        units[currentGridPosition.x, currentGridPosition.y] = unit;

        if (unit.GetComponent<player>())
        {
            int d = detection[currentGridPosition.x, currentGridPosition.y];

            if (d != 0)
                foreach (int uid in BitOperationUtility.GetIndicesOfOne(d))
                    LevelManager.Instance.Enemies[uid].GetComponent<EnemyController>().Mode = EnemyMode.Chasing;
        }
        else if (unit.GetComponent<Enemy>())
        {
            int uid = unit.GetComponent<EnemyController>().UID;

            HashSet<Tile> currentDetectionArea = ProjectileManager.Instance.getProjectileRange(GetTile(currentGridPosition), unit.GetComponent<Enemy>().DetectionRange, true, unit.transform.rotation.eulerAngles.y);

            if (isNotInitialization)
            {
                foreach (Tile tile in detectionAreas[uid])
                    if (!currentDetectionArea.Contains(tile))
                        RemoveDetection(tile, uid);
            }

            foreach (Tile tile in currentDetectionArea)
                AddDetection(tile, uid);

            detectionAreas[uid] = currentDetectionArea;
        }

        onUnitMove.Invoke(unit, previousGridPosition, currentGridPosition);
    }

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }


    private void AddDetection(Tile tile, int uid)
    {
        if (LevelManager.Instance.Player.GridPosition == tile.gridPosition)
            LevelManager.Instance.Enemies[uid].GetComponent<EnemyController>().Mode = EnemyMode.Chasing;

        BitOperationUtility.WriteBit(ref detection[tile.x, tile.y], uid, 1);
    }

    private void RemoveDetection(Tile tile, int uid)
    {
        BitOperationUtility.WriteBit(ref detection[tile.x, tile.y], uid, 0);
    }

    private void HandleCurrentPlayerStateChange(PlayerState previousState, PlayerState currentState)
    {
        switch (currentState)
        {
            case PlayerState.MovementConfirmation:
                foreach (Tile tile in grid)
                    if (tile && tile.IsHighlighted(Tile.HighlightColor.Blue))
                    {
                        tile.Dehighlight(Tile.HighlightColor.Blue);
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
            player Player = LevelManager.Instance.Player;

            if (path.Count == 0)
                Highlight(GetTile(Player.transform.position), Player.Ap, Player.IsAccessibleTo, Tile.HighlightColor.Blue, true, true);
            else
                Highlight(path.Destination, Player.Ap - path.Count, Player.IsAccessibleTo, Tile.HighlightColor.Blue, true, true);

            foreach (Tile wayPoint in path)
                Highlight(wayPoint, Tile.HighlightColor.Green, false);
        }
    }

    private void HandleCardToUseChange(Card cardToUse)
    {
        if (cardToUse != null)
            Highlight(GetTile(LevelManager.Instance.Player.transform.position), cardToUse.Data.Range, IsWalkable, Tile.HighlightColor.Green, false, false);
        else
            DehighlightAll();
    }

    [Serializable]
    public class TilesetElement {
        public string name;
        public int id;
        public bool walkable;
        public Tileset tileset;
    }
}
