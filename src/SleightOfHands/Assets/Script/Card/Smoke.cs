using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour {
    int duration = 2;
    int counter = 0;
    int detectionRange = 3;
	// Use this for initialization
	void Start () {
        LevelManager.Instance.onCurrentTurnChange.AddListener(HandleTimeOut);
        CameraManager.Instance.FocusAt(this.transform.position);
    }
	
	// Update is called once per frame
	void Update () {
		if(GameObject.FindGameObjectsWithTag("Enemy") != null)
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject obj in enemies)
            {
                Enemy enemy = obj.GetComponent<Enemy>();
                if (CheckEnemy(obj))
                {
                    //StartCoroutine(Question(obj));
                    if (enemy.CurrentDetectionState == EnemyDetectionState.Found)
                    {
                        EnemyManager.Instance.QuestionPop(obj.transform);

                        enemy.DetectionRange = 0;
                        enemy.SetDetectionState(EnemyDetectionState.Normal);
                    }
                }
                else
                {
                        enemy.DetectionRange = 3;
                }
            }
        }
	}

    private bool CheckEnemy(GameObject obj)
    {
        Tile enemyTile = GridManager.Instance.GetTile(obj.transform.position);
        Tile smokeTile = GridManager.Instance.GetTile(this.transform.position);
        if (MathUtility.ManhattanDistance(enemyTile.x, enemyTile.y, smokeTile.x, smokeTile.y) <= detectionRange)
            return true;
        else
            return false;
    }

    void HandleTimeOut(int turn)
    {
        counter++;
        if(counter == duration)
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject obj in enemies)
            {
                if (CheckEnemy(obj))
                {
                    obj.GetComponent<Enemy>().DetectionRange = 3;
                }
            }
            Destroy(this.gameObject);
        }
        
    }
}
