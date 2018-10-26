using UnityEngine;

public class MouseInteractable : MonoBehaviour
{
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
