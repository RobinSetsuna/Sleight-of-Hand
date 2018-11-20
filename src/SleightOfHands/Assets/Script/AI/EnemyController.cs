using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState : int
{
    Deactivated,
    Idle,
    Move,
    Attack,
}

public enum EnemyMode : int
{
    Patrolling,
    Chasing,
    Dazzled,
}

public class EnemyController : MouseInteractable
{
	private Enemy enemy;

    private List<Vector2Int> wayPoints;

    private int nextPosIndex;
    private bool newRound;
    
    private EnemyMode previousMode = EnemyMode.Patrolling;

    private System.Action callback;
    private Path<Tile> path;

    private int uid = -1;
    public int UID
    {
        get
        {
            return uid;
        }

        internal set
        {
            if (uid < 0)
                uid = value;
        }
    }

    /// <summary>
    /// The current behavior mode of the enemy
    /// </summary>
    private EnemyMode mode;
    public EnemyMode Mode
    {
        get
        {
            return mode;
        }

        internal set
        {
            if (value != mode)
            {
#if UNITY_EDITOR
                Debug.Log(LogUtility.MakeLogStringFormat("EnemyController", "Change mode from {0} to {1}.", mode, value));
#endif
                // Before leaving the previous mode
                switch (mode)
                {
                    case EnemyMode.Chasing:
                        LevelManager.Instance.Player.onStatisticChange.RemoveListener(HandleTargetStatisticChange);
                        break;
                }

                mode = value;

                if (currentEnemyState == EnemyState.Deactivated)
                    previousMode = value;

                // After entering the new mode
                switch (mode)
                {
                    case EnemyMode.Chasing:
                        LevelManager.Instance.Player.onStatisticChange.AddListener(HandleTargetStatisticChange);
                        Founded();
                        break;
                }
            }
        }
    }

    /// <summary>
    /// The current state of the enemy
    /// </summary>
    private EnemyState currentEnemyState;
    public EnemyState CurrentEnemyState
    {
        get
        {
            return currentEnemyState;
        }

        private set
        {


            // Reset the current state
            if (value == currentEnemyState)
            {
#if UNITY_EDITOR
                Debug.Log(LogUtility.MakeLogStringFormat("EnemyController", "Reset the current state ({0}).", currentEnemyState));
#endif

                //switch (currentEnemyState)
                //{
                //}
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log(LogUtility.MakeLogStringFormat("EnemyController", "Made a transition from {0} to {1}.", currentEnemyState, value));
#endif

                // Before leaving the previous state
                //switch (currentEnemyState)
                //{
                //}

                //EnemyState previousEnemyState = currentEnemyState;
                currentEnemyState = value;

                // After entering the new state
                //switch (currentEnemyState)
                //{
                //}

                // In the new state
                switch (currentEnemyState)
                {
                    case EnemyState.Deactivated:
                        path = null;
                        if (callback != null)
                            callback.Invoke();
                        break;

                    case EnemyState.Idle:
                        if (enemy.Ap <= (mode == EnemyMode.Patrolling ? enemy.InitialActionPoint - enemy.InitialActionPoint / 2 : 0))
                            CurrentEnemyState = EnemyState.Deactivated;
                        else
                            CurrentEnemyState = EnemyState.Move;
                        path = null;
                        break;

                    case EnemyState.Move:
                        if (IsInAttackRange(LevelManager.Instance.Player))
                            StartCoroutine(AttackDecision());
                        else
                            StartCoroutine(MovementDecision());
                        break;
                }
            }
        }
    }

    public void NotifyDetection(player Player)
    {
        Vector2Int playerGridPosition = Player.GridPosition;
        Vector2Int enemyGridPosition = enemy.GridPosition;

        if (Player.VisibleRange >= MathUtility.ManhattanDistance(playerGridPosition.x, playerGridPosition.y, enemyGridPosition.x, enemyGridPosition.y))
            Mode = EnemyMode.Chasing;
    }

    /// <summary>
    /// a set the pathList from loadedData
    /// </summary>
    internal void SetWayPoints(List<Vector2Int> wayPoints)
    {
        this.wayPoints = wayPoints;
    }

    internal void Activate(System.Action callback)
    {
        this.callback = callback;

        newRound = true;
        CurrentEnemyState = EnemyState.Idle;
    }

    private void Start()
    {
        enemy = GetComponent<Enemy>();

        enemy.onStatusEffectChange.AddListener(HandleStatusEffectChange);
    }

    /// <summary>
    /// Attack Decision Tree
    /// </summary>
    private IEnumerator AttackDecision()
    {
        player Player = LevelManager.Instance.Player;

        if (IsInAttackRange(Player) && enemy.Ap >= 1 && mode == EnemyMode.Chasing)
        {
            transform.LookAt(Player.transform, Vector3.up);

            EnemyManager.Instance.AttackPop(transform);
            yield return new WaitForSeconds(2f);

            Player.ApplyDamage(enemy.Attack);
            enemy.Statistics.ApplyFatigue(1);

            CurrentEnemyState = EnemyState.Deactivated;
        }
    }

    /// <summary>
    /// Movement Decision Tree
    /// </summary>
    private IEnumerator MovementDecision()
    {
        Tile enemyTile = GridManager.Instance.GetTile(transform.position);
        switch (Mode)
        {
                case EnemyMode.Chasing:
                    if (newRound) {
                        EnemyManager.Instance.FoundPop(transform);
                        yield return new WaitForSeconds(1.5f);
                        newRound = false;
                     }
                    Tile playerTile = GridManager.Instance.GetTile(LevelManager.Instance.Player.transform.position);
                    Tile finalDes = NearPosition(playerTile, enemyTile);
                    if (finalDes == null)
                    {
                        CurrentEnemyState = EnemyState.Deactivated;
                        yield break;
                    }
                    finalDes = Navigation.FindPath(GridManager.Instance, enemyTile, finalDes, GridManager.Instance.IsWalkable).GetSecond();
                    path = Navigation.FindPath(GridManager.Instance, enemyTile, finalDes, GridManager.Instance.IsWalkable);
                    break;

                case EnemyMode.Patrolling:
                    if (wayPoints.Count==0)
                    {
                        break;
                    }
                    if (newRound) {
                        EnemyManager.Instance.IdlePop(transform);
                        yield return new WaitForSeconds(1.5f);
                        newRound = false;
                    }
                    var enemyPos = enemyTile.gridPosition;
                    if (wayPoints.Exists(node=>node.x==enemyPos.x&&node.y== enemyPos.y)){
                    //var current = pathList.FindIndex(node => node.x == enemyPos.x && node.y == enemyPos.y);
                        var current = nextPosIndex;
                        int destinationIndex = current + 1;
                        if (destinationIndex >= wayPoints.Count) destinationIndex = 0;
                        nextPosIndex = destinationIndex;
                        Tile destination = GridManager.Instance.GetTile(wayPoints[destinationIndex]);
                        destination = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable).GetSecond();
                        if (GridManager.Instance.HasUnitOn(destination.x, destination.y))
                        {
                            path = null;
                        }
                        else
                        {
                            path = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable);  
                        }

                        
                    }
                    else
                    {
                        Tile destination = GridManager.Instance.GetTile(wayPoints[FindClosestPathNode(enemyPos)]);
                        if (GridManager.Instance.HasUnitOn(destination.x, destination.y))
                        {
                            path = null;
                        }
                        else
                        {
                            path = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable);  
                        }
                    }
                    break;

                case EnemyMode.Dazzled:
                    int x = Random.Range(-1, 1);
                    int y = Random.Range(-1, 1);
                    while (Mathf.Abs(x) + Mathf.Abs(y) > 1)
                    {
                        x = Random.Range(-1, 1);
                        y = Random.Range(-1, 1);
                    }
                    int temp_x = enemyTile.x - 1;
                    int temp_y = enemyTile.y + 0;
                    if (GridManager.Instance.HasUnitOn(temp_x, temp_y) || !GridManager.Instance.IsWalkable(temp_x, temp_y))
                    {
                        CurrentEnemyState = EnemyState.Deactivated;
                    }
                    else
                    {
                        Tile destination = GridManager.Instance.GetTile(temp_x, temp_y);
                        path = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable);
                    }
                    break;
        }

        if (path != null)
        {
            int temp = mode == EnemyMode.Patrolling ? enemy.Ap / 2 : enemy.Ap;

            for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
            {
                if (temp == 0)
                    break;

                ActionManager.Singleton.AddBack(new Movement(enemy, tile), ResetToIdle);
            }
            path = null;
        }
        else
        {
            CurrentEnemyState = EnemyState.Deactivated;
        }
    }

    /// <summary>
    /// check if the player is in the attack range
    /// </summary>
    private bool IsInAttackRange(Unit target)
    {
        Tile enemyTile = GridManager.Instance.GetTile(transform.position);
        Tile targetTile = GridManager.Instance.GetTile(target.transform.position);

        return MathUtility.ManhattanDistance(enemyTile.x, enemyTile.y, targetTile.x, targetTile.y) <= Mathf.Min(enemy.AttackRange, target.VisibleRange);
    }

    private void ResetToIdle()
    {
        StartCoroutine(resetToIdle());
    }

    private IEnumerator resetToIdle()
    {
        if (previousMode != mode)
        {
            previousMode = mode;
            yield return new WaitForSeconds(1f);
        }

        if (previousMode == EnemyMode.Dazzled)
            CurrentEnemyState = EnemyState.Deactivated;
        else
        {
            ActionManager.Singleton.Clear();
            CurrentEnemyState = EnemyState.Idle;
        }

        yield break;
    }

    /// <summary>
    /// when the enemy is off the designed path, find the closest node first then set it as target.
    /// return the index in pathList
    /// </summary>
    private int FindClosestPathNode(Vector2Int current)
    {
        int counter = 9999;
        int nearest = 0;
        for(int i = 0; i <wayPoints.Count;i++)
        {
            Vector2Int pos = wayPoints[i];
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

    private void Founded()
    {
        EnemyManager.Instance.AlertPop(transform);
        AudioSource _audioSource = gameObject.GetComponent<AudioSource>();
        AudioClip audioClip = Resources.Load<AudioClip>("Audio/SFX/beDetected");

        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

    private void HandleTargetStatisticChange(Statistic statistic, float previousValue, float currentValue)
    {
        switch (statistic)
        {
            case Statistic.VisibleRange:
                player Player = LevelManager.Instance.Player;
                Vector2Int playerGridPosition = Player.GridPosition;
                Vector2Int enemyGridPosition = enemy.GridPosition;
                if (Player.VisibleRange < MathUtility.ManhattanDistance(playerGridPosition.x, playerGridPosition.y, enemyGridPosition.x, enemyGridPosition.y))
                    Mode = EnemyMode.Patrolling;
                break;
        }
    }

    private void HandleStatusEffectChange(ChangeType change, StatusEffect statusEffect)
    {
        switch (change)
        {
            case ChangeType.Incremental:
                switch (statusEffect.Id)
                {
                    case 4: // Blind
                        Mode = EnemyMode.Dazzled;
                        break;
                }
                break;

            case ChangeType.Decremental:
                switch (statusEffect.Id)
                {
                    case 4: // Blind
                        Mode = EnemyMode.Patrolling;
                        break;
                }
                break;
        }
    }
}
