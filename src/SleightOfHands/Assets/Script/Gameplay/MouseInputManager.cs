using UnityEngine;
using UnityEngine.Events;

public class MouseInputManager
{
    private static readonly MouseInputManager singleton = new MouseInputManager();
    public static MouseInputManager Singleton
    {
        get
        {
            return singleton;
        }
    }

    private static int mouseDragThreshold = 200;

    public class EventOnDataChange : UnityEvent<GameObject> {}
    public EventOnDataChange OnCurrentMouseTargetChange = new EventOnDataChange();
    public EventOnDataChange OnNewClickedObject = new EventOnDataChange();

    public long MouseDownTime { get; private set; }
    public bool IsMouseDragging { get; private set; }

    private GameObject currentMouseTarget;
    public GameObject CurrentMouseTarget
    {
        get
        {
            return currentMouseTarget;
        }

        private set
        {
            if (value != CurrentMouseTarget)
            {
                currentMouseTarget = value;
                OnCurrentMouseTargetChange.Invoke(value);
            }
        }
    }

    private GameObject lastClickedObject;
    public GameObject LastClickedObject
    {
        get
        {
            return lastClickedObject;
        }

        private set
        {
            if (value != lastClickedObject)
            {
                lastClickedObject = value;
                OnNewClickedObject.Invoke(value);
            }
        }
    }

    private MouseInputManager() {}

    internal void NotifyMouseDown(GameObject go)
    {
        MouseDownTime = TimeUtility.localTimeInMilisecond;
    }

    internal void NotifyMouseDrag(GameObject go)
    {
        if (!IsMouseDragging)
            IsMouseDragging = TimeUtility.localTimeInMilisecond - MouseDownTime > mouseDragThreshold;
    }

    internal void NotifyMouseOver(GameObject go)
    {
        CurrentMouseTarget = go;
    }

    internal void NotifyMouseUp(GameObject go)
    {
        if (!IsMouseDragging)
            LastClickedObject = go;
        else
            IsMouseDragging = false;
    }
}
