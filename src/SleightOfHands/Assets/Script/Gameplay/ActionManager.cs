using System.Collections.Generic;

/// <summary>
/// A queue system which will continueously execute the first action in the queue until the queue becomes empty
/// </summary>
public class ActionManager
{
    private static readonly ActionManager singleton = new ActionManager();

    /// <summary>
    /// The unique instance
    /// </summary>
    public static ActionManager Singleton
    {
        get
        {
            return singleton;
        }
    }

    private ActionQueue actionQueue;
    private Dictionary<Action, System.Action> callbacks;

    private Action actionInExecution;

    private ActionManager()
    {
        actionQueue = new ActionQueue();
        callbacks = new Dictionary<Action, System.Action>();
    }

    /// <summary>
    /// Add a new action to be executed immediately
    /// </summary>
    /// <param name="action"> The action to be added </param>
    internal void AddFront(Action action, System.Action callback = null)
    {
        actionQueue.PushFront(action);

        if (callback != null)
            callbacks.Add(action, callback);

        if (actionInExecution == null)
            Execute();
    }

    /// <summary>
    /// Add a new action to be executed at last
    /// </summary>
    /// <param name="action"> The action to be added </param>
    internal void AddBack(Action action, System.Action callback = null)
    {
        actionQueue.PushBack(action);

        if (callback != null)
            callbacks.Add(action, callback);

        if (actionInExecution == null)
            Execute();
    }
    
    internal void Clear()
    {
        actionQueue.Clear();
    }

    /// <summary>
    /// Execute the first action in the queue if the queue is not empty or execute assigned callback function if it exists
    /// </summary>
    private void Execute()
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log(LogUtility.MakeLogString("ActionManager", "Execute\n" + actionQueue));
#endif

        if (actionInExecution != null && callbacks.ContainsKey(actionInExecution))
        {
            callbacks[actionInExecution].Invoke();
            callbacks.Remove(actionInExecution);
        }

        if (!actionQueue.IsEmpty())
        {
            actionInExecution = actionQueue.Pop();
            actionInExecution.Execute(Execute);
        }
        else
            actionInExecution = null;
    }

    public void Reset()
    {
        actionQueue = new ActionQueue();
        callbacks = new Dictionary<Action, System.Action>();
    }
}
