public class ActionManager
{
    private static readonly ActionManager singleton = new ActionManager();
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

    internal void Add(Action action)
    {
        actionQueue.PushBack(action);
    }

    internal Action AddNewAction(System.Action<System.Action> callback)
    {
        return actionQueue.PushBack(callback);
    }

    internal void Execute()
    {UnityEngine.Debug.Log(actionQueue);
        if (!actionQueue.IsEmpty())
            actionQueue.Pop().Execute(Execute);
        else if (executionCallback != null)
        {
            executionCallback();
            executionCallback = null;
        }
    }

    internal void Execute(System.Action callback)
    {
        executionCallback = callback;
        Execute();
    }
}
