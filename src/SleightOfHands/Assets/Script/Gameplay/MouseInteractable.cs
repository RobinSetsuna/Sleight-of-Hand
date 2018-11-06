using UnityEngine;

public enum UnitType : int
{
    Default = 0,
    Player,
    Enemy,
    Tile,
    Item,
    UI,
}

public class MouseInteractable : MonoBehaviour
{
    public UnitType Type;

    private void OnMouseDown()
    {
        MouseInputManager.Singleton.NotifyMouseDown(this);
    }

    private void OnMouseDrag()
    {
        MouseInputManager.Singleton.NotifyMouseDrag(this);
    }

    private void OnMouseEnter()
    {
        MouseInputManager.Singleton.NotifyMouseEnter(this);
    }

    private void OnMouseUp()
    {
        MouseInputManager.Singleton.NotifyMouseUp(this);
    }
}
