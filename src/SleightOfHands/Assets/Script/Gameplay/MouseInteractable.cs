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

/// <summary>
/// A component that will make all mouse inputs on this object to be detected by the MouseInputManager
/// </summary>
public class MouseInteractable : MonoBehaviour
{
    /// <summary>
    /// The type of the attached object
    /// </summary>
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
