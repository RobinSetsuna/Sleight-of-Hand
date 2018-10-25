using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
///	<summary/>
/// Tile
/// tile interaction, tile status change.
/// </summary>
public class Tile : MouseInteractable, IEquatable<Tile>
{
	[SerializeField]private Color defaultColor;

	[SerializeField]private Color mouseOverColor;

	[SerializeField]private Color selectedColor;

	[SerializeField]private Color highlightColor;

	private bool mouseOver;
	
	public bool walkable;
	//public Vector3 worldPosition;
	public Vector2Int gridPosition;
	public int distance = 9999;
	public int x
	{
		get
		{
            // return  Mathf.RoundToInt(gridPosition.x);
            return gridPosition.x;
		}
	}

	public int y	{
		get
		{
            // return  Mathf.RoundToInt(gridPosition.y);
            return gridPosition.y;
		}
	}

	public bool highlighted = false;
	public bool selected = false;
	
	//private void OnMouseEnter()
	//{
		
	//	if (walkable && !GridManager.Instance.dragging && !selected)
	//	{
	//		mouseOver = true;
	//		GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
	//	}
		
	//	if (walkable && GridManager.Instance.dragging && !selected && GridManager.Instance.AccessibleCheck(x,y))
	//	{
	//		selected = true;
	//		GetComponent<Renderer>().material.SetColor("_Color", selectedColor);
	//	}
	//}

	//private void OnMouseOver()
	//{
	//	if (walkable && !GridManager.Instance.dragging && !selected)
	//	{
	//		mouseOver = true;
	//		GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
	//	}
		
	//	if (walkable && GridManager.Instance.dragging && !selected && GridManager.Instance.AccessibleCheck(x,y))
	//	{
	//		//print( x + " " + y);
	//		selected = true;
	//		GetComponent<Renderer>().material.SetColor("_Color", selectedColor);
	//	}
	//}

	//void OnMouseExit() {
	//	if (walkable&&!selected)
	//	{
	//		mouseOver = false;
	//		if (highlighted)
	//		{
	//			// back to highlight status
	//			GetComponent<Renderer>().material.SetColor("_Color", highlightColor);
	//		}
	//		else
	//		{
	//			GetComponent<Renderer>().material.SetColor("_Color", defaultColor);
	//		}
	//	}
	//}

	public void setSelected()
	{
		// set tile to selected
		selected = true;
		GetComponent<Renderer>().material.SetColor("_Color", selectedColor);
	}
	
	public void Wipe()
	{
		//wipe all held effect for tile
		highlighted = false;
		selected = false;
		GetComponent<Renderer>().material.SetColor("_Color", defaultColor);
	}
	public void HighlightTile()
	{
		// tile highlight
		highlighted = true;
		GetComponent<Renderer>().material.SetColor("_Color", highlightColor);
	}

    public bool Equals(Tile other)
    {
        return gridPosition == other.gridPosition;
    }

    override public string ToString()
    {
        return gridPosition.ToString();
    }
}
