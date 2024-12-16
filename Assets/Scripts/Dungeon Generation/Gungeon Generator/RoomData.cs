using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomData
{
    public int gridX;
    public int gridY;
    public RoomType roomType;

    public bool connectUp;
    public bool connectDown;
    public bool connectLeft;
    public bool connectRight;

    public RoomData(int x, int y, RoomType type)
    {
        this.gridX = x;
        this.gridY = y;
        this.roomType = type;
    }
}
