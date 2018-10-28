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

    public EventOnDataUpdate<MouseInteractable> OnCurrentMouseTargetChange = new EventOnDataUpdate<MouseInteractable>();
    public EventOnDataUpdate<MouseInteractable> OnObjectClicked = new EventOnDataUpdate<MouseInteractable>();
    public EventOnDataUpdate<MouseInteractable> OnEndDragging = new EventOnDataUpdate<MouseInteractable>();

    public bool IsMouseDown { get; private set; }
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
        IsMouseDown = true;
        MouseDownTime = TimeUtility.localTimeInMilisecond;
    }

    internal void NotifyMouseDrag(MouseInteractable obj)
    {
        if (!IsMouseDragging)
            IsMouseDragging = TimeUtility.localTimeInMilisecond - MouseDownTime > mouseDragThreshold;
    }

    internal void NotifyMouseEnter(MouseInteractable obj)
    {
        if (IsMouseDown)
            IsMouseDragging = true;

        CurrentMouseTarget = obj;
    }

    internal void NotifyMouseUp(MouseInteractable obj)
    {
        IsMouseDown = false;

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
