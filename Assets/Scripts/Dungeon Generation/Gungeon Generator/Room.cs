using UnityEngine;

public class Room
{
    public Vector2Int position;
    public Vector2Int size;
    public int margin;

    // Propiedades para obtener los límites de la habitación
    public int xMin { get { return position.x; } }
    public int xMax { get { return position.x + size.x; } }
    public int yMin { get { return position.y; } }
    public int yMax { get { return position.y + size.y; } }

    // Centro de la habitación
    public Vector2Int center
    {
        get
        {
            return new Vector2Int(position.x + size.x / 2, position.y + size.y / 2);
        }
    }

    // Constructor
    public Room(Vector2Int position, Vector2Int size, int margin)
    {
        this.position = position;
        this.size = size;
        this.margin = margin;
    }

    // Método para comprobar solapamiento con otra habitación, considerando el margen
    public bool Overlaps(Room other)
    {
        return !(xMax + margin < other.xMin - margin ||
                 xMin - margin > other.xMax + margin ||
                 yMax + margin < other.yMin - margin ||
                 yMin - margin > other.yMax + margin);
    }
}