using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room
{
    public RectInt Rect;
    public Vector2Int Center => new(Rect.x + Rect.width / 2, Rect.y + Rect.height / 2);
    public RoomType Type;
    public List<Room> ConnectedRooms = new();

    public Room(RectInt rect)
    {
        Rect = rect;
        Type = RoomType.Normal;
    }
}

public enum RoomType
{
    Start,
    End,
    Boss,
    Loot,
    Normal
}
