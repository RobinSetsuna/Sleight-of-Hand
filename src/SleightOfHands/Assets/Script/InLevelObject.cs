using UnityEngine;

/// <summary>
///                       p
///                       l
///                       a
///                       y
///                       e
///                       r
///                       |
/// 0000 0000 0000 0000 0000 0000 0000 0000
///                      | |
///                      e o
///                      n b
///                      e s
///                      m t
///                      y a
///                        c
///                        l
///                        e
/// </summary>
public enum InLevelObjectMask : int
{
    Obstacle = 0x100,
    Player = 0x200,
    Enemy = 0x400,
}

public abstract class InLevelObject : MonoBehaviour
{
    //private Tile[] tilesOccupied;
    //public Tile Anchor { get; private set; }
}
