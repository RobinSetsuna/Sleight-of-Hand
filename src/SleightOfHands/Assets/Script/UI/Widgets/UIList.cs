using System;
using UnityEngine;

public class UIList : UIWidget
{
    [SerializeField] private Vector2 margin = new Vector2(10, 10);
    [SerializeField] private int row = 1;
    [SerializeField] private int column = 1;
    [SerializeField] private Vector2 itemSize;
    [SerializeField] private bool hideInactives = true;
    [SerializeField] private RectTransform background;

    public int Count
    {
        get
        {
            int count = 0;
            for (int i = 0; i < transform.childCount; i++)
                if (transform.GetChild(i).gameObject.activeSelf)
                    count++;

            return count;
        }
    }

    public float Length
    {
        get
        {
            return CalculateLength(Count);
        }
    }

    public float Width
    {
        get
        {
            return CalculateWidth(Count);
        }
    }

    public override void Refresh(params object[] args)
    {
        int i = 0;
        int N = transform.childCount;

        while (i < N)
        {
            Transform item = transform.GetChild(i);

            if (hideInactives && !item.gameObject.activeSelf)
            {
                item.SetSiblingIndex(--N);
                continue;
            }

            int x = i % column;
            int y = i / column;

            if (y > row)
                break;

            item.localPosition = new Vector3((itemSize.x + margin.x) * x + margin.x, -(itemSize.y + margin.y) * y - margin.y, 0);
            i++;
        }

        if (background)
            background.sizeDelta = new Vector2(CalculateLength(i), CalculateWidth(i));
    }

    private float CalculateLength(int count)
    {
        return count == 0 ? 0 : margin.x + Math.Min(column, count) * (itemSize.x + margin.x);
    }

    private float CalculateWidth(int count)
    {
        return count == 0 ? 0 : margin.y + ((count - 1) / column + 1) * (itemSize.y + margin.x);
    }
}
