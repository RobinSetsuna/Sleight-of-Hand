using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour {
    int duration = 2;
    int counter = 0;
	// Use this for initialization
	void Start () {
        LevelManager.Instance.onCurrentTurnChange.AddListener(HandleTimeOut);
    }
	
	// Update is called once per frame
	void Update () {
		if(GameObject.FindGameObjectsWithTag("Enemy") != null)
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject obj in enemies)
            {
                if(CheckEnemy(obj))
                {
                    obj.GetComponent<Enemy>().DetectionRange = 0;
                }
                else
                {
                    obj.GetComponent<Enemy>().DetectionRange = 3;
                }
            }
        }
	}

    private bool CheckEnemy(GameObject obj)
    {
        Tile enemyTile = GridManager.Instance.GetTile(obj.transform.position);
        Tile smokeTile = GridManager.Instance.GetTile(this.transform.position);
        if (GridManager.Instance.IsAdjacent(enemyTile, smokeTile))
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
