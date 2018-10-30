using UnityEngine;
using UnityEngine.Events;

public class MouseInputManager
{
    private static readonly MouseInputManager singleton = new MouseInputManager();

    public static MouseInputManager Singleton
    {
        get { return singleton; }
    }

    public static readonly int mouseFocusThreshold = 200;
    public static readonly float mouseDragThreshold = 10;

    public class EventOnMouseDrag : UnityEvent<MouseInteractable, Vector3, Vector3> {}

    //public EventOnDataUpdate<MouseInteractable> OnMouseDown = new EventOnDataUpdate<MouseInteractable>();
    public EventOnDataUpdate<MouseInteractable> OnMouseDrag = new EventOnDataUpdate<MouseInteractable>();
    //public EventOnDataUpdate<MouseInteractable> OnMouseUp = new EventOnDataUpdate<MouseInteractable>();

    public EventOnDataUpdate<MouseInteractable> onMouseEnter = new EventOnDataUpdate<MouseInteractable>();
    public EventOnDataUpdate<MouseInteractable> onMouseFocus = new EventOnDataUpdate<MouseInteractable>();
    public EventOnDataUpdate<MouseInteractable> onMouseClick = new EventOnDataUpdate<MouseInteractable>();
    public EventOnDataUpdate<MouseInteractable> onDragEnd = new EventOnDataUpdate<MouseInteractable>();

    public bool IsMouseDown { get; private set; }
    public long MouseDownTime { get; private set; }
    public Vector3 MouseDownPosition { get; private set; }

    public bool IsMouseDragging { get; private set; }

    public MouseInteractable CurrentMouseTarget { get; private set; }

    private MouseInputManager() {}

    internal void NotifyMouseDown(MouseInteractable obj)
    {
        IsMouseDown = true;
        MouseDownTime = TimeUtility.localTimeInMilisecond;
        MouseDownPosition = Input.mousePosition;

        //OnMouseDown.Invoke(obj);
    }

    internal void NotifyMouseDrag(MouseInteractable obj)
    {

        if (!IsMouseDragging)
        {
            if (Vector3.Distance(Input.mousePosition, MouseDownPosition) > mouseDragThreshold)
            {
                IsMouseDragging = true;
                OnMouseDrag.Invoke(obj);
            }
        }
        else
            OnMouseDrag.Invoke(obj);
    }

    internal void NotifyMouseEnter(MouseInteractable obj)
    {
        if (!IsMouseDragging && IsMouseDown)
            IsMouseDragging = true;
        
        CurrentMouseTarget = obj;

        onMouseEnter.Invoke(obj);
    }

    internal void NotifyMouseUp(MouseInteractable obj)
    {
        IsMouseDown = false;

        if (!IsMouseDragging)
        {
            if (TimeUtility.localTimeInMilisecond - MouseDownTime > mouseFocusThreshold)
                onMouseFocus.Invoke(obj);
            else
            {
                onMouseClick.Invoke(obj);
                Debug.Log(obj);
            }
        }
        else
        {
            IsMouseDragging = false;
            onDragEnd.Invoke(obj);
        }

        //OnMouseUp.Invoke(obj);
    }
}
