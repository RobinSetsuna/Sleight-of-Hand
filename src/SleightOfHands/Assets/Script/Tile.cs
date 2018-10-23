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
		
		if (walkable && !GridManager._instance.dragging && !selected)
		{
			mouseOver = true;
			GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
		}
		
		if (walkable && GridManager._instance.dragging && !selected && GridManager._instance.AccessibleCheck(x,y))
		{
			selected = true;
			GetComponent<Renderer>().material.SetColor("_Color", selectedColor);
		}
	}

	private void OnMouseOver()
	{
		if (walkable && !GridManager._instance.dragging && !selected)
		{
			mouseOver = true;
			GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
		}
		
		if (walkable && GridManager._instance.dragging && !selected && GridManager._instance.AccessibleCheck(x,y))
		{
			print( x + " " + y);
			selected = true;
			GetComponent<Renderer>().material.SetColor("_Color", selectedColor);
		}
	}

	// Update is called once per frame
	void OnMouseExit() {
		if (walkable&&!selected)
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

	public void set_selected()
	{
		selected = true;
		GetComponent<Renderer>().material.SetColor("_Color", selectedColor);
	}
	
	public void wipe()
	{
		highlighted = false;
		selected = false;
		GetComponent<Renderer>().material.SetColor("_Color", defaultColor);
	}
	public void Highlight_tile()
	{
		highlighted = true;
		GetComponent<Renderer>().material.SetColor("_Color", highlightColor);
	}

}
