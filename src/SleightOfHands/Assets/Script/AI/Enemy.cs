using System.Collections;
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
    [SerializeField]private int attack_range;
	private GameObject player;

	private int counter = 0;
    public int AttackRange
    {
        get
        {
            return attack_range;
        }
        set
        {
            attack_range = value;
        }
    }
    public int DetectionRange
    {
        get
        {
            return detection_range;
        }
        set
        {
            detection_range = value;
        }
    }
    public bool DetectionHighlighted = false;
    public HashSet<Tile> RangeList;
    private List<Vector2Int> pathList;
    private int nextPosIndex;
    private bool newRound;
    private EnemyDetectionState currentDetectionState;
    private EnemyDetectionState previousState = EnemyDetectionState.Normal;

    /// <summary>
    /// The current Detection state of the Enemy
    /// </summary>
    public EnemyDetectionState CurrentDetectionState
    {
        get
        {
            return currentDetectionState;
        }
        set
        {
#if UNITY_EDITOR
            LogUtility.PrintLogFormat("Enemy", "Made a Detection transition to {0}.", value);
#endif
            currentDetectionState = value;
        }
    }

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
                //onPathUpdate.Invoke(path); 
                //not invoke the event now. since we are not using it.
            }
        }
    }
    
    
    /// <summary>
    /// The current state of the Enemy
    /// </summary>
    private EnemyMoveState currentEnemyState;
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
                //switch (currentEnemyState)
                //{
                //}
            }
            else
            {

                EnemyMoveState previousEnemyState = currentEnemyState;

                currentEnemyState = value;

                // After entering the new state
                switch (currentEnemyState)
                {
                    case EnemyMoveState.Unmoveable:
                        Path = null;
                        break;
                    case EnemyMoveState.Idle:
                        if (Ap <= (currentDetectionState == EnemyDetectionState.Normal ? InitialActionPoint - InitialActionPoint / 2 : 0))
                            LevelManager.Instance.EndEnvironmentActionPhase();
                        else
                            CurrentEnemyState = EnemyMoveState.Move;
                        Path = null;
                        break;
                    case EnemyMoveState.Move:
                        if (InAttackRange())
                            StartCoroutine(AttackDecision());
                        else
                            StartCoroutine(MovementDecision());
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

    public void refresh() {
        newRound = true;
    }
    public void mute() {
        newRound = false;
    }
    
    /// <summary>
    /// Attack Decision Tree
    /// </summary>
    private IEnumerator AttackDecision()
    {
        if (InAttackRange() && Ap >= 1 && currentDetectionState ==  EnemyDetectionState.Found)
        {
            //is ok to Atk 
            // TODO 
            //ADD HEALTH UPDATE HERE

            EnemyManager.Instance.AttackPop(transform);

            Statistics.AddStatusEffect(new StatusEffect(1, 2));

            yield return new WaitForSeconds(1.5f);
            LevelManager.Instance.EndEnvironmentActionPhase();
            // remove the actionPoint
        }
        yield return null;
    }

    /// <summary>
    /// Movement Decision Tree
    /// </summary>
    private IEnumerator MovementDecision()
    {
        Tile enemyTile = GridManager.Instance.GetTile(transform.position);
        switch (CurrentDetectionState)
        {
                case EnemyDetectionState.Found:
                    if (newRound) {
                        EnemyManager.Instance.FoundPop(transform);
                        yield return new WaitForSeconds(1.5f);
                        newRound = false;
                     } 

                    Tile playerTile = GridManager.Instance.GetTile(player.transform.position);
                    Tile finalDes = NearPosition(playerTile, enemyTile);
                    if (finalDes == null)
                    {
                        LevelManager.Instance.EndEnvironmentActionPhase();
                        yield return null;
                    }
                    Path = Navigation.FindPath(GridManager.Instance, enemyTile, finalDes, GridManager.Instance.IsWalkable);
                    break;
                case EnemyDetectionState.Normal:
                    if (pathList.Count==0)
                    {
                        break;
                    }
                    if (newRound) {
                        EnemyManager.Instance.IdlePop(transform);
                        yield return new WaitForSeconds(1.5f);
                        newRound = false;
                    }
                    var enemyPos = enemyTile.gridPosition;
                    if (pathList.Exists(node=>node.x==enemyPos.x&&node.y== enemyPos.y))
                    {
                    //var current = pathList.FindIndex(node => node.x == enemyPos.x && node.y == enemyPos.y);
                        var current = nextPosIndex;
                        int destinationIndex = current + 1;
                        if (destinationIndex >= pathList.Count) destinationIndex = 0;
                        nextPosIndex = destinationIndex;
                        Tile destination = GridManager.Instance.GetTile(pathList[destinationIndex]);
                        Path = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable);
                    }
                    else
                    {
                        Tile destination = GridManager.Instance.GetTile(pathList[FindClosestPathNode(enemyPos)]);
                        Path = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable);
                    }
                    break;
                case EnemyDetectionState.Doubt:
                    // currently not use doubt status
                   // Debug.Log("Doubt status, the code should never trigger this.");
                    break;
        }

        if (path != null)
        {
            int temp = currentDetectionState == EnemyDetectionState.Normal ? Ap / 2 : Ap;

            for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
            {
                if (temp == 0)
                    break;

                ActionManager.Singleton.AddBack(new Movement(this, tile));//, --temp == 0 ? (System.Action)ResetToIdle : null);

                temp--;
            }

            Path = null;
            ActionManager.Singleton.Execute(ResetToIdle);
        }
    }
    
    /// <summary>
    /// check if the player is in the attack range
    /// </summary>
    private bool InAttackRange()
    {
        Tile enemyTile = GridManager.Instance.GetTile(transform.position);
        Tile playerTile = GridManager.Instance.GetTile(player.transform.position);
        int distance = Mathf.Abs(enemyTile.x - playerTile.x) + Mathf.Abs(enemyTile.y - playerTile.y)  ;
        return distance <= attack_range;
    }

    /// <summary>
    /// a setter for enemy Detection
    /// </summary>
    public void SetDetectionState(EnemyDetectionState current)
    {
        if (currentDetectionState == EnemyDetectionState.Found && current != EnemyDetectionState.Found)
        {
            EnemyManager.Instance.QuestionPop(transform);
        }
        previousState = CurrentDetectionState;
        currentDetectionState = current;
        
    }
    
    /// <summary>
    /// a set the pathList from loadedData
    /// </summary>
    public void setPathList(List<Vector2Int> pl)
    {
        pathList = pl;
    }
    
    /// <summary>
    /// Make a transition to PlayerState.Move
    /// </summary>
    private void ResetToIdle()
    {
        StartCoroutine(resetToIdle());
    }
    private IEnumerator resetToIdle() {
        if (previousState != currentDetectionState) {
            yield return new WaitForSeconds(1f);
        }
        CurrentEnemyState = EnemyMoveState.Idle;
        yield return null;
    }

    /// <summary>
    /// Highlight the detection range in tile 
    /// </summary>

    public void HighlightDetection()
    {
        // show the range to be detected
        if (DetectionHighlighted)
        {
            foreach (Tile tile in RangeList)
            {
                tile.Dehighlight();
            }
            DetectionHighlighted = false;
            Enemy[] allEnemies = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in allEnemies)
            {
                if (enemy.DetectionHighlighted)
                {
                    foreach (Tile tile in enemy.RangeList)
                    {
                        tile.Highlight(Tile.HighlightColor.Red);
                    }
                }
            }
        }
        else
        {
            foreach (Tile tile in RangeList)
            {
                tile.Dehighlight();
            }
            Tile current_tile = GridManager.Instance.GetTile(transform.position);
            RangeList = ProjectileManager.Instance.getProjectileRange(current_tile, DetectionRange, true, transform.rotation.eulerAngles.y);
            foreach (Tile tile in RangeList)
            {
                tile.Highlight(Tile.HighlightColor.Red);
            }
            DetectionHighlighted = true;
        }
    }
    /// <summary>
    /// Every movement from enemy and player will get detected, and check the range of detection to set the detection status
    /// </summary>
	private void HandleDetection(Unit unit, Vector2Int previousPos, Vector2Int pos)
    {
        var yRot = transform.rotation.eulerAngles.y; 
        if (unit.tag == "Player"||unit.tag == "Enemy")
        {
            if (currentDetectionState == EnemyDetectionState.Normal) {
                Tile current_tile = GridManager.Instance.GetTile(transform.position);
                RangeList = ProjectileManager.Instance.getProjectileRange(current_tile, detection_range, true, yRot);
                if (RangeList.Contains(GridManager.Instance.GetTile(player.GetComponent<player>().GridPosition)))
                {
                    //detected

                    // add some operation here
                    SetDetectionState(EnemyDetectionState.Found);
                    
                    StartCoroutine(Founded());
                }
            }

        }
	}

    /// <summary>
    /// when the enemy is off the designed path, find the closest node first then set it as target.
    /// return the index in pathList
    /// </summary>
    private int FindClosestPathNode(Vector2Int current)
    {
        int counter = 9999;
        int nearest = 0;
        for(int i = 0; i <pathList.Count;i++)
        {
            Vector2Int pos = pathList[i];
            if (Mathf.Abs(pos.x - current.x) + Mathf.Abs(pos.y - current.y) < counter)
            {
                if (!GridManager.Instance.HasUnitOn(pos.x, pos.y))
                {
                    nearest = i;
                    counter = Mathf.Abs(pos.x - current.x) + Mathf.Abs(pos.y - current.y);
                }
            }
        }
        return nearest;
    }
    
    
    /// <summary>
    /// get and check the tile around the player to set path.
    /// return the nearest tile that around player.
    /// </summary>
    private Tile NearPosition(Tile center,Tile current)
    {
        int counter = 9999;
        Tile nearest = null;
        List<Tile> neighbor = new List<Tile>();
        if (center.x + 1 < GridManager.Instance.Width)
        {
            neighbor.Add(GridManager.Instance.GetTile(new Vector2Int(center.x+1,center.y)));
        }
        if (center.x - 1 > 0)
        {
            neighbor.Add(GridManager.Instance.GetTile(new Vector2Int(center.x-1,center.y)));
        }

        if (center.y + 1 < GridManager.Instance.Length)
        {
            neighbor.Add(GridManager.Instance.GetTile(new Vector2Int(center.x,center.y+1)));
        }
        
        if (center.y - 1 > 0)
        {
            neighbor.Add(GridManager.Instance.GetTile(new Vector2Int(center.x,center.y-1)));
        }
        foreach(Tile tile in neighbor){
            if (Mathf.Abs(tile.x - current.x) + Mathf.Abs(tile.y - current.y) < counter)
            {
                if (!GridManager.Instance.HasUnitOn(tile.x, tile.y) && tile.walkable)
                {
                    nearest = tile;
                    counter = Mathf.Abs(tile.x - current.x) + Mathf.Abs(tile.y - current.y);
                }
            }

            if (current.gridPosition == tile.gridPosition)
            {
                return null;
            }
        }
        return nearest;
    }

    private IEnumerator Founded()
    {
        CameraManager.Instance.FocusAt(transform.position);
        EnemyManager.Instance.AlertPop(transform);
        AudioSource _audioSource = gameObject.GetComponent<AudioSource>();
        AudioClip audioClip = Resources.Load<AudioClip>("Audio/SFX/beDetected");

        _audioSource.clip = audioClip;
        _audioSource.Play();

        yield break;
    }
}
