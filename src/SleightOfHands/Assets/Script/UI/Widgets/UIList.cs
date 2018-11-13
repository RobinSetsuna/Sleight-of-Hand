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
        {
            if (i == 0)
                background.sizeDelta = new Vector2(0, 0);
            else
                background.sizeDelta = new Vector2(margin.x + Math.Min(column, i) * (itemSize.x + margin.x), margin.y + ((i - 1) / column + 1) * (itemSize.y + margin.x));
        }
    }
}
