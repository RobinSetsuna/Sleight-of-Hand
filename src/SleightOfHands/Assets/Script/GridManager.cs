﻿using System;
using UnityEngine;
using System.Collections.Generic;


public class GridManager : MonoBehaviour {

    [SerializeField]private GameObject map;
    private Tile[,] grid;
    
    public LayerMask unwalkableMask;
    public float nodeRadius;
    public Transform tilePrefab;
    public Vector2 mapSize;
    public static GridManager _instance;


    [Range(0,1)]
    public float outlinePercent;

    public bool dragging = false;
    private HashSet<Tile>  recheck_list;
    private int range;
    void Start() {
        GenerateMap ();
        _instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
            //Debug.Log("hold!");
        }
        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
            //Debug.Log("hold!");
        }
    }

    public Tile TileFromWorldPoint(Vector3 worldPosition)
    {

        Tile navigate_tile = grid[0, 0];
        // take the position of grid[0,0] to check the distance of grid shuffled 
        float percentX = (worldPosition.x - navigate_tile.transform.position.x)/ (mapSize.x * nodeRadius * 2);
        float percentY = (worldPosition.z - navigate_tile.transform.position.z)/ (mapSize.y  * nodeRadius * 2);
        
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((mapSize.x-1) * percentX);
        int y = Mathf.RoundToInt((mapSize.y-1) * percentY);
        //Debug.Log("x: "+ x + " y：" + y);
        return grid[x,y];
    }
    /// <GenerateMap>
    /// generated grid map
    /// map_size : the number of cube in row and column
    /// node_radius: the cube radius
    /// parameter: None
    /// </summary>
    public void GenerateMap() {

        string holderName = "Generated Map";
        if (transform.Find(holderName)) {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject (holderName).transform;
        mapHolder.parent = transform;
        
        grid = new Tile[Mathf.RoundToInt(mapSize.x),Mathf.RoundToInt(mapSize.y)];
        for (int x = 0; x < mapSize.x; x ++) {
            for (int y = 0; y < mapSize.y; y ++) {
                Vector3 tilePosition = new Vector3(-mapSize.x/2 +nodeRadius + x + transform.position.x, 2, -mapSize.y/2 + nodeRadius + y + transform.position.z);
                bool walkable = !(Physics.CheckSphere(tilePosition,nodeRadius,unwalkableMask));
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one * (1-outlinePercent);
                newTile.parent = mapHolder;
                var temp = newTile.GetComponent<Tile>();
                temp.walkable = walkable;
                //temp.worldPosition = tilePosition;
                temp.gridPosition = new Vector2(x,y);
                grid[x,y] = temp;
            }
        }
    }
    
    /// <Hightlight>
    /// give center tile, highlight range, and highlight type to highlight specific group of tiles.
    /// parameter: (Tile(tile),Range(int),flag(int))
    /// 
    /// flag: 1 obstructed by the wall. ignore all tiles behind the wall. example usage: effect can not across the wall
    /// flag: 2 ignore distance, just avoid unwalkable tile , example usage: effect can across the wall
    /// flag: 3 consider distance, not ignore the tiles behind wall. example usage:movement use
    /// </summary>
    public void Highlight(Tile center, int _range, int flag)
    {
        bool ignoreDistance = (flag == 2);
        bool rechecking = (flag == 3||flag == 2);
        range = _range;
        center.Highlight_tile();
        center.distance = 0;
        recheck_list = new HashSet<Tile>();
        for (int x = 0; x <= range; x++)
        {
            for (int y = 0; y <= range; y++)
            {
                if (x + y <= range && x + y != 0)
                {
                    // tile is out of range or already highlight
                    //Debug.Log("x:" + x +" y: "+ y + "is out of range");
                    set_highlight(center.x + x, center.y + y,rechecking,ignoreDistance);
                    set_highlight(center.x - x, center.y + y,rechecking,ignoreDistance);
                    set_highlight(center.x + x, center.y - y,rechecking,ignoreDistance);
                    set_highlight(center.x - x, center.y - y,rechecking,ignoreDistance);
                }        
            }
        }

        foreach (Tile tile in recheck_list)
        {
            //print("rechecking: "+ tile.x + " "+ tile.y);
            set_highlight(tile.x, tile.y, false, false);
        }
    }

    private void set_highlight(int x, int y,bool recheck , bool ignore_distance)
    {
        //Debug.Log("x:" + x +" y: "+ y);
        if (x >= 0 && x < grid.Length && y >= 0 && y < grid.Length)
        {
            var temp = grid[x,y];

            int ret = NeighborCheck(temp);
            if ( ret > 0 && temp.walkable)
            {
                if (ignore_distance)
                {
                    temp.distance = Mathf.Abs(temp.x - x)+Mathf.Abs(temp.y - y); 
                }
                else
                {
                    temp.distance = ret; 
                }
                temp.Highlight_tile();
            }
            else if(ret == 0&&recheck&&temp.walkable)
            {
                    recheck_list.Add(temp);
                // Debug.Log("neighbor check failed");
            }
        }
    }

    private int NeighborCheck(Tile tile)
    {
        // check any near 4 tiles is highlighted or not
        // return the distance to center
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    // center skip
                    continue;
                }
                if (x != 0 && y != 0)
                {
                    // disable diagonal check.  remove this 'if' statement when we want diagonal check
                    continue;
                }
                int temp_x = tile.x + x;
                int temp_y = tile.y + y;
                if( temp_x >= 0 && temp_x < grid.Length && temp_y >= 0 && temp_y < grid.Length)
                {
                    if (grid[temp_x, temp_y].highlighted && grid[temp_x, temp_y].distance + 1 <= range)
                    {
                        return grid[temp_x, temp_y].distance + 1;
                    }
                }
            }
        }
        return 0;
    }
}