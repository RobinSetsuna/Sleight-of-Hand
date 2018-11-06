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
    private System.Action executionCallback;

    private ActionManager()
    {
        actionQueue = new ActionQueue();
    }

    /// <summary>
    /// Add a new action into the system
    /// </summary>
    /// <param name="action"> The action to be added </param>
    internal void Add(Action action)
    {
        actionQueue.PushBack(action);
    }

    /// <summary>
    /// Construct and add a new action into the system
    /// </summary>
    /// <param name="callback"> The callback function for the new action to be added </param>
    /// <returns> The new action constructed and added </returns>
    internal Action AddNewAction(System.Action<System.Action> callback)
    {
        return actionQueue.PushBack(callback);
    }

    /// <summary>
    /// Assign a callback to be executed after all actions are executed and start to execute existing actions
    /// </summary>
    /// <param name="callback"></param>
    internal void Execute(System.Action callback)
    {
        executionCallback = callback;
        Execute();
    }

    /// <summary>
    /// Execute the first action in the queue if the queue is not empty or execute assigned callback function if it exists
    /// </summary>
    internal void Execute()
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log(actionQueue);
#endif
        if (!actionQueue.IsEmpty())
            actionQueue.Pop().Execute(Execute);
        else if (executionCallback != null)
        {
            executionCallback();
            executionCallback = null;
        }
    }
}
