using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{
	[SerializeField]private Color defaultColor;

	[SerializeField]private Color mouseOverColor;

	[SerializeField]private Color selectedColor;

	[SerializeField] private Color highlightColor;

	private bool mouseOver = false;
	
	public bool walkable;
	//public Vector3 worldPosition;
	public Vector2 gridPosition;
	public int distance = 9999;
	
	public int x
	{
		get
		{
			return  Mathf.RoundToInt(gridPosition.x);
		}
	}

	public int y	{
		get
		{
			return  Mathf.RoundToInt(gridPosition.y);
		}
	}

	public bool highlighted = false;
	public bool selected = false;
	// Use this for initialization
	private void OnMouseEnter()
	{
		if (walkable)
		{
			mouseOver = true;
			GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
		}
	}
	
	// Update is called once per frame
	void OnMouseExit() {
		if (walkable)
		{
			mouseOver = false;
			if (highlighted)
			{
				// back to highlight status
				GetComponent<Renderer>().material.SetColor("_Color", highlightColor);
			}
			else
			{
				GetComponent<Renderer>().material.SetColor("_Color", defaultColor);
			}
		}
	}
	void OnMouseDrag() {
		//Debug.Log("holding");
		GetComponent<Renderer>().material.SetColor("_Color", selectedColor);
	}

	public void Highlight_tile()
	{
		highlighted = true;
		GetComponent<Renderer>().material.SetColor("_Color", highlightColor);
	}

}
