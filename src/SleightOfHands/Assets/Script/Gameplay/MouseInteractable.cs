using UnityEngine;

public class MouseInteractable : MonoBehaviour
{
    private void OnMouseDown()
    {
        MouseInputManager.Singleton.NotifyMouseDown(gameObject);
    }

    private void OnMouseDrag()
    {
        MouseInputManager.Singleton.NotifyMouseDrag(gameObject);
    }

    private void OnMouseOver()
    {
        MouseInputManager.Singleton.NotifyMouseOver(gameObject);
    }

    private void OnMouseUp()
    {
        MouseInputManager.Singleton.NotifyMouseUp(gameObject);
    }
}
