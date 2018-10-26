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

    [SerializeField] private player _player;
    public player Player
    {
        get
        {
            return _player;
        }
    }

    public LayerMask unwalkableMask;
    public float nodeRadius;
    public Transform tilePrefab;
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
        GenerateMap (null);
        checktimes = 0;
        ok_to_drag = false;
        action_point = 5;

        GameManager.Singleton.OnPathChange.AddListener(HandlePathChange);
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

    //public Tile TileFromWorldPoint(Vector3 worldPosition)
    //{
    //    Tile navigate_tile = grid[0, 0];

    //    // take the position of grid[0,0] to check the distance of grid shuffled 
    //    float percentX = (worldPosition.x - navigate_tile.transform.position.x) / (mapSize.x * nodeRadius * 2);
    //    float percentY = (worldPosition.z - navigate_tile.transform.position.z) / (mapSize.y  * nodeRadius * 2);

    //    percentX = Mathf.Clamp01(percentX);
    //    percentY = Mathf.Clamp01(percentY);
    //    int x = Mathf.RoundToInt((mapSize.x - 1) * percentX);
    //    int y = Mathf.RoundToInt((mapSize.y - 1) * percentY);
    //    //Debug.Log("x: "+ x + " y：" + y);
    //    return grid[x, y];
    //}

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
    public void GenerateMap(LevelManager.LevelData levelData) {

        string holderName = "Generated Map";
        // check for exist map, and destroy it
        if (transform.Find(holderName)) {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        
        // set Script holder as parent
        Transform mapHolder = new GameObject (holderName).transform;
        mapHolder.parent = transform;

        // Extract data from levelData
        if (levelData != null) {
            Debug.Log(levelData.tiles.Length);
            mapSize = new Vector2Int(levelData.width, Mathf.CeilToInt(levelData.tiles.Length / levelData.width));
        }
        //  start,code for temp usage , delete after level data implemented
        else
        {
            mapSize = new Vector2Int(20,20);
        }
        // end
        
        
        // new grid with size [mapSize.x,maoSize.y]
        grid = new Tile[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x ++) {
            for (int y = 0; y < mapSize.y; y ++) {
                // parse position for tile
                // Vector3 tilePosition = new Vector3(-mapSize.x/2 +nodeRadius + x + transform.position.x, 2, -mapSize.y/2 + nodeRadius + y + transform.position.z);
                Vector3 tilePosition = GetWorldPosition(x, y);

                //walkable collision check
                bool walkable = !(Physics.CheckSphere(tilePosition,nodeRadius, unwalkableMask));

                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90), mapHolder);

                // initiate outline
                newTile.localScale = Vector3.one * (1 - outlinePercent);

                // set tile value
                var temp = newTile.GetComponent<Tile>();
                temp.walkable = walkable;

                // insertion
                temp.gridPosition = new Vector2Int(x, y);
                grid[x,y] = temp;
            }
        }
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

    //public bool AccessibleCheck(int tile_x, int tile_y)
    //{
    //    for (int x = -1; x <= 1; x++)
    //    {
    //        for (int y = -1; y <= 1; y++)
    //        {
    //            if (x == 0 && y == 0)
    //            {
    //                // center skip
    //                continue;
    //            }

    //            if (x != 0 && y != 0)
    //            {
    //                // disable diagonal check.  remove this 'if' statement when we want diagonal check
    //                continue;
    //            }

    //            int temp_x = tile_x + x;
    //            int temp_y = tile_y + y;
    //            if (temp_x >= 0 && temp_x < grid.Length && temp_y >= 0 && temp_y < grid.Length)
    //            {
    //                if (grid[temp_x, temp_y].selected && temp_x == generatedPath[checktimes-1].x && temp_y == generatedPath[checktimes-1].y )
    //                {
    //                    //accessible
    //                    generatedPath[checktimes] = grid[tile_x, tile_y];// add tile to path, selected
    //                    checktimes++; // check success time = selected tiles.
    //                    return true;
    //                }
    //            }
    //        }
    //    }
    //    return false;
    //}

    /// <Hightlight>
    /// give center tile, highlight range, and highlight type to highlight specific group of tiles.
    /// parameter: (Tile(tile),Range(int),flag(int))
    /// 
    /// flag: 1 obstructed by the wall. ignore all tiles behind the wall. example usage: effect can not across the wall
    /// flag: 2 ignore distance, just avoid unwalkable tile , example usage: effect can across the wall
    /// flag: 3 consider distance, not ignore the tiles behind wall. example usage:movement use
    /// </summary>
    //public void Highlight(Tile center, int range, int flag)
    //{
    //    generatedPath = flag == 3 ? new Tile[action_point+1] : null;
    //    generatedPath[checktimes] = center;// add stand_tile to path, selected
    //    checktimes++;
    //    bool ignoreDistance = (flag == 2);
    //    bool rechecking = (flag == 3||flag == 2);
    //    this.range = range;
    //    center.HighlightTile();
    //    center.distance = 0;
    //    recheck_list = new HashSet<Tile>();
    //    for (int x = 0; x <= this.range; x++)
    //    {
    //        for (int y = 0; y <= this.range; y++)
    //        {
    //            if (x + y <= this.range && x + y != 0)
    //            {
    //                // tile is out of range or already highlight
    //                //Debug.Log("x:" + x +" y: "+ y + "is out of range");
    //                setHighlight(center.x + x, center.y + y, rechecking, ignoreDistance);
    //                setHighlight(center.x - x, center.y + y, rechecking, ignoreDistance);
    //                setHighlight(center.x + x, center.y - y, rechecking, ignoreDistance);
    //                setHighlight(center.x - x, center.y - y, rechecking, ignoreDistance);
    //            }        
    //        }
    //    }

    //    foreach (Tile tile in recheck_list)
    //    {
    //        //print("rechecking: "+ tile.x + " "+ tile.y);
    //        setHighlight(tile.x, tile.y, false, false);
    //    }
    //}

    private void HandlePathChange(Path<Tile> path)
    {
        DehighlightAll();

        if (path != null)
        {
            if (path.Count == 0)
                Highlight(TileFromWorldPoint(_player.transform.position), _player.ActionPoint, Tile.HighlightColor.Blue);
            else
                Highlight(path.Destination, _player.ActionPoint - path.Count, Tile.HighlightColor.Blue);

            foreach (Tile wayPoint in path)
                Highlight(wayPoint, 0, Tile.HighlightColor.Green);
            
        }
    }

    public void Highlight(Tile center, int range, Tile.HighlightColor color)
    {
        Highlight(center, 0, range, int.MinValue, color);
    }

    public void Highlight(Tile center, int range, int mask, Tile.HighlightColor color)
    {
        Highlight(center, 0, range, mask, color);
    }

    public void Highlight(Tile center, int nearest, int farest, int mask, Tile.HighlightColor color)
    {
        bool[,] isVisited = new bool[Length, Width];

        Highlight(center, nearest, farest, mask, color, ref isVisited);
    }

    private void Highlight(Tile center, int nearest, int farest, int mask, Tile.HighlightColor color, ref bool[,] isVisited)
    {
        center.Highlight(color);
        checktimes++;

        int x = center.x;
        int y = center.y;

        if (farest > nearest)
        {
            int newNearest = Math.Max(0, nearest - 1);
            int newFarest = farest - 1;

            if (x + 1 < Length && !isVisited[x + 1, y])
            {
                Tile tile = grid[x + 1, y];

                if ((tile.Mask & mask) != 0)
                    Highlight(tile, newNearest, newFarest, mask, color, ref isVisited);
                else
                    isVisited[x + 1, y] = true;
            }

            if (x - 1 >= 0 && !isVisited[x - 1, y])
            {
                Tile tile = grid[x - 1, y];

                if ((tile.Mask & mask) != 0)
                    Highlight(tile, newNearest, newFarest, mask, color, ref isVisited);
                else
                    isVisited[x - 1, y] = true;
            }

            if (y + 1 < Width && !isVisited[x, y + 1])
            {
                Tile tile = grid[x, y + 1];

                if ((tile.Mask & mask) != 0)
                    Highlight(tile, newNearest, newFarest, mask, color, ref isVisited);
                else
                    isVisited[x, y + 1] = true;
            }

            if (y - 1 >= 0 && !isVisited[x, y - 1])
            {
                Tile tile = grid[x, y - 1];

                if ((tile.Mask & mask) != 0)
                    Highlight(tile, newNearest, newFarest, mask, color, ref isVisited);
                else
                    isVisited[x, y - 1] = true;
            }
        }
    }

    //private void setHighlight(int x, int y,bool recheck , bool ignore_distance)
    //{
    //    //Debug.Log("x:" + x +" y: "+ y);
    //    if (x >= 0 && x < grid.Length && y >= 0 && y < grid.Length)
    //    {
    //        var temp = grid[x,y];

    //        int ret = NeighborCheck(temp);
    //        if ( ret > 0 && temp.walkable)
    //        {
    //            if (ignore_distance)
    //            {
    //                temp.distance = Mathf.Abs(temp.x - x)+Mathf.Abs(temp.y - y); 
    //            }
    //            else
    //            {
    //                temp.distance = ret; 
    //            }
    //            temp.HighlightTile();
    //        }
    //        else if(ret == 0&&recheck&&temp.walkable)
    //        {
    //                recheck_list.Add(temp);
    //            // Debug.Log("neighbor check failed");
    //        }
    //    }
    //}

    //private int NeighborCheck(Tile tile)
    //{
    //    // check any near 4 tiles is highlighted or not
    //    // return the distance to center
    //    for (int x = -1; x <= 1; x++)
    //    {
    //        for (int y = -1; y <= 1; y++)
    //        {
    //            if (x == 0 && y == 0)
    //            {
    //                // center skip
    //                continue;
    //            }
    //            if (x != 0 && y != 0)
    //            {
    //                // disable diagonal check.  remove this 'if' statement when we want diagonal check
    //                continue;
    //            }
    //            int temp_x = tile.x + x;
    //            int temp_y = tile.y + y;
    //            if( temp_x >= 0 && temp_x < grid.Length && temp_y >= 0 && temp_y < grid.Length)
    //            {
    //                if (grid[temp_x, temp_y].highlighted && grid[temp_x, temp_y].distance + 1 <= range)
    //                {
    //                    return grid[temp_x, temp_y].distance + 1;
    //                }
    //            }
    //        }
    //    }
    //    return 0;
    //}
    
    //public void setActionPoints(int _Action_point)
    //{
    //    //set Action points
    //    action_point = _Action_point;
    //}

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
