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

public class EnemyController : MonoBehaviour
{
    public EventOnDataChange<EnemyState> onCurrentEnemyStateChange = new EventOnDataChange<EnemyState>();

	private Enemy pawn;
    public Unit Target { get; internal set; }

    private List<Vector2Int> wayPoints;

    private int nextPosIndex;
    private bool newRound;

    private DetectionState currentDetectionState;
    private DetectionState previousDetectionState = DetectionState.Normal;

    private System.Action callback;

    /// <summary>
    /// The current Detection state of the Enemy
    /// </summary>
    public DetectionState CurrentDetectionState
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
    private EnemyState currentEnemyState;
    public EnemyState CurrentEnemyState
    {
        get
        {
            return currentEnemyState;
        }

        private set
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
                EnemyState previousEnemyState = currentEnemyState;
                currentEnemyState = value;

                // After entering the new state
                switch (currentEnemyState)
                {
                    case EnemyState.Deactivated:
                        Path = null;
                        break;
                    case EnemyState.Idle:
                        switch (CurrentDetectionState)
                        {
                            case DetectionState.Normal:
                                if (apToUse > 0)
                                    CurrentEnemyState = EnemyState.Move;
                                break;
                        }
                        if (pawn.Ap <= (currentDetectionState == DetectionState.Normal ? pawn.InitialActionPoint - pawn.InitialActionPoint / 2 : 0))
                            Deactivate();
                        else
                            CurrentEnemyState = EnemyState.Move;
                        break;
                    case EnemyState.Move:
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

    private void Start()
    {
        pawn = GetComponent<Enemy>();

        LevelManager.Instance.onGameEnd.AddListener(StopAllCoroutines);
    }

    internal void Activate(System.Action callback)
    {
        this.callback = callback;

        newRound = true;
        CurrentEnemyState = EnemyState.Idle;
    }

    private void Deactivate()
    {
        if (callback != null)
            callback.Invoke();
    }

    /// <summary>
    /// Attack Decision Tree
    /// </summary>
    private IEnumerator AttackDecision()
    {
        if (InAttackRange() && pawn.Ap >= 1 && currentDetectionState == DetectionState.Found)
        {
            player Player = LevelManager.Instance.Player;

            transform.LookAt(Player.transform, Vector3.up);

            EnemyManager.Instance.AttackPop(transform);
            yield return new WaitForSeconds(2f);

            Player.Statistics.ApplyDamage(pawn.attack);
            pawn.Statistics.AddStatusEffect(new StatusEffect(1, 2));

            LevelManager.Instance.EndEnvironmentActionPhase();
        }
    }

    /// <summary>
    /// Movement Decision Tree
    /// </summary>
    private IEnumerator MovementDecision()
    {
        Tile enemyTile = GridManager.Instance.GetTile(transform.position);
        switch (CurrentDetectionState)
        {
                case DetectionState.Found:
                    if (newRound) {
                        EnemyManager.Instance.FoundPop(transform);
                        yield return new WaitForSeconds(1.5f);
                        newRound = false;
                     }
                    Tile playerTile = GridManager.Instance.GetTile(Player.transform.position);
                    Tile finalDes = NearPosition(playerTile, enemyTile);
                    if (finalDes == null)
                    {
                        LevelManager.Instance.EndEnvironmentActionPhase();
                        yield break;
                    }
                    // --------------------------------------------------------------------------------------------------------------------
                    // this code used as a temp solution, must change the actionManager to remove the NAN Movement action
                    finalDes = Navigation.FindPath(GridManager.Instance, enemyTile, finalDes, GridManager.Instance.IsWalkable).GetSecond();
                    Path = Navigation.FindPath(GridManager.Instance, enemyTile, finalDes, GridManager.Instance.IsWalkable);
                    // --------------------------------------------------------------------------------------------------------------------
                    break;
                case DetectionState.Normal:
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
                        Path = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable);
                    }
                    else
                    {
                        Tile destination = GridManager.Instance.GetTile(wayPoints[FindClosestPathNode(enemyPos)]);
                        Path = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable);
                    }
                    break;
                case DetectionState.Doubt:
                    // currently not use doubt status
                   // Debug.Log("Doubt status, the code should never trigger this.");
                    int x = Random.Range(-1, 1);
                    int y = Random.Range(-1, 1);
                    while (Mathf.Abs(x) + Mathf.Abs(y) > 1)
                    {
                        x = Random.Range(-1, 1);
                        y = Random.Range(-1, 1);
                    }
                    int temp_x = enemyTile.x - 1;
                    int temp_y = enemyTile.y + 0;
                    if (GridManager.Instance.HasUnitOn(temp_x, temp_y) ||
                        !GridManager.Instance.IsWalkable(temp_x, temp_y))
                    {
                        // Oops, no movement
                        path = null;
                        LevelManager.Instance.EndEnvironmentActionPhase();
                    }
                    else
                    {
                        Tile destination = GridManager.Instance.GetTile(temp_x, temp_y);
                        Path = Navigation.FindPath(GridManager.Instance, enemyTile, destination, GridManager.Instance.IsWalkable);
                    }
                    break;
        }

        if (path != null)
        {
            int temp = currentDetectionState == DetectionState.Normal ? pawn.Ap / 2 : pawn.Ap;

            for (Tile tile = path.Reset(); !path.IsFinished(); tile = path.MoveForward())
            {
                if (temp == 0)
                    break;

                ActionManager.Singleton.AddBack(new Movement(pawn, tile), ResetToIdle);
            }
            Path = null;
        }
    }

    /// <summary>
    /// check if the player is in the attack range
    /// </summary>
    private bool InAttackRange()
    {
        Tile enemyTile = GridManager.Instance.GetTile(transform.position);
        Tile playerTile = GridManager.Instance.GetTile(LevelManager.Instance.Player.transform.position);

        return MathUtility.ManhattanDistance(enemyTile.x, enemyTile.y, playerTile.x, playerTile.y) <= pawn.AttackRange;
    }

    /// <summary>
    /// a setter for enemy Detection
    /// </summary>
    public void SetDetectionState(DetectionState current)
    {
        if (currentDetectionState == DetectionState.Found && current != DetectionState.Found)
            EnemyManager.Instance.QuestionPop(transform);

        previousDetectionState = CurrentDetectionState;
        currentDetectionState = current;
    }

    /// <summary>
    /// a set the pathList from loadedData
    /// </summary>
    public void SetPathList(List<Vector2Int> pl)
    {
        wayPoints = pl;
    }

    /// <summary>
    /// Make a transition to PlayerState.Move
    /// </summary>
    private void ResetToIdle()
    {
        StartCoroutine(resetToIdle());
    }

    private IEnumerator resetToIdle()
    {
        if (previousDetectionState != currentDetectionState)
        {
            previousDetectionState = currentDetectionState;
            yield return new WaitForSeconds(1f);
        }

        if (previousDetectionState == DetectionState.Doubt)
            LevelManager.Instance.EndEnvironmentActionPhase();
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
}
