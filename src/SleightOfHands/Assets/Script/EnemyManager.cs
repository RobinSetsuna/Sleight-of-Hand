using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	//EnemyList
	public List<Enemy> Enemies;
	private static EnemyManager instance;
	public static EnemyManager Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = GameObject.Find("Level Manager");

				if (go)
					instance = go.GetComponent<EnemyManager>();
			}
                

			return instance;
		}
	}
	
	// internal use
	private int index = 0;
	private Enemy currentEnemy = null;

	private void Awake()
	{
		LevelManager.Instance.OnCurrentPhaseChangeForEnvironment.AddListener(HandleCurrentPhaseChange);
	}

	private void OnDestroy()
	{
		//Disable();

		LevelManager.Instance.OnCurrentPhaseChangeForEnvironment.RemoveListener(HandleCurrentPhaseChange);
	}
	
	public void setEnemies(List<Enemy> objList)
	{
		//call this in the end of each round
		// remove the moved mark to each enemy in a new round
		Enemies = new List<Enemy>();
		Enemies = objList;
	}

	private void HandleCurrentPhaseChange(Phase currentPhase)
	{
		switch (currentPhase)
		{
			case Phase.Start:
				if (currentEnemy == null && index < Enemies.Count())
				{
					currentEnemy = Enemies[index];
                    currentEnemy.refresh();
                }
				LevelManager.Instance.StartEnvironmentActionPhase(); // finish start phase, to the action phase
				break;
			case Phase.Action:
				if (currentEnemy)
				{
					currentEnemy.CurrentEnemyState = EnemyMoveState.Idle;
                    Transform enemyTransform = currentEnemy.transform;
                    CameraManager.Instance.FocusAt(enemyTransform.position);
                    CameraManager.Instance.BoundCameraFollow(enemyTransform);
                }
				else
				{
					LevelManager.Instance.EndEnvironmentActionPhase();
				}
				break;
			case Phase.End:
				if (currentEnemy != null)
				{
                    currentEnemy.mute();
					currentEnemy.CurrentEnemyState = EnemyMoveState.Unmoveable;
				}
				currentEnemy = null;
				index++;
				if (index >= Enemies.Count())
				{
					//all the enemy has moved
					index = 0;
					DehighlightAll();
					CameraManager.Instance.CameraZoomOut();
					LevelManager.Instance.NextRound();
				}
				CameraManager.Instance.UnboundCameraFollow();
				LevelManager.Instance.StartNextPhaseTurn();
				break;
		}
	}

	public void DehighlightAll()
	{
		Enemy[] allEnemies = FindObjectsOfType<Enemy>();
		foreach (Enemy enemy in allEnemies)
		{
			enemy.DetectionHighlighted = false;
				foreach (Tile tile in enemy.RangeList)
				{
					tile.Dehighlight();
				}
		}
	}

	public void AlertPop(Transform enemy)
	{
		var temp = Instantiate(ResourceUtility.GetPrefab<GameObject>("AlertBubble"), enemy.position, Quaternion.identity,enemy);
		Destroy(temp,0.5f);
	}
	public void AttackPop(Transform enemy)
	{
		var temp = Instantiate(ResourceUtility.GetPrefab<GameObject>("AttackBubble"), enemy.position, Quaternion.identity,enemy);
		Destroy(temp,1.5f);
	}
	public void IdlePop(Transform enemy)
	{
		var temp = Instantiate(ResourceUtility.GetPrefab<GameObject>("IdleBubble"), enemy.position, Quaternion.identity,enemy);
		Destroy(temp,1.5f);
	}
	public void FoundPop(Transform enemy)
	{
		var temp = Instantiate(ResourceUtility.GetPrefab<GameObject>("FoundBubble"), enemy.position, Quaternion.identity,enemy);
		Destroy(temp,1.5f);
	}
	public void QuestionPop(Transform enemy)
	{
		var temp = Instantiate(ResourceUtility.GetPrefab<GameObject>("QuestionBubble"), enemy.position, Quaternion.identity,enemy);
		Destroy(temp,1.5f);
	}

}
