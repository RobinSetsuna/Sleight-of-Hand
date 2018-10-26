using System.Collections.Generic;
using UnityEngine;

public class NavGrid : MonoBehaviour, INavGrid<Vector3>
{
    [SerializeField] private float xMin;
    [SerializeField] private float yMin;
    [SerializeField] private float xMax;
    [SerializeField] private float yMax;

    [SerializeField] private float tileSize;

    private float[,] heights;

    public int Length { get; private set; }

    public int Width { get; private set; }

    public NavGrid(float xMin, float yMin, float xMax, float yMax, float tileSize)
    {
        this.xMin = xMin;
        this.yMin = yMin;
        this.xMax = xMax;
        this.yMax = yMax;
        this.tileSize = tileSize;

        Refresh();
    }

    public Vector2Int GetIndices(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt((position.x - xMin) / tileSize), Mathf.FloorToInt((position.z - yMin) / tileSize));
    }

    public Vector3 GetTile(Vector2Int indices)
    {
        return new Vector3(tileSize * (indices.x + 0.5f), heights[indices.x, indices.y], tileSize * (indices.y + 0.5f));
    }

    public bool IsAccessible(int x, int y)
    {
        return x >= 0 && x < Length && y >= 0 && y < Width && heights[x, y] != float.MinValue;
    }

    public bool IsAccessible(Vector3 position)
    {
        Vector2Int indices = GetIndices(position);
        return IsAccessible(indices.x, indices.y);
    }

    public List<Vector2Int> GetAdjacentIndices(int x, int y)
    {
        float h = heights[x, y];

        List<Vector2Int> indices = new List<Vector2Int>();

        for (int dx = -1; dx < 2; dx++)
            for (int dy = -1; dy < 2; dy++)
            {
                int xi = x + dx;
                int yi = y + dy;

                if (x != 0 && y != 0 && IsAccessible(xi, yi) && Mathf.Abs(heights[xi, yi] - h) < 0.3f)
                    indices.Add(new Vector2Int(xi, yi));
            }

        return indices;
    }

    void OnEnable()
    {
        Refresh();
    }

    #if UNITY_EDITOR
    float[,] gizmoHeights;
    //private Path<Vector3> path = null;
    void OnDrawGizmos()
    {
        for (int x = 0; x < Length; x++)
            for (int y = 0; y < Width; y++)
            {
                float h = heights[x, y];
                Gizmos.color = h == float.MinValue ? Color.red : Color.green;
                Gizmos.DrawWireCube(new Vector3(tileSize * (x + 0.5f), gizmoHeights[x, y], tileSize * (y + 0.5f)), new Vector3(tileSize, 0, tileSize));
            }

        //if (path == null)
        //{
        //    RandomNumberGenerator rng = new RandomNumberGenerator(TimeUtility.localTime);

        //    Vector3 start = new Vector3(rng.GenerateRandomFloat(0, 500), 0, rng.GenerateRandomFloat(250, 500));
        //    while (!IsAccessible(start))
        //        start = new Vector3(rng.GenerateRandomFloat(0, 500), 0, rng.GenerateRandomFloat(250, 500));

        //    Vector3 destination = new Vector3(rng.GenerateRandomFloat(0, 500), 0, rng.GenerateRandomFloat(0, 250));
        //    while (!IsAccessible(destination))
        //        destination = new Vector3(rng.GenerateRandomFloat(0, 500), 0, rng.GenerateRandomFloat(0, 250));

        //    path = Navigation.FindPath(this, start, destination);
        //}
        //else
        //{
        //    path.Reset();

        //    Gizmos.color = Color.blue;
        //    for (Vector3 point = path.Current; point != default(Vector3); point = path.MoveForward())
        //        Gizmos.DrawWireCube(point, new Vector3(tileSize, 0, tileSize));
        //}
    }
    #endif

    private void Refresh()
    {
        Length = Mathf.FloorToInt((xMax - xMin) / tileSize);
        Width = Mathf.FloorToInt((yMax - yMin) / tileSize);

#if UNITY_EDITOR
        float[,] realHeights = new float[Length + 1, Width + 1];
#endif

        float[,] rawHeights = new float[Length + 1, Width + 1];
        for (int x = 0; x < Length + 1; x++)
            for (int y = 0; y < Width + 1; y++)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(tileSize * x, 500, tileSize * y), Vector3.down, out hit))
                    rawHeights[x, y] = hit.transform.tag == "Unwalkable" ? float.MinValue : hit.point.y;
                else
                    rawHeights[x, y] = float.MinValue;

#if UNITY_EDITOR
                realHeights[x, y] = hit.point.y;
#endif
            }

#if UNITY_EDITOR
        gizmoHeights = new float[Length, Width];
#endif

        heights = new float[Length, Width];
        for (int x = 0; x < Length; x++)
            for (int y = 0; y < Width; y++)
            {
                float minH = Mathf.Min(Mathf.Min(rawHeights[x, y], Mathf.Min(x, y + 1)), Mathf.Min(Mathf.Min(x + 1, y), Mathf.Min(x + 1, y + 1)));

#if UNITY_EDITOR
                gizmoHeights[x, y] = (realHeights[x, y] + realHeights[x, y + 1] + realHeights[x + 1, y] + realHeights[x + 1, y + 1]) / 4;
#endif

                if (minH == float.MinValue)
                    heights[x, y] = float.MinValue;
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(new Vector3(tileSize * (x + 0.5f), 500, tileSize * (y + 0.5f)), Vector3.down, out hit))
                    {
                        heights[x, y] = hit.transform.tag == "Unwalkable" ? float.MinValue : hit.point.y;
                        
#if UNITY_EDITOR
                        if (hit.transform.tag != "Unwalkable")
                            gizmoHeights[x, y] = hit.point.y;
#endif
                    }
                    else
                        heights[x, y] = float.MinValue;
                }
            }
    }
}
