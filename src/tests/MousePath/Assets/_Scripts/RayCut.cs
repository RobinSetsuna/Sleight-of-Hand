using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCut : MonoBehaviour {

    private Color originColor;

    // public GameObject heartPart;

    public List<GameObject> ToBeDestroyed = new List<GameObject>();

    private List<GameObject> playerParts = new List<GameObject>();

	// Use this for initialization
	void Start () {
        originColor = GameObject.FindWithTag("tiny part").GetComponent<SpriteRenderer>().color;
        
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButton(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Vector2 heartVec = new Vector2(heartPart.transform.position.x, heartPart.transform.position.y);
            //Vector2 dir = mousePos - heartVec;

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.tag == "tiny part")
            {
                //Vector2 endPos = mousePos + dir * 10.0f;
                //Debug.DrawLine(mousePos, endPos, Color.blue);
                Vector2 toDiePos;

                GameObject toDie = hit.collider.gameObject;
                // Debug.Log(hit.collider.gameObject.transform.position);
                toDie.GetComponent<SpriteRenderer>().color = Color.red;
                ToBeDestroyed.Add(toDie);
                // Destroy(hit.collider.gameObject);
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            foreach (GameObject go in ToBeDestroyed) {
                // Destroy(go);
                go.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
	}
}
