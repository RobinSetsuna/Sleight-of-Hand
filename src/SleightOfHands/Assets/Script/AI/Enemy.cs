using System.Collections.Generic;
using UnityEngine;

public enum EnemyMoveState : int
{
    Unmoveable,
    Idle,
    Move,
    Attack
}
public enum EnemyDetectionState : int
{
    Normal,
    Doubt,
    Found
}

public class Enemy : Unit
{
    // event 
    public int ID;
    public EventOnDataChange<EnemyMoveState> onCurrentEnemyStateChange = new EventOnDataChange<EnemyMoveState>();
    
    
	[SerializeField]private int detection_range;
	private GameObject player;

	private int counter = 0;

    private bool detection_highlighted = false;
    private HashSet<Tile> rangeList;
    
    /// <summary>
    /// An event triggered whenever the planned path is changed by the player
    /// </summary>
    public EventOnDataUpdate<Path<Tile>> onPathUpdate = new EventOnDataUpdate<Path<Tile>>();
    
    private Path<Tile> path;
    private Path<Tile> Path
    {
        set
        {
            if (value != path)
            {
                path = value;
                onPathUpdate.Invoke(path);
            }
        }
    }
    private EnemyMoveState currentEnemyState;
    /// <summary>
    /// The current state of the Enemy
    /// </summary>
    public EnemyMoveState CurrentEnemyState
    {
        get
        {
            return currentEnemyState;
        }

        set
        {
#if UNITY_EDITOR
            LogUtility.PrintLogFormat("Enemy", "Made a transition to {0}.", value);
#endif
            // Reset current state
            if (value == currentEnemyState)
            {
                switch (currentEnemyState)
                {
                    case EnemyMoveState.Idle:
//                        path.Clear();
//                        onPathUpdate.Invoke(path);
                        break;
                }
            }
            else
            {
                // Before leaving the previous state
//                switch (currentEnemyState)
//                {
//                    case EnemyMoveState.Unmoveable:
//                        //Enable();
//                        break;
//                }

                EnemyMoveState previousEnemyState = CurrentEnemyState;
                currentEnemyState = value;

                // After entering the new state
                switch (currentEnemyState)
                {
                    case EnemyMoveState.Unmoveable:
                        Path = null;
                        //Disable();
                        break;
                    case EnemyMoveState.Idle:
                        if (ActionPoint <= 0)
                        {
                            //exhausted
                            LevelManager.Instance.EndEnvironmentActionPhase();
                        }
                        else
                        {
                            ActionDecsion();
                            
                        }
                        Path = null;
                        break;
                    case EnemyMoveState.Move:
                        Tile enemyTile = GridManager.Instance.GetTile(transform.position);
                        Tile playerTile = GridManager.Instance.GetTile(player.transform.position);
                        playerTile.gridPosition.y += 1;
                        Path = Navigation.FindPath(GridManager.Instance, enemyTile, playerTile);
                        
                        if (path != null)
                        {
                            int temp = ActionPoint;
                            for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
                                if (temp > 0)
                                {
                                    ActionManager.Singleton.Add(new Movement(GetComponent<Enemy>(), tile));
                                    temp--;
                                }
                            Path = null;
                            ActionManager.Singleton.Execute(ResetToIdle);
                        }else
                        {
                            LevelManager.Instance.EndEnvironmentActionPhase();
                        }
                        break;
                }
                onCurrentEnemyStateChange.Invoke(previousEnemyState, currentEnemyState);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindWithTag("Player");
        GridManager.Instance.onUnitMove.AddListener(HandleDetection);
    }
    /// <summary>
    /// Make a transition to PlayerState.Move
    /// </summary>
    private void ResetToIdle()
    {
        CurrentEnemyState = EnemyMoveState.Idle;
    }

    public void ActionDecsion()
    {
        if (ActionPoint > 0)
        {
            //decsion tree here, move or attack or anotherthing
            CurrentEnemyState = EnemyMoveState.Move;
        }
        else
        {
            //exhausted
            LevelManager.Instance.EndEnvironmentActionPhase();
        } 
    }

    public void HightlightDetection()
    {
        // show the range to be detected
        if (detection_highlighted)
        {
            foreach (Tile tile in rangeList)
            {
                tile.Dehighlight();
            }
            detection_highlighted = false;
        }
        else
        {
            Tile current_tile = GridManager.Instance.GetTile(transform.position);
            rangeList = ProjectileManager.Instance.getProjectileRange(current_tile, detection_range);

            GridManager.Instance.DehighlightAll();
            foreach (Tile tile in rangeList)
            {
                tile.Highlight(Tile.HighlightColor.Red);
            }
            detection_highlighted = true;
        }
    }

	private void HandleDetection(Unit unit, Vector2Int previousPos, Vector2Int pos)
	{
        if (unit.tag == "Player"||this)
        {
            Tile current_tile = GridManager.Instance.GetTile(transform.position);
            rangeList = ProjectileManager.Instance.getProjectileRange(current_tile, detection_range);

            if (rangeList.Contains(GridManager.Instance.GetTile(player.GetComponent<player>().GridPosition)))
            {
                //detected
                // add some operation here
            }
        }
	}
}
