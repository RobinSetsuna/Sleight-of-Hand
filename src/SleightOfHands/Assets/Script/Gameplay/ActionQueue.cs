using System.Collections.Generic;

/// <summary>
/// A specialized data structure for Action implemented in a LinkedList with a queue behaviour
/// </summary>
public class ActionQueue
{
    private LinkedList<Action> actions;
    private LinkedListNode<Action> current;

    /// <summary>
    /// Construct an empty queue
    /// </summary>
    public ActionQueue()
    {
        actions = new LinkedList<Action>();
        actions.AddLast(new LinkedListNode<Action>(null));

        current = actions.Last;
    }

    /// <summary>
    /// Whether the queue is empty
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return current == actions.Last;
    }

    /// <summary>
    /// Add a new action to the front of the queue
    /// </summary>
    /// <param name="action"> The action to be added </param>
    public void PushFront(Action action)
    {
        current = actions.AddBefore(current, action);
    }

    /// <summary>
    /// Construct and add a new action to the front of the queue
    /// </summary>
    /// <param name="callback"> The callback function for the action to be added </param>
    /// <returns> The new action constructed and added </returns>
    public Action PushFront(System.Action<System.Action> callback)
    {
        Action action = new Action(callback);

        PushFront(action);

        return action;
    }

    /// <summary>
    /// Add a new action from the back of the queue
    /// </summary>
    /// <param name="action"> The action to be added </param>
    public void PushBack(Action action)
    {
        if (current == actions.Last)
            PushFront(action);
        else
            actions.AddBefore(actions.Last, action);
    }

    /// <summary>
    /// Construct and add a new action from the back of the queue
    /// </summary>
    /// <param name="callback"> The callback function for the action to be added </param>
    /// <returns> The new action constructed and added </returns>
    public Action PushBack(System.Action<System.Action> callback)
    {
        Action action = new Action(callback);

        PushBack(action);

        return action;
    }

    /// <summary>
    /// Remove the first action in the queue
    /// </summary>
    /// <returns> The removed action </returns>
    public Action Pop()
    {
        if (IsEmpty())
            return null;

        Action action = current.Value;

        current = current.Next;

        return action;
    }

    public void Clear()
    {
        if (!IsEmpty())
        {
            while (current != actions.Last)
                actions.RemoveLast();
        }
    }

    public override string ToString()
    {
        string s = "";

        for (LinkedListNode<Action> node = actions.First; node != actions.Last; node = node.Next)
                s += (node == current ? "->" : "  ") + node.Value.ToString() + "\n";

        return s;
    }
}
