using System.Collections.Generic;
using UnityEngine;

public interface INavGrid<T>
{
    int Length { get; }
    int Width { get; }

    Vector2Int GetIndices(T position);
    T GetTile(Vector2Int indices);

    bool IsAccessible(int x, int y);
    List<Vector2Int> GetAdjacentIndices(int x, int y);
}
