using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBSTransparent : MonoBehaviour {

    private List<Renderer> ObstacleRenderer;

    private Renderer tempRend;

    public GameObject player;

    public GameObject mainCamera;

    public float obsAlpha;

	// Use this for initialization
	void Start () {
        ObstacleRenderer = new List<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {

        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        Debug.DrawLine(player.transform.position, mainCamera.transform.position, Color.red);

        RaycastHit[] hit;
        hit = Physics.RaycastAll(player.transform.position, mainCamera.transform.position);

        if (hit.Length > 0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                tempRend = hit[i].collider.gameObject.GetComponent<Renderer>();
                ObstacleRenderer.Add(tempRend);
                SetMaterialAlpha(tempRend, obsAlpha);
            }
        }
        else {
            for (int i = 0; i < ObstacleRenderer.Count; i++) {
                tempRend = ObstacleRenderer[i];
                SetMaterialAlpha(tempRend, 1.0f);
            }
        }
	}

    private void SetMaterialAlpha(Renderer r, float tAlpha) {
        int num = r.materials.Length;
        for (int i = 0; i < num; i++) {
            Color color = r.materials[i].color;
            color.a = tAlpha;
            r.materials[i].SetColor("_Color", color);
        }
    }
}
