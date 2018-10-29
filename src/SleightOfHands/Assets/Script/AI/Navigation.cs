using System;
using System.Collections.Generic;
using UnityEngine;

public struct Navigation
{
    private struct AStarTile<T> : IComparable where T : IEquatable<T>
    {
        public Vector2Int indices;
        public Vector2Int previous;
        public float f, g, h;

        public int CompareTo(AStarTile<T> other)
        {
            return f.CompareTo(other.f);
        }

        public int CompareTo(object obj)
        {
            return CompareTo((AStarTile<T>)obj);
        }
    }

    public static Path<T> FindPath<T>(INavGrid<T> navGrid, T start, T destination) where T : IEquatable<T>
    {
        Vector2Int startIndices = navGrid.GetIndices(start);
        Vector2Int destinationIndices = navGrid.GetIndices(destination);

        if (!navGrid.IsAccessible(startIndices.x, startIndices.y) || !navGrid.IsAccessible(destinationIndices.x, destinationIndices.y))
            return null;

        if (start.Equals(destination))
            return new Path<T>(start);

        int length = navGrid.Length;
        int width = navGrid.Width;

        bool[,] closedList = new bool[length, width];

        AStarTile<T>[,] tiles = new AStarTile<T>[length, width];

        for (int x = 0; x < length; x++)
            for (int y = 0; y < width; y++)
            {
                tiles[x, y].indices = new Vector2Int(x, y);
                tiles[x, y].previous = new Vector2Int(-1, -1);
                tiles[x, y].f = float.MaxValue;
                tiles[x, y].g = float.MaxValue;
                tiles[x, y].h = float.MaxValue;
            }

        FibonacciHeap<AStarTile<T>> openList = new FibonacciHeap<AStarTile<T>>();
        Dictionary<Vector2Int, FibonacciHeapNode<AStarTile<T>>> openListMap = new Dictionary<Vector2Int, FibonacciHeapNode<AStarTile<T>>>();

        tiles[startIndices.x, startIndices.y].indices = startIndices;
        tiles[startIndices.x, startIndices.y].previous = startIndices;
        tiles[startIndices.x, startIndices.y].f = 0;
        tiles[startIndices.x, startIndices.y].g = 0;
        tiles[startIndices.x, startIndices.y].h = 0;

        openListMap.Add(startIndices, openList.Push(tiles[startIndices.x, startIndices.y]));

        while (!openList.IsEmpty())
        {
            AStarTile<T> current = openList.Pop().Value;

            Vector2Int currentIndices = current.indices;
            int x = currentIndices.x;
            int y = currentIndices.y;

            closedList[x, y] = true;

            List<Vector2Int> adjacentIndices = navGrid.GetAdjacentIndices(x, y);
            for (int i = 0; i < adjacentIndices.Count; i++)
            {
                Vector2Int neighborIndices = adjacentIndices[i];

                int xi = neighborIndices.x;
                int yi = neighborIndices.y;

                if (neighborIndices == destinationIndices)
                {
                    tiles[xi, yi].previous = currentIndices;

                    LinkedList<T> wayPoints = new LinkedList<T>();

                    while (neighborIndices != startIndices)
                    {
                        wayPoints.AddFirst(navGrid.GetTile(neighborIndices));
                        neighborIndices = tiles[neighborIndices.x, neighborIndices.y].previous;
                    }
                    
                    return new Path<T>(start, wayPoints);
                }

                if (closedList[xi, yi] || !navGrid.IsAccessible(xi, yi))
                    continue;

                float gNew = current.g + MathUtility.ManhattanDistance(xi, yi, x, y);
                float hNew = MathUtility.ManhattanDistance(xi, yi, destinationIndices.x, destinationIndices.y);
                float fNew = gNew + hNew;

                if (tiles[xi, yi].f < fNew)
                    continue;

                tiles[xi, yi].previous = currentIndices;
                tiles[xi, yi].g = gNew;
                tiles[xi, yi].h = hNew;
                tiles[xi, yi].f = fNew;

                if (!openListMap.ContainsKey(neighborIndices))
                    openListMap.Add(neighborIndices, openList.Push(tiles[xi, yi]));
                else
                    openList.Decrement(openListMap[neighborIndices], tiles[xi, yi]);
            }
        }

        return null;
    }
}
