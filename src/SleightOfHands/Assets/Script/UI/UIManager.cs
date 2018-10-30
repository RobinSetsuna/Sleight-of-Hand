using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager singleton;
    public static UIManager Singleton
    {
        get
        {
            if (!singleton)
                singleton = new GameObject("UI Manager").AddComponent<UIManager>();

            return singleton;
        }
    }

    public enum UIMode : int
    {
        DEFAULT = 0,
        PERMANENT,
    }

    private Stack<string> uiStack;
    private Dictionary<string, UserInterface> uiOpened;

    private bool isCancelButtonDown = false;

    public bool IsInViewport(string name)
    {
        return uiOpened.ContainsKey(name);
    }

    public bool IsViewportClear()
    {
        return uiStack.Count != 0;
    }

    public UserInterface Open(string name, UIMode mode = UIMode.DEFAULT, params object[] args)
    {
#if UNITY_EDITOR
        LogUtility.PrintLog("UI", IsInViewport(name) ? name + " is already in viewport" : "Open " + name);
#endif

        if (IsInViewport(name))
            return uiOpened[name];

        UserInterface ui = Instantiate(ResourceUtility.GetUIPrefab(name), transform, false);

        uiOpened.Add(name, ui);

        ui.OnOpen(args);

        if (mode != UIMode.PERMANENT)
            uiStack.Push(name);

        return ui;
    }

    public void Close(string name)
    {
#if UNITY_EDITOR
        LogUtility.PrintLog("UI", IsInViewport(name) ? "Close " + name : name + " is not in viewport");
#endif

        if (IsInViewport(name))
        {
            UserInterface ui = uiOpened[name];

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

    public void Toggle(string name)
    {
        if (uiOpened.ContainsKey(name))
            Close(name);
        else
            Open(name);
    }

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
        uiOpened = new Dictionary<string, UserInterface>();
    }

    //void FixedUpdate()
    //{
    //    if (Input.GetAxis("Cancel") == 0)
    //        isCancelButtonDown = false;
    //    else if (!isCancelButtonDown)
    //    {
    //        isCancelButtonDown = true;

    //        if (HasUIOpened())
    //        {
    //            Close(uiStack.Peek());
    //        }
    //        else
    //        {
    //            Open("IngameMenu");
    //        }
    //    }
    //}
}
