using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A system which detects all mouse inputs on object with a MouseInteractable component and turns them into individual events
/// </summary>
public class MouseInputManager
{
    private static readonly MouseInputManager singleton = new MouseInputManager();

    /// <summary>
    /// The unique instance
    /// </summary>
    public static MouseInputManager Singleton
    {
        get
        {
            return singleton;
        }
    }

    /// <summary>
    /// [read-only] The minimum time interval required for a click to become a focus
    /// </summary>
    public static readonly int mouseFocusThreshold = 200;

    /// <summary>
    /// [read-only] The minimum pixel distance required for a drag
    /// </summary>
    public static readonly float mouseDragThreshold = 10;

    /// <summary>
    /// An event type for MouseInputManager.Singleton.onMouseDrag
    /// </summary>
    public class EventOnMouseDrag : UnityEvent<MouseInteractable, Vector3, Vector3> {}
    
    /// <summary>
    /// An event triggered as long as the mouse is dragging
    /// </summary>
    public EventOnDataUpdate<MouseInteractable> onMouseDrag = new EventOnDataUpdate<MouseInteractable>();

    //public EventOnDataUpdate<MouseInteractable> onMouseDown = new EventOnDataUpdate<MouseInteractable>();
    //public EventOnDataUpdate<MouseInteractable> onMouseUp = new EventOnDataUpdate<MouseInteractable>();

    /// <summary>
    /// An event triggered whenever the mouse is entering a new object on screen
    /// </summary>
    public EventOnDataUpdate<MouseInteractable> onMouseEnter = new EventOnDataUpdate<MouseInteractable>();

    /// <summary>
    /// An event triggered whenever a mouse focus (a long click) is performed
    /// </summary>
    public EventOnDataUpdate<MouseInteractable> onMouseFocus = new EventOnDataUpdate<MouseInteractable>();

    /// <summary>
    /// An event triggered whenever a mouse click is performed
    /// </summary>
    public EventOnDataUpdate<MouseInteractable> onMouseClick = new EventOnDataUpdate<MouseInteractable>();

    /// <summary>
    /// An event triggered whenever a mouse drag ends
    /// </summary>
    public EventOnDataUpdate<MouseInteractable> onMouseDragEnd = new EventOnDataUpdate<MouseInteractable>();

    /// <summary>
    /// Whether the mouse is currently pressed
    /// </summary>
    public bool IsMouseDown { get; private set; }

    /// <summary>
    /// When the mouse was pressed last time
    /// </summary>
    public long MouseDownTime { get; private set; }

    /// <summary>
    /// Where the mouse was pressed last time
    /// </summary>
    public Vector3 MouseDownPosition { get; private set; }

    /// <summary>
    /// Whether the mouse is currently dragging
    /// </summary>
    public bool IsMouseDragging { get; private set; }

    /// <summary>
    /// The object which the mouse is currently on
    /// </summary>
    public MouseInteractable CurrentMouseTarget { get; private set; }

    private MouseInputManager() {}

    internal void NotifyMouseDown(MouseInteractable obj)
    {
        IsMouseDown = true;
        MouseDownTime = TimeUtility.localTimeInMilisecond;
        MouseDownPosition = Input.mousePosition;

        //onMouseDown.Invoke(obj);
    }

    internal void NotifyMouseDrag(MouseInteractable obj)
    {
        if (!IsMouseDragging)
        {
            if (Vector3.Distance(Input.mousePosition, MouseDownPosition) > mouseDragThreshold)
            {
#if UNITY_EDITOR
                Debug.Log(LogUtility.MakeLogString("MouseInputManager", "Start dragging"));
#endif
                IsMouseDragging = true;
            }
        }
        else
            onMouseDrag.Invoke(obj);
    }

    internal void NotifyMouseEnter(MouseInteractable obj)
    {
        if (!IsMouseDragging && IsMouseDown)
        {
#if UNITY_EDITOR
            Debug.Log(LogUtility.MakeLogString("MouseInputManager", "Start dragging"));
#endif
            IsMouseDragging = true;
        }

        CurrentMouseTarget = obj;

        onMouseEnter.Invoke(obj);
    }

    internal void NotifyMouseUp(MouseInteractable obj)
    {
        IsMouseDown = false;

        if (!IsMouseDragging)
        {
            if (TimeUtility.localTimeInMilisecond - MouseDownTime > mouseFocusThreshold)
            {
#if UNITY_EDITOR
                Debug.Log(LogUtility.MakeLogString("MouseInputManager", "Focus " + obj));
#endif
                onMouseFocus.Invoke(obj);
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log(LogUtility.MakeLogString("MouseInputManager", "Click " + obj));
#endif
                onMouseClick.Invoke(obj);
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log(LogUtility.MakeLogString("MouseInputManager", "End dragging (started from " + obj + ")"));
#endif
            IsMouseDragging = false;
            onMouseDragEnd.Invoke(obj);
        }

        //onMouseUp.Invoke(obj);
    }
}
