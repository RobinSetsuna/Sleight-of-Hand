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

    public class EventOnDataChange : UnityEvent<MouseInteractable> {}
    public EventOnDataChange OnCurrentMouseTargetChange = new EventOnDataChange();
    public EventOnDataChange OnObjectClicked = new EventOnDataChange();
    public EventOnDataChange OnEndDragging = new EventOnDataChange();

    public long MouseDownTime { get; private set; }
    public bool IsMouseDragging { get; private set; }

    private MouseInteractable currentMouseTarget;
    public MouseInteractable CurrentMouseTarget
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

    private MouseInteractable lastClickedObject;
    public MouseInteractable LastClickedObject
    {
        get
        {
            return lastClickedObject;
        }

        private set
        {
            //if (value != lastClickedObject)
            lastClickedObject = value;
        }
    }

    private MouseInputManager() {}

    internal void NotifyMouseDown(MouseInteractable obj)
    {
        MouseDownTime = TimeUtility.localTimeInMilisecond;
    }

    internal void NotifyMouseDrag(MouseInteractable obj)
    {
        if (!IsMouseDragging)
            IsMouseDragging = TimeUtility.localTimeInMilisecond - MouseDownTime > mouseDragThreshold;
    }

    internal void NotifyMouseEnter(MouseInteractable obj)
    {
        CurrentMouseTarget = obj;
    }

    internal void NotifyMouseUp(MouseInteractable obj)
    {
        if (!IsMouseDragging)
        {
            LastClickedObject = obj;
            OnObjectClicked.Invoke(obj);
        }
        else
        {
            IsMouseDragging = false;
            OnEndDragging.Invoke(obj);
        }
    }
}
