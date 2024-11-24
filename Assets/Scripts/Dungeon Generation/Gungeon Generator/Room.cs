using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomType
    {
        Spawn,
        End,
        Boss,
        Shop,
        Loot,
        Normal
    }

    public RoomType roomType;
    public Vector2Int gridPosition;

    public Vector2Int roomSize;
}
