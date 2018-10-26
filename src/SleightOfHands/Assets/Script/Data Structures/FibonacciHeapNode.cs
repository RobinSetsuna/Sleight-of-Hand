using System;

public class FibonacciHeapNode<T> : IComparable where T : IComparable
{
    protected internal bool isMarked;

    protected internal FibonacciHeapNode<T> left;
    protected internal FibonacciHeapNode<T> right;
    protected internal FibonacciHeapNode<T> parent;
    protected internal FibonacciHeapNode<T> child;

    public T Value { get; protected internal set; }
    public int Degree { get; protected internal set; }

    public FibonacciHeapNode(T value)
    {
        Value = value;

        Degree = 0;
        isMarked = false;

        left = this;
        right = this;
        parent = null;
        child = null;
    }

    public void Concatenate(FibonacciHeapNode<T> other)
    {
        if (other == null)
            return;

        FibonacciHeapNode<T> node = other.left;

        left.right = other;
        other.left = left;

        left = node;
        node.right = this;
    }

    public void Extract()
    {
        left.right = right;
        right.left = left;

        left = this;
        right = this;
    }

    public override string ToString()
    {
        string s = " ";

        FibonacciHeapNode<T> node = child;
        if (node != null)
            do
            {
                s += node.ToString() + " ";

                node = node.right;
            } while (node != child);

        return string.Format("[ {0} -{1}->{2}]", Value, Degree, s);
    }

    public int CompareTo(FibonacciHeapNode<T> other)
    {
        return Value.CompareTo(other.Value);
    }

    int IComparable.CompareTo(object obj)
    {
        return CompareTo((FibonacciHeapNode<T>)obj);
    }

    public static bool operator >(FibonacciHeapNode<T> lhs, FibonacciHeapNode<T> rhs)
    {
        return lhs.CompareTo(rhs) > 0;
    }

    public static bool operator <(FibonacciHeapNode<T> lhs, FibonacciHeapNode<T> rhs)
    {
        return lhs.CompareTo(rhs) < 0;
    }
}
