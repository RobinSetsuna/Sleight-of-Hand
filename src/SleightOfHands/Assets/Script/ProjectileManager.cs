using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public HashSet<Tile> getProjectileRange(Tile center,int detection_range)
    {
        // Player moved; Stealth Detection
        retList = new HashSet<Tile>();
        checked_obstacles = new HashSet<Vector2Int>();
        int range = detection_range;
        retList.Add(center);
        for (int x = 0; x <= range; x++)
        {
            bool ret1 = true;
            bool ret2 = true;
            bool ret3 = true;
            bool ret4 = true;
            bool ret5 = true;
            bool ret6 = true;
            bool ret7 = true;
            bool ret8 = true;
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
                        ret5 = process_status_check(center.x + x, center.y - y);
                    }
                    if (ret6)
                    {
                        ret6 = process_status_check(center.x + y, center.y - x);
                    }
                    if (ret7)
                    {
                        ret7 = process_status_check(center.x - x, center.y - y);
                    }
                    if (ret8)
                    {
                        ret8 = process_status_check(center.x - y, center.y - x);
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
            var temp = GridManager.Instance.getTile(x,y);
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
                    if (retList.Contains(GridManager.Instance.getTile(temp_x,temp_y)))
                    {
                        ret = true;
                    }
    
                    Tile onCheck = GridManager.Instance.getTile(temp_x, temp_y);
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
