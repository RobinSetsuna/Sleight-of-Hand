﻿using System.Collections;
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
				}
				LevelManager.Instance.StartEnvironmentActionPhase(); // finish start phase, to the action phase
				break;
			case Phase.Action:
				if (currentEnemy)
				{
					currentEnemy.CurrentEnemyState = EnemyMoveState.Idle;
				}
				else
				{
					LevelManager.Instance.EndEnvironmentActionPhase();
				}
				break;
			case Phase.End:
				if (currentEnemy != null)
				{
					currentEnemy.CurrentEnemyState = EnemyMoveState.Unmoveable;
				}
				currentEnemy = null;
				index++;
				if (index >= Enemies.Count())
				{
					//all the enemy has moved
					index = 0;
					LevelManager.Instance.NextRound();
				}
				LevelManager.Instance.StartNextPhaseTurn();
				break;
		}
	}

}