using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glue : MonoBehaviour {
    int duration = 2;
    int counter = 0;
    // Use this for initialization
    void Start () {
        LevelManager.Instance.onCurrentTurnChange.AddListener(HandleTimeOut);
        CameraManager.Instance.FocusAt(this.transform.position);
        //GridManager.Instance.GetTile(this.transform.position).walkable = false;
    }
	
	// Update is called once per frame
	void Update () {
        //if (GameObject.FindGameObjectsWithTag("Enemy") != null)
        //{
        //    var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //    foreach (GameObject obj in enemies)
        //    {
        //        Tile enemyTile = GridManager.Instance.GetTile(obj.transform.position);      
        //        Tile GlueTile = GridManager.Instance.GetTile(this.gameObject.transform.position);
        //        if (enemyTile.transform.position == GlueTile.transform.position)
        //        {
        //            StartCoroutine(Question(obj));
        //            obj.GetComponent<Enemy>().speed = -10;
                  
        //            //obj.transform.position = new Vector3(GlueTile.transform.position.x, obj.transform.position.y, GlueTile.transform.position.z);
        //            //Destroy(this.gameObject);
        //        }
        //    }
        //}      
    }

    void HandleTimeOut(int turn)
    {
        counter++;
        if (counter == duration)
        {
            //GridManager.Instance.GetTile(this.transform.position).walkable = true;
            Destroy(this.gameObject);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy)
        {
            EnemyManager.Instance.QuestionPop(other.gameObject.transform);
        }  
    }

    private void OnTriggerExit(Collider other)
    {


    }

}
