﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
///	<summary/>
/// Tile
/// tile interaction, tile status change.
/// </summary>
public class Tile : MonoBehaviour
{
	[SerializeField]private Color defaultColor;

	[SerializeField]private Color mouseOverColor;

	[SerializeField]private Color selectedColor;

	[SerializeField]private Color highlightColor;

	private bool mouseOver;
	
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
	private Renderer Component
	{
		get { return  gameObject.GetComponentInChildren<Renderer>(); }
	}

	private void OnMouseEnter()
	{
		
		if (walkable && !GridManager.Instance.dragging && !selected)
		{
			mouseOver = true;
			Component.material.SetColor("_Color", mouseOverColor);
		}
		
		if (walkable && GridManager.Instance.dragging && !selected && GridManager.Instance.AccessibleCheck(x,y))
		{
			selected = true;
			Component.material.SetColor("_Color", selectedColor);
		}
	}

	private void OnMouseOver()
	{
		if (walkable && !GridManager.Instance.dragging && !selected)
		{
			mouseOver = true;
			Component.material.SetColor("_Color", mouseOverColor);
		}
		
		if (walkable && GridManager.Instance.dragging && !selected && GridManager.Instance.AccessibleCheck(x,y))
		{
			//print( x + " " + y);
			selected = true;
			Component.material.SetColor("_Color", selectedColor);
		}
	}

	void OnMouseExit() {
		if (walkable&&!selected)
		{
			mouseOver = false;
			if (highlighted)
			{
				// back to highlight status
				Component.material.SetColor("_Color", highlightColor);
			}
			else
			{
				Component.material.SetColor("_Color", defaultColor);
			}
		}
	}

	public void setSelected()
	{
		// set tile to selected
		selected = true;
		Component.material.SetColor("_Color", selectedColor);
	}
	
	public void Wipe()
	{
		//wipe all held effect for tile
		highlighted = false;
		selected = false;
		Component.material.SetColor("_Color", defaultColor);
	}
	public void HighlightTile()
	{
		// tile highlight
		highlighted = true;
		Component.material.SetColor("_Color", highlightColor);
	}

}
