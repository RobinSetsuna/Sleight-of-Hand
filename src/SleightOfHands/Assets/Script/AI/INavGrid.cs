using System.Collections.Generic;
using UnityEngine;

public interface INavGrid<T>
{
    int Length { get; }
    int Width { get; }

    Vector2Int GetGridPosition(T tile);
    T GetTile(Vector2Int gridPosition);

    bool IsAccessible(int x, int y);
    List<Vector2Int> GetAdjacentGridPositions(int x, int y);
}
