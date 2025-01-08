using UnityEngine;

public enum RoomConnections
{
    None = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3
}

[System.Serializable]
public class RoomData
{
    public int gridX;
    public int gridY;
    public RoomType roomType;

    public RoomConnections connections;

    public GameObject[] enemyPrefabs;
    public Vector2[] spawnPoints;

    public RoomData(int x, int y, RoomType type)
    {
        this.gridX = x;
        this.gridY = y;
        this.roomType = type;
    }
}
