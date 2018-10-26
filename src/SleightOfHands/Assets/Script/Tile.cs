using System;
using UnityEngine;

///	<summary/>
/// Tile
/// tile interaction, tile status change.
/// </summary>
public class Tile : MouseInteractable, IEquatable<Tile>
{
    public enum HighlightColor : int
    {
        Blue = 1,
        Green = 2,
        Cyan = 3,
        Red = 4,
        Magenta = 5,
        Yello = 6,
        None = 7,
    }

    [SerializeField]private Color defaultColor;

	[SerializeField]private Color mouseOverColor;

	[SerializeField]private Color selectedColor;

	[SerializeField]private Color highlightColor;

    public bool walkable
    {
        get
        {
            return IsWalkable();
        }

        set
        {
            Mask = BitOperationUtility.WriteBit(Mask, 31, value);
        }
    }

	//public Vector3 worldPosition;
	public Vector2Int gridPosition;
	public int distance = 9999;

    /// <summary>
    /// w
    /// a
    /// l                     p
    /// k                     l              g
    /// a                     a              r
    /// b                     y              e
    /// l                     e              e
    /// e                     r              n
    /// |                     |              |
    /// 0000 0000 0000 0000 0000 0000 0000 0000
    ///                      | |            | |
    ///                      e o            r b
    ///                      n b            e l
    ///                      e s            d u
    ///                      m t              e
    ///                      y a
    ///                        c
    ///                        l
    ///                        e
    /// </summary>
    public int Mask { get; private set; }

	public int x
	{
		get
		{
            // return  Mathf.RoundToInt(gridPosition.x);
            return gridPosition.x;
		}
	}

	public int y
    {
		get
		{
            // return  Mathf.RoundToInt(gridPosition.y);
            return gridPosition.y;
		}
	}

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

    public bool IsWalkable()
    {
        return Mask < 0;
    }

    public bool IsHighlighted()
    {
        return (Mask & 0xf) != (int)HighlightColor.None;
    }

    public void setSelected()
	{
		// set tile to selected
		selected = true;
		GetComponent<Renderer>().material.SetColor("_Color", selectedColor);
	}
	
	public void Wipe()
	{
		//wipe all held effect for tile
		selected = false;

		GetComponent<Renderer>().material.SetColor("_Color", defaultColor);
	}

    public void Dehighlight()
    {
        Highlight(HighlightColor.None);
    }

	public void Highlight(HighlightColor color)
	{
        int mask = (int)color;

        if (((Mask ^ mask) & 0xf) != 0)
        {
            Mask = BitOperationUtility.WriteBits(Mask, mask, 0, 3);
            RefreshHighlight();
        }
	}

    public bool Equals(Tile other)
    {
        return gridPosition == other.gridPosition;
    }

    override public string ToString()
    {
        return gridPosition.ToString();
    }

    private void RefreshHighlight()
    {
        GetComponent<Renderer>().material.SetColor("_Color", new Color(BitOperationUtility.ReadBit(Mask, 2), BitOperationUtility.ReadBit(Mask, 1), BitOperationUtility.ReadBit(Mask, 0)));
    }
}
