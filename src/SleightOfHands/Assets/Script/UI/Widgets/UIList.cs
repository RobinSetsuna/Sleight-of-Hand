using System;
using UnityEngine;

public class UIList : UIWidget
{
    [SerializeField] Vector2 margin = new Vector2(10, 10);
    [SerializeField] private int row = 1;
    [SerializeField] private int column = 1;
    [SerializeField] Vector2 itemSize;
    [SerializeField] private bool hideInactives = true;

    public override void Refresh()
    {
        int i = 0;
        while (i < transform.childCount)
        {
            Transform item = transform.GetChild(i);

            if (!item.gameObject.activeSelf && hideInactives)
                break;

            int x = i % row;
            int y = i / row;

            if (y > column)
                break;

            item.localPosition = new Vector3((itemSize.x + margin.x) * (x - 1) + margin.x, (itemSize.y + margin.y) * (y - 1) - margin.y, 0);

            i++;
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(margin.x + Math.Min(column, i) * itemSize.x, margin.y + (i / row) * itemSize.y);
    }
}
