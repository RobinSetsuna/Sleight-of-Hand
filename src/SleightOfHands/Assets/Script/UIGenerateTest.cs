using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGenerateTest : MonoBehaviour {

    public GameObject canvas;

    public GameObject image;

	// Use this for initialization
	void Start () {

        float canvasWidth = canvas.gameObject.GetComponent<RectTransform>().rect.width;
        float canvasHeight = canvas.gameObject.GetComponent<RectTransform>().rect.height;
        float imageWidth = image.gameObject.GetComponent<RectTransform>().rect.width;
        float imageHeight = image.gameObject.GetComponent<RectTransform>().rect.height;

        GameObject imageSpawn = Instantiate(image) as GameObject;

        imageSpawn.transform.SetParent(canvas.transform);
        // instantiate at the right bottom corner of the canvas
        imageSpawn.transform.localPosition = new Vector3(canvasWidth / 2 - imageWidth / 2, - canvasHeight / 2 + imageHeight / 2, 0);
        imageSpawn.transform.localRotation = canvas.transform.rotation;
	}
	
}
