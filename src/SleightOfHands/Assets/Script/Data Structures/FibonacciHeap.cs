using System;
using System.Collections.Generic;

public class FibonacciHeap<T> where T : IComparable
{
    private FibonacciHeapNode<T> root;

    public int Count { get; private set; }

    public T Minimum
    {
        get
        {
            if (root == null)
                throw new EmptyHeapException("The heap is empty");

            return root.Value;
        }
    }

    public FibonacciHeap()
    {
        Count = 0;
        root = null;
    }

    public override string ToString()
    {
        string s = "";

        FibonacciHeapNode<T> node = root;
        if (node != null)
            do
            {
                s += node.ToString() + " ";

                node = node.right;
            } while (node != root);

        return s;
    }

    public FibonacciHeapNode<T> Push(T value)
    {
        FibonacciHeapNode<T> node = new FibonacciHeapNode<T>(value);

        if (root == null)
            root = node;
        else
        {
            root.Concatenate(node);

            if (node < root)
                root = node;
        }

        Count++;

        return node;
    }

    public FibonacciHeapNode<T> Pop()
    {
        if (Count == 0)
            throw new EmptyHeapException("Cannot pop from an empty heap");

        FibonacciHeapNode<T> oldRoot = root;

        if (Count == 1)
            root = null;
        else
        {
            FibonacciHeapNode<T> node = root.child;
            if (node != null)
            {
                do
                {
                    node.parent = null;
                    node = node.right;
                } while (node != root.child);

                root.child = null;
                root.Concatenate(node);
            }

            FibonacciHeapNode<T> newRoot = root.right;

            root.Extract();

            root = newRoot;

            if (Count > 2)
                Consolidate();
        }

        Count--;

        return oldRoot;
    }

    public bool IsEmpty()
    {
        return Count == 0;
    }

    public void Decrement(FibonacciHeapNode<T> node, T value)
    {
        int comparison = value.CompareTo(node.Value);

        if (comparison == 0)
            return;

        if (comparison > 0)
            throw new ArgumentException("Cannot increment a node value");

        node.Value = value;

        FibonacciHeapNode<T> parent = node.parent;

        if (parent != null && node < parent)
        {
            Cut(node);
            CascadinglyCut(parent);
        }

        if (node < root)
            root = node;
    }

    public void Union(FibonacciHeap<T> other)
    {
        if (other == null || other.Count == 0)
            return;

        if (Count == 0)
            root = other.root;
        else
        {
            root.Concatenate(other.root);

            if (other.root < root)
                root = other.root;

            Count += other.Count;
        }
    }

    private void Consolidate()
    {
        Dictionary<int, FibonacciHeapNode<T>> map = new Dictionary<int, FibonacciHeapNode<T>>();

        Queue<FibonacciHeapNode<T>> q = new Queue<FibonacciHeapNode<T>>();
        FibonacciHeapNode<T> node = root;
        do
        {
            q.Enqueue(node);
            node = node.right;
        } while (node != root);

        while (q.Count > 0)
        {
            node = q.Peek();
            q.Dequeue();

            int degree = node.Degree;

            //LogUtility.PrintLog("FHeap", string.Format("degree = {0}: Find a node [ {1} -{2}-> ]", degree, node.value, node.degree));
            while (map.ContainsKey(degree))
            {
                FibonacciHeapNode<T> newChild = map[degree];
                //LogUtility.PrintLog("FHeap", string.Format("degree = {0}: Find another node [ {1} -{2}-> ]", degree, newChild.value, newChild.degree));
                if (newChild < node)
                {
                    //LogUtility.PrintLog("FHeap", string.Format("degree = {0}: Switch", degree));
                    FibonacciHeapNode<T> t = node;
                    node = newChild;
                    newChild = t;
                }

                Link(node, newChild);
                //LogUtility.PrintLog("FHeap", string.Format("degree = {0}: Link [ {1} -{2}-> ] to [ {3} -{4}-> ]", degree, newChild.value, newChild.degree, node.value, node.degree));
                map.Remove(degree);
                //LogUtility.PrintLog("FHeap", string.Format("degree = {0}: Remove degree {0} from map", degree));
                degree++;
                //LogUtility.PrintLog("FHeap", string.Format("degree = {0}: Add degree", degree));
            }

            map.Add(degree, node);
        }

        FibonacciHeapNode<T> min = root;
        node = root.right;
        while (node != root)
        {
            if (node < min)
                min = node;

            node = node.right;
        }

        root = min;
    }

    private void Link(FibonacciHeapNode<T> parent, FibonacciHeapNode<T> child)
    {
        child.Extract();

        child.parent = parent;

        if (parent.child == null)
            parent.child = child;
        else
            parent.child.Concatenate(child);

        parent.Degree++;

        child.isMarked = false;

        if (root == child)
            root = parent;
    }

    private void Cut(FibonacciHeapNode<T> node)
    {
        FibonacciHeapNode<T> parent = node.parent;

        if (parent.child == node)
        {
            FibonacciHeapNode<T> newChild = node.right;
            if (newChild == node)
                parent.child = null;
            else
                parent.child = newChild;
        }

        node.Extract();
        node.parent = null;
        node.isMarked = false;

        parent.Degree--;

        root.Concatenate(node);
    }

    private void CascadinglyCut(FibonacciHeapNode<T> node)
    {
        FibonacciHeapNode<T> parent = node.parent;

        if (parent != null)
        {
            if (!parent.isMarked)
                parent.isMarked = true;
            else
            {
                Cut(node);
                CascadinglyCut(parent);
            }
        }
    }
}
