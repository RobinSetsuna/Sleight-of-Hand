using System.Collections.Generic;
using UnityEngine;

public class EnemyManager
{
    private static EnemyManager instance = new EnemyManager();
    public static EnemyManager Instance
    {
        get
        {
            return instance;
        }
    }

    private List<EnemyController> enemies = new List<EnemyController>();
    private int currentEnemyIndex;

    private System.Action callback;

    private EnemyManager() {}

    public void StartEnemyRound(int turn, System.Action callback)
    {
        this.callback = callback;

        currentEnemyIndex = -1;

        ActivateNextEnemy();
    }

    internal void Initialize(List<EnemyController> enemies)
    {
        this.enemies = enemies;
    }

    private void ActivateNextEnemy()
    {
        if (++currentEnemyIndex == enemies.Count)
        {
            if (callback != null)
                callback.Invoke();
        }
        else
        {
            Transform currentEnemyTransform = enemies[currentEnemyIndex].transform;

            CameraManager.Instance.FocusAt(currentEnemyTransform.position, ActivateCurrentEnemy);
            CameraManager.Instance.BoundCameraFollow(currentEnemyTransform);
        }
    }

    private void ActivateCurrentEnemy()
    {
        enemies[currentEnemyIndex].Activate(ActivateNextEnemy);
    }

    public void AlertPop(Transform enemy)
    {
        var temp = Object.Instantiate(ResourceUtility.GetPrefab<GameObject>("AlertBubble"), enemy.position, Quaternion.identity, enemy);
        Object.Destroy(temp, 0.5f);
    }

    public void AttackPop(Transform enemy)
    {
        var temp = Object.Instantiate(ResourceUtility.GetPrefab<GameObject>("AttackBubble"), enemy.position, Quaternion.identity, enemy);
        Object.Destroy(temp, 1.5f);
    }

    public void IdlePop(Transform enemy)
    {
        var temp = Object.Instantiate(ResourceUtility.GetPrefab<GameObject>("IdleBubble"), enemy.position, Quaternion.identity, enemy);
        Object.Destroy(temp, 1.5f);
    }

    public void FoundPop(Transform enemy)
    {
        var temp = Object.Instantiate(ResourceUtility.GetPrefab<GameObject>("FoundBubble"), enemy.position, Quaternion.identity, enemy);
        Object.Destroy(temp, 1.5f);
    }

    public void QuestionPop(Transform enemy)
    {
        var temp = Object.Instantiate(ResourceUtility.GetPrefab<GameObject>("QuestionBubble"), enemy.position, Quaternion.identity, enemy);
        Object.Destroy(temp, 1.5f);
    }

    ////EnemyList
    //public List<Enemy> Enemies;

    //// internal use
    //private int index = 0;
    //private Enemy currentEnemy = null;

    //private System.Action callback;

    //   private void Awake()
    //   {
    //       LevelManager.Instance.OnCurrentPhaseChangeForEnvironment.AddListener(HandleCurrentPhaseChange);
    //   }

    //   private void OnDestroy()
    //   {
    //       //Disable();

    //       LevelManager.Instance.OnCurrentPhaseChangeForEnvironment.RemoveListener(HandleCurrentPhaseChange);
    //   }

    //   public void setEnemies(List<Enemy> objList)
    //   {
    //       //call this in the end of each round
    //       // remove the moved mark to each enemy in a new round
    //       Enemies = new List<Enemy>();
    //       Enemies = objList;
    //   }

    //   private void HandleCurrentPhaseChange(Phase currentPhase)
    //{
    //	switch (currentPhase)
    //	{
    //		case Phase.Start:
    //			if (currentEnemy == null && index < Enemies.Count())
    //			{
    //				currentEnemy = Enemies[index];
    //                   currentEnemy.Refresh();
    //               }
    //			DehighlightAll();
    //			LevelManager.Instance.StartEnvironmentActionPhase(); // finish start phase, to the action phase
    //			break;
    //		case Phase.Action:
    //			if (currentEnemy)
    //			{
    //                   Transform enemyTransform = currentEnemy.transform;
    //                   CameraManager.Instance.FocusAt(enemyTransform.position, MoveCurrentEnemy);
    //                   CameraManager.Instance.BoundCameraFollow(enemyTransform);
    //               }
    //			else
    //			{
    //				LevelManager.Instance.EndEnvironmentActionPhase();
    //			}
    //			break;
    //		case Phase.End:
    //			if (currentEnemy != null)
    //			{
    //                   currentEnemy.Mute();
    //				currentEnemy.CurrentEnemyState = EnemyMoveState.Unmoveable;
    //			}
    //			currentEnemy = null;
    //			if (++index >= Enemies.Count())
    //			{
    //				//all the enemy has moved
    //				index = 0;
    //				DehighlightAll();
    //				CameraManager.Instance.CameraZoomOut();
    //				LevelManager.Instance.NextRound();
    //			}
    //			LevelManager.Instance.StartNextPhaseTurn();
    //			break;
    //	}
    //}

    //   public void MoveCurrentEnemy()
    //   {
    //       currentEnemy.CurrentEnemyState = EnemyMoveState.Idle;
    //   }

    //public void DehighlightAll()
    //{
    //	Enemy[] allEnemies = FindObjectsOfType<Enemy>();
    //	foreach (Enemy enemy in allEnemies)
    //           if (enemy.DetectionHighlighted)
    //           {
    //               enemy.DetectionHighlighted = false;
    //               foreach (Tile tile in enemy.RangeList)
    //                   tile.Dehighlight();
    //           }
    //}
}
