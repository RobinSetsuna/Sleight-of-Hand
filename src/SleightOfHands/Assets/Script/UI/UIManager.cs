using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager singleton;

    /// <summary>
    /// The unique instance
    /// </summary>
    public static UIManager Singleton
    {
        get
        {
            if (!singleton)
                singleton = new GameObject("UI Manager").AddComponent<UIManager>();

            return singleton;
        }
    }

    /// <summary>
    /// The way to open the new window
    /// </summary>
    public enum UIMode : int
    {
        /// <summary>
        /// The window should be closed in a short period of time, therefore making the viewport not clear when it is not closed
        /// </summary>
        DEFAULT = 0,

        /// <summary>
        /// The windows is considered to be a part of the viewport
        /// </summary>
        PERMANENT,
    }

    private Stack<string> uiStack;
    private Dictionary<string, UIWindow> uiOpened;

    private bool isCancelButtonDown = false;

    /// <summary>
    /// Whether the UI window is opened in the viewport (not considering the UIMode)
    /// </summary>
    /// <param name="name"> The name of the UI window </param>
    /// <returns> Whether the window is opened in the viewport </returns>
    public bool IsInViewport(string name)
    {
        return uiOpened.ContainsKey(name);
    }

    /// <summary>
    /// Whether any UI windows are opened with UIMode.DEFAULT
    /// </summary>
    /// <returns> Whether any UI windows are opened with UIMode.DEFAULT </returns>
    public bool IsViewportClear()
    {
        return uiStack.Count != 0;
    }

    /// <summary>
    /// Open a new UI window
    /// </summary>
    /// <param name="name"> The name of the UI window to be opened </param>
    /// <param name="mode"> The mode to be used to open the UI window </param>
    /// <param name="args"> Extra arguments passed to UIWindow.OnOpen() </param>
    /// <returns></returns>
    public UIWindow Open(string name, UIMode mode = UIMode.DEFAULT, params object[] args)
    {
#if UNITY_EDITOR
        LogUtility.PrintLog("UI", IsInViewport(name) ? name + " is already in viewport" : "Open " + name);
#endif

        if (IsInViewport(name))
            return uiOpened[name];

        UIWindow ui = Instantiate(ResourceUtility.GetUIPrefab(name), transform, false);

        uiOpened.Add(name, ui);

        ui.OnOpen(args);

        if (mode != UIMode.PERMANENT)
            uiStack.Push(name);

        return ui;
    }

    /// <summary>
    /// Close a previously opened UI window
    /// </summary>
    /// <param name="name"> The name of the UI window to be closed </param>
    public void Close(string name)
    {
#if UNITY_EDITOR
        LogUtility.PrintLog("UI", IsInViewport(name) ? "Close " + name : name + " is not in viewport");
#endif

        if (IsInViewport(name))
        {
            UIWindow ui = uiOpened[name];

            ui.OnClose();

            Stack<string> s = new Stack<string>();

            while (uiStack.Peek().CompareTo(name) != 0)
                s.Push(uiStack.Pop());

            uiStack.Pop();

            while (s.Count > 0)
                uiStack.Push(s.Pop());

            Destroy(ui.gameObject);

            uiOpened.Remove(name);
        }
    }

    /// <summary>
    /// Open the named UI window if it is not opened or close it if it was previously opened
    /// </summary>
    /// <param name="name"> The name of the UI window to be toggled </param>
    public void Toggle(string name)
    {
        if (uiOpened.ContainsKey(name))
            Close(name);
        else
            Open(name);
    }

    /// <summary>
    /// Convert pixel position to canvas position (can be used to assign transform.localPosition)
    /// </summary>
    /// <param name="position"> The pixel position to be converted </param>
    /// <returns> The converted canvas position </returns>
    public Vector3 GetCanvasPosition(Vector3 position)
    {
        Vector2 referenceResolution = GetComponent<CanvasScaler>().referenceResolution;
        return new Vector3((position.x / Screen.width - 0.5f) * referenceResolution.x, (position.y / Screen.height - 0.5f) * referenceResolution.y, 0);
    }

    void Awake()
    {
        if (!singleton)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        uiStack = new Stack<string>();
        uiOpened = new Dictionary<string, UIWindow>();
    }
}
