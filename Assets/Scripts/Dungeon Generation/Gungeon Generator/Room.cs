using UnityEngine;

public class Room
{
    public Vector2Int position;
    public Vector2Int size;
    public int margin;

    public int xMin => position.x;
    public int xMax => position.x + size.x;
    public int yMin => position.y;
    public int yMax => position.y + size.y;

    public Vector2Int center => new Vector2Int(position.x + size.x / 2, position.y + size.y / 2);

    public Room(Vector2Int position, Vector2Int size, int margin)
    {
        this.position = position;
        this.size = size;
        this.margin = margin;
    }

    public bool Overlaps(Room other)
    {
        return xMin - margin < other.xMax + other.margin &&
               xMax + margin > other.xMin - other.margin &&
               yMin - margin < other.yMax + other.margin &&
               yMax + margin > other.yMin - other.margin;
    }
}