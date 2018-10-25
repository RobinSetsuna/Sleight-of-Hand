﻿using System;
using System.Collections.Generic;

public class Path<T> : IEquatable<Path<T>> where T : IEquatable<T>
{
    private LinkedList<T> wayPoints;
    private LinkedListNode<T> current;

    public T Current
    {
        get
        {
            if (current == null)
                return default(T);

            return current.Value;
        }
    }

    public T Start
    {
        get
        {
            return wayPoints.First.Value;
        }
    }

    public T Destination
    {
        get
        {
            return wayPoints.Last.Value;
        }
    }

    private Path()
    {
        wayPoints = new LinkedList<T>();
        current = null;
    }

    public Path(T start) : this()
    {
        wayPoints.AddFirst(start);
    }

    public Path(T start,  List<T> wayPoints) : this()
    {
        this.wayPoints.AddFirst(start);

        for (int i = 0; i < wayPoints.Count; i++)
            this.wayPoints.AddLast(wayPoints[i]);
    }

    public Path(T start, LinkedList<T> wayPoints) : this()
    {
        this.wayPoints.AddFirst(start);

        foreach (T wayPoint in wayPoints)
            this.wayPoints.AddLast(wayPoint);
    }

    public bool IsFinished()
    {
        return current == null;
    }

    public T Reset()
    {
        current = wayPoints.First.Next;

        return current.Value;
    }

    public T MoveForward()
    {
        if (current == null)
            return default(T);

        current = current.Next;

        if (current == null)
            return default(T);

        return current.Value;
    }

    public T MoveBackward()
    {
        if (current == null)
            return default(T);

        current = current.Previous;
        return current.Value;
    }

    public bool Equals(Path<T> other)
    {
        return Start.Equals(other.Start) && Destination.Equals(other.Destination) && IsSame(wayPoints.First, other.wayPoints.First);
    }

    public override string ToString()
    {
        string s = "";

        foreach (T wayPoint in wayPoints)
            s += " -> " + wayPoint.ToString();

        s = s.Substring(4);

        return s;
    }

    private static bool IsSame(LinkedListNode<T> a, LinkedListNode<T> b)
    {
        if (a == null && b == null)
            return true;

        if (a == null)
            return false;

        if (b == null)
            return false;

        return a.Value.Equals(b.Value) && IsSame(a.Next, b.Next);
    }
}
