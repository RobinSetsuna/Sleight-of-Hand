﻿using UnityEngine;

public class UIBoxColliderScaler : UIWidget
{
    private void Start()
    {
        if (!GetComponent<BoxCollider>())
            Destroy(this);
    }

    public override void Refresh(params object[] args)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        Vector2 pivot = rectTransform.pivot;
        Vector2 size = rectTransform.sizeDelta;

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.center = (Vector3)(size * 0.5f - pivot * size) + new Vector3(0, 0, 1);
        boxCollider.size = size;
    }
}
