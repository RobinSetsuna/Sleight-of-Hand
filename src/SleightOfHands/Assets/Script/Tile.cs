using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public Color defaultColor;

	public Color mouseOverColor;

	public Color selectedColor;

	private bool mouseOver = false;

	private bool selected = false;
	// Use this for initialization
	private void OnMouseEnter()
	{

			mouseOver = true;
			GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
	}
	
	// Update is called once per frame
	void OnMouseExit() {
			mouseOver = false;
			GetComponent<Renderer>().material.SetColor("_Color", defaultColor);
	}
	void OnMouseDrag() {
		//Debug.Log("holding");
		GetComponent<Renderer>().material.SetColor("_Color", selectedColor);
	}

}
