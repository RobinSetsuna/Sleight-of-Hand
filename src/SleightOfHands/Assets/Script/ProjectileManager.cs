using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// all implementation of Public utlization function goes here.
/// </summary>
public class ProjectileManager: MonoBehaviour{
    
    private static ProjectileManager instance;
    public static ProjectileManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.Find("Level Manager").GetComponent<ProjectileManager>();
            return instance;
        }
    }
    
    private HashSet<Vector2Int> checked_obstacles;
    private HashSet<Tile> retList;
    public HashSet<Tile> getProjectileRange(Tile center,int detection_range,bool directed,float yRot)
    {
        // Player moved; Stealth Detection
        retList = new HashSet<Tile>();
        checked_obstacles = new HashSet<Vector2Int>();
        int range = detection_range;
        retList.Add(center);
        bool Quadrant_1 = false;
        bool Quadrant_2 = false;
        bool Quadrant_3 = false;
        bool Quadrant_4 = false;
        if (directed)
        {
            if (Math.Round(yRot) == 0)
            {
                Quadrant_1 = true;
                Quadrant_2 = true;
            }else if (Math.Round(yRot) == 90)
            {
                Quadrant_1 = true;
                Quadrant_4 = true;
            }else if (Math.Round(yRot) == 180)
            {
                Quadrant_3 = true;
                Quadrant_4 = true;
            }
            else if (Math.Round(yRot) == 270)
            {
                Quadrant_2 = true;
                Quadrant_3 = true;
            }
        }
        else
        {
             Quadrant_1 = true;
             Quadrant_2 = true;
             Quadrant_3 = true;
             Quadrant_4 = true;
        }

        for (int x = 0; x <= range; x++)
        {
            bool ret1 = false;
            bool ret2 = false;
            bool ret3 = false;
            bool ret4 = false;
            bool ret5 = false;
            bool ret6 = false;
            bool ret7 = false;
            bool ret8 = false;
            if (Quadrant_1){
                ret1 = true;
                ret2 = true;
            }
            if (Quadrant_2){
                ret3 = true;
                ret4 = true;
            }
            if (Quadrant_3){
                ret5 = true;
                ret6 = true;
            }
            if (Quadrant_4){
                ret7 = true;
                ret8 = true;
            }

            for (int y = x; y <= range; y++)
            {
				
                if (x + y <= range&&x+y!=0)
                {
                    // tile is out of range or already highlight
                    if (ret1)
                    {
                        ret1 = process_status_check(center.x + x, center.y + y);
                    }
                    if (ret2)
                    {
                        ret2 = process_status_check(center.x + y, center.y + x);
                    }
                    if (ret3)
                    {
                        ret3 = process_status_check(center.x - x, center.y + y);
                    }
                    if (ret4)
                    {
                        ret4 = process_status_check(center.x - y, center.y + x);
                    }
                    if (ret5)
                    {
                        ret5 = process_status_check(center.x - x, center.y - y);
                    }
                    if (ret6)
                    {
                        ret6 = process_status_check(center.x - y, center.y - x);
                    }
                    if (ret7)
                    {
                        ret7 = process_status_check(center.x + x, center.y - y);
                    }
                    if (ret8)
                    {
                        ret8 = process_status_check(center.x + y, center.y - x);
                    }
                }        
            }
        }

        //recheck_for_Obstacles(center,detection_range);
        return retList;
    }
    private bool process_status_check(int x, int y)
    {
        //Debug.Log("x:" + x +" y: "+ y);
        if (x >= 0 && x < GridManager.Instance.Width && y >= 0 && y < GridManager.Instance.Length)
        {
            var temp = GridManager.Instance.GetTile(x,y);
            if ( NeighborCheck(temp) && temp.walkable)
            {
                //Debug.Log("Highlighted");
                retList.Add(temp);
                return true;
            }
            if (!temp.walkable)
            {
                checked_obstacles.Add(new Vector2Int(temp.x,temp.y));
                return false;
            }
        }
         
        return true;
    }


    private bool NeighborCheck(Tile tile)
    {
        // check any near 4 tiles is highlighted or not
        // return the distance to center
        bool ret = false;
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
                if( temp_x >= 0 && temp_x < GridManager.Instance.Width && temp_y >= 0 && temp_y < GridManager.Instance.Length)
                {
                    if (retList.Contains(GridManager.Instance.GetTile(temp_x,temp_y)))
                    {
                        ret = true;
                    }
    
                    Tile onCheck = GridManager.Instance.GetTile(temp_x, temp_y);
                    Vector2Int onCheck_pos = new Vector2Int(temp_x,temp_y);
                    if (!onCheck.walkable&&checked_obstacles.Contains(onCheck_pos))
                    {
                        ret = false;
                    }
                }
            }
        }
        return ret;
    }
    
}
