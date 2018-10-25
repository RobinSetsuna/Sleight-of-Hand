using System.Collections.Generic;
using UnityEngine;

public class ActionQueue : MonoBehaviour
{
    private LinkedList<Action> actions;
    private LinkedListNode<Action> current;

    public ActionQueue()
    {
        actions = new LinkedList<Action>();
        actions.AddLast(new LinkedListNode<Action>(null));

        current = actions.Last;
    }

    public bool IsEmpty()
    {
        return current == actions.Last;
    }

    public void PushFront(Action action)
    {
        Debug.Log("[ActionQueue] PushFront: " + action.ToString());

        current = actions.AddBefore(current, action);
    }

    public Action PushFront(System.Action<System.Action> callback)
    {
        Action action = new Action(callback);

        PushFront(action);

        return action;
    }

    public void PushBack(Action action)
    {
        if (current == actions.Last)
            PushFront(action);
        else
        {
            Debug.Log("[ActionQueue] PushBack: " + action.ToString());

            actions.AddBefore(actions.Last, action);
        }
    }

    public Action PushBack(System.Action<System.Action> callback)
    {
        Action action = new Action(callback);

        PushBack(action);

        return action;
    }

    public Action Pop()
    {
        if (IsEmpty())
            return null;

        Action action = current.Value;

        current = current.Next;

        return action;
    }

    public override string ToString()
    {
        string s = "";

        for (LinkedListNode<Action> node = actions.First; node != actions.Last; node = node.Next)
        {
            if (node == current)
                s += " -> **" + node.Value.ToString() + "**";
            else
                s += " -> " + node.Value.ToString();
        }

        s = s.Substring(4);

        return s;
    }
}
