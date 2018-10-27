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
        None = 0,
        Black = 1,
        Blue = 3,
        Green = 5,
        Cyan = 7,
        Red = 9,
        Magenta = 11,
        Yello = 13,
        White = 15,
    }

    //[SerializeField]private Color defaultColor;

    //[SerializeField]private Color mouseOverColor;

    //[SerializeField]private Color selectedColor;

    //[SerializeField]private Color highlightColor;

    [SerializeField][Range(0, 1)] private float alpha = 0.5f;

    public bool walkable
    {
        get
        {
            return IsWalkable();
        }

        set
        {
            BitOperationUtility.WriteBit(ref mark, 31, value);
        }
    }

	//public Vector3 worldPosition;
	public Vector2Int gridPosition;
	public int distance = 9999;

    /// <summary>
    /// w
    /// a
    /// l                     p
    /// k                     l
    /// a                     a              b
    /// b                     y            r l
    /// l                     e            e u
    /// e                     r            d e
    /// |                     |            | |
    /// 0100 0000 0000 0000 0000 0000 0000 0000
    ///  |                   | |            | |
    ///  t                   e o            g a
    ///  i                   n b            r l
    ///  l                  e s            e p
    ///  e                   m t            e h
    ///                      y a            n a
    ///                        c
    ///                        l
    ///                        e
    /// </summary>
    private int mark = 0x40000000;
    public int Mark
    {
        get
        {
            return mark;
        }
    }

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
        return mark < 0;
    }

    public bool IsHighlighted()
    {
        // return (Mark & 0xf) != 0;
        return IsHighlighted(HighlightColor.White);
    }

    public bool IsHighlighted(HighlightColor color)
    {
        int c = (int)color;
        return (mark & c) == c;
    }

    //   public void setSelected()
    //{
    //	// set tile to selected
    //	selected = true;
    //	GetComponent<Renderer>().material.SetColor("_Color", selectedColor);
    //}

    public void Wipe()
	{
		//wipe all held effect for tile
		selected = false;

		//GetComponent<Renderer>().material.SetColor("_Color", defaultColor);
	}

    public void Dehighlight()
    {
        Highlight(HighlightColor.None);
    }

	public void Highlight(HighlightColor color)
	{
        int mask = (int)color;

        if (((mark ^ mask) & 0xf) != 0)
        {
            BitOperationUtility.WriteBits(ref mark, mask, 0, 3);
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
        GetComponent<Renderer>().material.SetColor("_Color", new Color(BitOperationUtility.ReadBit(mark, 3), BitOperationUtility.ReadBit(mark, 2), BitOperationUtility.ReadBit(mark, 1), BitOperationUtility.ReadBit(mark, 0) * alpha));
    }
}
