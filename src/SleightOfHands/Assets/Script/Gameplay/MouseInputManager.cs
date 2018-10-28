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

    //public EventOnDataChange1<MouseInteractable> OnMouseDown = new EventOnDataChange1<MouseInteractable>();
    public EventOnDataChange1<MouseInteractable> OnMouseDrag = new EventOnDataChange1<MouseInteractable>();
    //public EventOnDataChange1<MouseInteractable> OnMouseUp = new EventOnDataChange1<MouseInteractable>();

    public EventOnDataChange1<MouseInteractable> OnCurrentMouseTargetChange = new EventOnDataChange1<MouseInteractable>();
    public EventOnDataChange1<MouseInteractable> OnObjectFocus = new EventOnDataChange1<MouseInteractable>();
    public EventOnDataChange1<MouseInteractable> OnObjectClicked = new EventOnDataChange1<MouseInteractable>();
    public EventOnDataChange1<MouseInteractable> OnEndDragging = new EventOnDataChange1<MouseInteractable>();

    public bool IsMouseDown { get; private set; }
    public long MouseDownTime { get; private set; }
    public Vector3 MouseDownPosition { get; private set; }

    public bool IsMouseDragging { get; private set; }

    private MouseInteractable currentMouseTarget;
    public MouseInteractable CurrentMouseTarget
    {
        get { return currentMouseTarget; }

        private set
        {
            if (value != CurrentMouseTarget)
            {
                currentMouseTarget = value;
                OnCurrentMouseTargetChange.Invoke(value);
            }
        }
    }

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
    }

    internal void NotifyMouseUp(MouseInteractable obj)
    {
        IsMouseDown = false;

        if (!IsMouseDragging)
        {
            if (TimeUtility.localTimeInMilisecond - MouseDownTime > mouseFocusThreshold)
                OnObjectFocus.Invoke(obj);
            else
                OnObjectClicked.Invoke(obj);
        }
        else
        {
            IsMouseDragging = false;
            OnEndDragging.Invoke(obj);
        }

        //OnMouseUp.Invoke(obj);
    }
}
