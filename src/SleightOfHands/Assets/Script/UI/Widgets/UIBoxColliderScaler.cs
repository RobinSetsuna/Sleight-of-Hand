using UnityEngine;

public class UIBoxColliderScaler : UIWidget
{
    private void Start()
    {
        if (!GetComponent<BoxCollider>())
            Destroy(this);
    }

    public override void Refresh()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        Vector2 pivot = rectTransform.pivot;
        Vector2 size = rectTransform.sizeDelta;

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.center = size * 0.5f - pivot * size;
        boxCollider.size = size;
    }
}
