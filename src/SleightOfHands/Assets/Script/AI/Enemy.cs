// using System.Collections;
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
    //public EventOnDataChange<EnemyMoveState> onCurrentEnemyStateChange = new EventOnDataChange<EnemyMoveState>();

    [SerializeField] private int detectionRange;
    [SerializeField] private int attackRange;
    [SerializeField] private int attack = 100;

    public int Attack
    {
        get
        {
            return attack;
        }
    }

    //private bool isDetectionHighlighted = false;

    //private HashSet<Tile> detectionArea = new HashSet<Tile>();
    //public HashSet<Tile> DetectionArea
    //{
    //    get
    //    {
    //        return detectionArea;
    //    }
    //}

    protected override void Awake()
    {
        Statistics = new StatisticSystem(new AttributeSet(AttributeType.Ap_i, (float)initialActionPoint,
                                                          AttributeType.Hp_i, (float)initialHealth,
                                                          AttributeType.Dr_i, (float)detectionRange,
                                                          AttributeType.Ar_i, (float)attackRange));

        onStatisticChange = Statistics.onStatisticChange;
        onStatusEffectChange = Statistics.onStatusEffectChange;

        //Player = LevelManager.Instance.Player;

        LevelManager.Instance.onGameEnd.AddListener(StopAllCoroutines);

        //GridManager.Instance.onUnitMove.AddListener(HandleDetection);

        base.Awake();
    }

    protected override void OnDestroy()
    {
        LevelManager.Instance.onGameEnd.RemoveListener(StopAllCoroutines);

        base.OnDestroy();
    }

    /// <summary>
    /// Toggle the highlights of all tiles under detection
    /// </summary>
    //public void ToggleDetectionArea()
    //{
    //    // show the range to be detected
    //    if (isDetectionHighlighted)
    //    {
    //        foreach (Tile tile in detectionArea)
    //            tile.Dehighlight();

    //        isDetectionHighlighted = false;

    //        Enemy[] allEnemies = FindObjectsOfType<Enemy>();

    //        foreach (Enemy enemy in allEnemies)
    //            if (enemy.isDetectionHighlighted)
    //                foreach (Tile tile in enemy.detectionArea)
    //                    tile.Highlight(Tile.HighlightColor.Red);
    //    }
    //    else
    //    {
    //        foreach (Tile tile in detectionArea)
    //            tile.Dehighlight();

    //        Tile current_tile = GridManager.Instance.GetTile(transform.position);

    //        detectionArea = ProjectileManager.Instance.getProjectileRange(current_tile, DetectionRange, true, transform.rotation.eulerAngles.y);

    //        foreach (Tile tile in detectionArea)
    //            tile.Highlight(Tile.HighlightColor.Red);

    //        isDetectionHighlighted = true;
    //    }
    //}

    //protected override void FinishMovement(System.Action callback)
    //{
    //    detectionArea = ProjectileManager.Instance.getProjectileRange(GridManager.Instance.GetTile(transform.position), DetectionRange, true, transform.rotation.eulerAngles.y);

    //    base.FinishMovement(callback);
    //}

    //private player Player;

    //private int counter = 0;

    //private List<Vector2Int> pathList;
    //private int nextPosIndex;
    //private bool newRound;
    //private EnemyDetectionState currentDetectionState;
    //private EnemyDetectionState previousState = EnemyDetectionState.Normal;

    /// <summary>
    /// The current Detection state of the Enemy
    /// </summary>
    //    public EnemyDetectionState CurrentDetectionState
    //    {
    //        get
    //        {
    //            return currentDetectionState;
    //        }
    //        set
    //        {
    //#if UNITY_EDITOR
    //            LogUtility.PrintLogFormat("Enemy", "Made a Detection transition to {0}.", value);
    //#endif
    //            currentDetectionState = value;
    //        }
    //    }

    /// <summary>
    /// An event triggered whenever the planned path is changed by the player
    /// </summary>
    //public EventOnDataUpdate<Path<Tile>> onPathUpdate = new EventOnDataUpdate<Path<Tile>>();

    //private Path<Tile> path;
    //private Path<Tile> Path
    //{
    //    set
    //    {
    //        if (value != path)
    //        {
    //            path = value;
    //            //onPathUpdate.Invoke(path);
    //            //not invoke the event now. since we are not using it.
    //        }
    //    }
    //}


    /// <summary>
    /// The current state of the Enemy
    /// </summary>
    //    private EnemyMoveState currentEnemyState;
    //    public EnemyMoveState CurrentEnemyState
    //    {
    //        get
    //        {
    //            return currentEnemyState;
    //        }

    //        set
    //        {
    //#if UNITY_EDITOR
    //            LogUtility.PrintLogFormat("Enemy", "Made a transition to {0}.", value);
    //#endif

    //            // Reset current state
    //            if (value == currentEnemyState)
    //            {
    //                //switch (currentEnemyState)
    //                //{
    //                //}
    //            }
    //            else
    //            {
    //                EnemyMoveState previousEnemyState = currentEnemyState;

    //                currentEnemyState = value;

    //                // After entering the new state
    //                switch (currentEnemyState)
    //                {
    //                    case EnemyMoveState.Unmoveable:
    //                        Path = null;
    //                        break;
    //                    case EnemyMoveState.Idle:
    //                        if (Ap <= (currentDetectionState == EnemyDetectionState.Normal ? InitialActionPoint - InitialActionPoint / 2 : 0))
    //                            LevelManager.Instance.EndEnvironmentActionPhase();
    //                        else
    //                            CurrentEnemyState = EnemyMoveState.Move;
    //                        Path = null;
    //                        break;
    //                    case EnemyMoveState.Move:
    //                        if (InAttackRange())
    //                            StartCoroutine(AttackDecision());
    //                        else
    //                            StartCoroutine(MovementDecision());
    //                        break;
    //                }

    //                onCurrentEnemyStateChange.Invoke(previousEnemyState, currentEnemyState);
    //            }
    //        }
    //    }

    //   public void Refresh() {
    //       newRound = true;
    //   }
    //   public void Mute() {
    //       newRound = false;
    //   }

    //   /// <summary>
    //   /// Attack Decision Tree
    //   /// </summary>
    //   private IEnumerator AttackDecision()
    //   {
    //       if (InAttackRange() && Ap >= 1 && currentDetectionState == EnemyDetectionState.Found)
    //       {
    //           transform.LookAt(Player.transform, Vector3.up);

    //           EnemyManager.Instance.AttackPop(transform);
    //           yield return new WaitForSeconds(2f);

    //           Player.Statistics.ApplyDamage(attack);
    //           Statistics.AddStatusEffect(new StatusEffect(1, 2));

    //           LevelManager.Instance.EndEnvironmentActionPhase();
    //       }
    //   }

    //   /// <summary>
    //   /// Movement Decision Tree
    //   /// </summary>
    //   private IEnumerator MovementDecision()
    //   {
    //       Tile enemyTile = GridManager.Instance.GetTile(transform.position);
    //       switch (CurrentDetectionState)
    //       {
    //               case EnemyDetectionState.Found:
    //                   if (newRound) {
    //                       EnemyManager.Instance.FoundPop(transform);
    //                       yield return new WaitForSeconds(1.5f);
    //                       newRound = false;
    //                    }
    //                   Tile playerTile = GridManager.Instance.GetTile(Player.transform.position);
    //                   Tile finalDes = NearPosition(playerTile, enemyTile);
    //                   if (finalDes == null)
    //                   {
    //                       LevelManager.Instance.EndEnvironmentActionPhase();
    //                       yield break;
    //                   }
    //                   // --------------------------------------------------------------------------------------------------------------------
    //                   // this code used as a temp solution, must change the actionManager to remove the NAN Movement action
    //                   finalDes = Navigation.FindPath(GridManager.Instance, enemyTile, finalDes, GridManager.Instance.IsWalkable).GetSecond();
    //                   Path = Navigation.FindPath(GridManager.Instance, enemyTile, finalDes, GridManager.Instance.IsWalkable);
    //                   // --------------------------------------------------------------------------------------------------------------------
    //                   break;
    //               case EnemyDetectionState.Normal:
    //                   if (pathList.Count==0)
    //                   {
    //                       break;
    //                   }
    //                   if (newRound) {
    //                       EnemyManager.Instance.IdlePop(transform);
    //                       yield return new WaitForSeconds(1.5f);
    //                       newRound = false;
    //                   }
    //                   var enemyPos = enemyTile.gridPosition;
    //                   if (pathList.Exists(node=>node.x==enemyPos.x&&node.y== enemyPos.y)){
    //                   //var current = pathList.FindIndex(node => node.x == enemyPos.x && node.y == enemyPos.y);
    //                       var current = nextPosIndex;
    //                       int destinationIndex = current + 1;
    //                       if (destinationIndex >= pathList.Count) destinationIndex = 0;
    //                       nextPosIndex = destinationIndex;
    //                       Tile destination = GridManager.Instance.GetTile(pathList[destinationIndex]);
    //                       destination = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable).GetSecond();
    //                       Path = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable);
    //                   }
    //                   else
    //                   {
    //                       Tile destination = GridManager.Instance.GetTile(pathList[FindClosestPathNode(enemyPos)]);
    //                       Path = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable);
    //                   }
    //                   break;
    //               case EnemyDetectionState.Doubt:
    //                   // currently not use doubt status
    //                  // Debug.Log("Doubt status, the code should never trigger this.");
    //                   int x = Random.Range(-1, 1);
    //                   int y = Random.Range(-1, 1);
    //                   while (Mathf.Abs(x) + Mathf.Abs(y) > 1)
    //                   {
    //                       x = Random.Range(-1, 1);
    //                       y = Random.Range(-1, 1);
    //                   }
    //                   int temp_x = enemyTile.x - 1;
    //                   int temp_y = enemyTile.y + 0;
    //                   if (GridManager.Instance.HasUnitOn(temp_x, temp_y) ||
    //                       !GridManager.Instance.IsWalkable(temp_x, temp_y))
    //                   {
    //                       // Oops, no movement
    //                       path = null;
    //                       LevelManager.Instance.EndEnvironmentActionPhase();
    //                   }
    //                   else
    //                   {
    //                       Tile destination = GridManager.Instance.GetTile(temp_x, temp_y);
    //                       Path = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable);
    //                   }
    //                   break;
    //       }

    //       if (path != null)
    //       {
    //           int temp = currentDetectionState == EnemyDetectionState.Normal ? Ap / 2 : Ap;

    //           for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
    //           {
    //               if (temp == 0)
    //                   break;

    //               ActionManager.Singleton.AddBack(new Movement(this, tile), ResetToIdle);
    //           }
    //           Path = null;
    //       }
    //   }

    //   /// <summary>
    //   /// check if the player is in the attack range
    //   /// </summary>
    //   private bool InAttackRange()
    //   {
    //       Tile enemyTile = GridManager.Instance.GetTile(transform.position);
    //       Tile playerTile = GridManager.Instance.GetTile(Player.transform.position);

    //       return MathUtility.ManhattanDistance(enemyTile.x, enemyTile.y, playerTile.x, playerTile.y) <= AttackRange;
    //   }

    //   /// <summary>
    //   /// a setter for enemy Detection
    //   /// </summary>
    //   public void SetDetectionState(EnemyDetectionState current)
    //   {
    //       if (currentDetectionState == EnemyDetectionState.Found && current != EnemyDetectionState.Found)
    //       {
    //           EnemyManager.Instance.QuestionPop(transform);
    //       }
    //       previousState = CurrentDetectionState;
    //       currentDetectionState = current;

    //   }

    //   /// <summary>
    //   /// a set the pathList from loadedData
    //   /// </summary>
    //   public void SetPathList(List<Vector2Int> pl)
    //   {
    //       pathList = pl;
    //   }

    //   /// <summary>
    //   /// Make a transition to PlayerState.Move
    //   /// </summary>
    //   private void ResetToIdle()
    //   {
    //       StartCoroutine(resetToIdle());
    //   }

    //   private IEnumerator resetToIdle()
    //   {
    //       if (previousState != currentDetectionState)
    //       {
    //           previousState = currentDetectionState;
    //           yield return new WaitForSeconds(1f);
    //       }

    //       if (previousState == EnemyDetectionState.Doubt)
    //       {
    //           LevelManager.Instance.EndEnvironmentActionPhase();
    //           yield return null;
    //       }
    //       else
    //       {
    //           ActionManager.Singleton.Clear();
    //           CurrentEnemyState = EnemyMoveState.Idle;
    //           yield return null;
    //       }
    //   }

    //   /// <summary>
    //   /// Every movement from enemy and player will get detected, and check the range of detection to set the detection status
    //   /// </summary>
    //private void HandleDetection(Unit unit, Vector2Int previousPos, Vector2Int pos)
    //   {
    //       var yRot = transform.rotation.eulerAngles.y;
    //       if (unit.tag == "Player"|| unit == this)
    //       {
    //           if (currentDetectionState == EnemyDetectionState.Normal)
    //           {
    //               Tile current_tile = GridManager.Instance.GetTile(transform.position);
    //               RangeList = ProjectileManager.Instance.getProjectileRange(current_tile, DetectionRange, true, yRot);

    //               Tile playerTile = GridManager.Instance.GetTile(Player.GridPosition);

    //               if (RangeList.Contains(playerTile) && MathUtility.ManhattanDistance(current_tile.x, current_tile.y, playerTile.x, playerTile.y) <= Player.VisibleRange)
    //               {
    //                   // Player is detected
    //                   SetDetectionState(EnemyDetectionState.Found);
    //                   CameraManager.Instance.FocusAt(transform.position, Founded);
    //               }
    //           }

    //       }
    //}

    //   /// <summary>
    //   /// when the enemy is off the designed path, find the closest node first then set it as target.
    //   /// return the index in pathList
    //   /// </summary>
    //   private int FindClosestPathNode(Vector2Int current)
    //   {
    //       int counter = 9999;
    //       int nearest = 0;
    //       for(int i = 0; i <pathList.Count;i++)
    //       {
    //           Vector2Int pos = pathList[i];
    //           if (Mathf.Abs(pos.x - current.x) + Mathf.Abs(pos.y - current.y) < counter)
    //           {
    //               if (!GridManager.Instance.HasUnitOn(pos.x, pos.y))
    //               {
    //                   nearest = i;
    //                   counter = Mathf.Abs(pos.x - current.x) + Mathf.Abs(pos.y - current.y);
    //               }
    //           }
    //       }
    //       return nearest;
    //   }


    //   /// <summary>
    //   /// get and check the tile around the player to set path.
    //   /// return the nearest tile that around player.
    //   /// </summary>
    //   private Tile NearPosition(Tile center,Tile current)
    //   {
    //       int counter = 9999;
    //       Tile nearest = null;
    //       List<Tile> neighbor = new List<Tile>();
    //       if (center.x + 1 < GridManager.Instance.Width)
    //       {
    //           neighbor.Add(GridManager.Instance.GetTile(new Vector2Int(center.x+1,center.y)));
    //       }
    //       if (center.x - 1 > 0)
    //       {
    //           neighbor.Add(GridManager.Instance.GetTile(new Vector2Int(center.x-1,center.y)));
    //       }

    //       if (center.y + 1 < GridManager.Instance.Length)
    //       {
    //           neighbor.Add(GridManager.Instance.GetTile(new Vector2Int(center.x,center.y+1)));
    //       }

    //       if (center.y - 1 > 0)
    //       {
    //           neighbor.Add(GridManager.Instance.GetTile(new Vector2Int(center.x,center.y-1)));
    //       }
    //       foreach(Tile tile in neighbor){
    //           if (Mathf.Abs(tile.x - current.x) + Mathf.Abs(tile.y - current.y) < counter)
    //           {
    //               if (!GridManager.Instance.HasUnitOn(tile.x, tile.y) && tile.walkable)
    //               {
    //                   nearest = tile;
    //                   counter = Mathf.Abs(tile.x - current.x) + Mathf.Abs(tile.y - current.y);
    //               }
    //           }

    //           if (current.gridPosition == tile.gridPosition)
    //           {
    //               return null;
    //           }
    //       }
    //       return nearest;
    //   }

    //   private void Founded()
    //   {
    //       EnemyManager.Instance.AlertPop(transform);
    //       AudioSource _audioSource = gameObject.GetComponent<AudioSource>();
    //       AudioClip audioClip = Resources.Load<AudioClip>("Audio/SFX/beDetected");

    //       _audioSource.clip = audioClip;
    //       _audioSource.Play();
    //   }
}