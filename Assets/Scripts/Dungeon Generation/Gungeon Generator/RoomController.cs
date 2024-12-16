using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomController : MonoBehaviour
{
    public Tilemap wallTilemap;

    [Header("Door Tiles")]
    public TileBase upDoorTile;
    public TileBase downDoorTile;
    public TileBase leftDoorTile;
    public TileBase rightDoorTile;

    [Header("Wall Tiles")]
    public TileBase upWallTile;
    public TileBase downWallTile;
    public TileBase leftWallTile;
    public TileBase rightWallTile;

    [Header("Room Properties")]
    public int roomWidth = 20;
    public int roomHeight = 20;

    public RoomData data;

    public void Initialize(RoomData roomData)
    {
        this.data = roomData;
        SetDoors(data.connectUp, "up");
        SetDoors(data.connectDown, "down");
        SetDoors(data.connectLeft, "left");
        SetDoors(data.connectRight, "right");

        SpawnContents();
    }

    void SetDoors(bool isConnected, string direction)
    {
        Vector3Int[] doorPositions = GetDoorPositions(direction);

        TileBase tileToUse = GetTileForDirection(direction, isConnected);

        foreach (var pos in doorPositions)
        {
            wallTilemap.SetTile(pos, tileToUse);
        }
    }

    Vector3Int[] GetDoorPositions(string direction)
    {
        int centerX = roomWidth / 2;
        int centerY = roomHeight / 2;

        if (direction == "up")
        {
            return new Vector3Int[] {
            new Vector3Int(centerX - 1, roomHeight - 1, 0),
            new Vector3Int(centerX,     roomHeight - 1, 0)
        };
        }
        else if (direction == "down")
        {
            return new Vector3Int[] {
            new Vector3Int(centerX - 1, 0, 0),
            new Vector3Int(centerX,     0, 0)
        };
        }
        else if (direction == "left")
        {
            return new Vector3Int[] {
            new Vector3Int(0, centerY - 1, 0),
            new Vector3Int(0, centerY,     0)
        };
        }
        else if (direction == "right")
        {
            return new Vector3Int[] {
            new Vector3Int(roomWidth - 1, centerY - 1, 0),
            new Vector3Int(roomWidth - 1, centerY,     0)
        };
        }

        return new Vector3Int[0];
    }

    TileBase GetTileForDirection(string direction, bool isDoor)
    {
        if (isDoor)
        {
            switch (direction)
            {
                case "up":
                    return upDoorTile;
                case "down":
                    return downDoorTile;
                case "left":
                    return leftDoorTile;
                case "right":
                    return rightDoorTile;
            }
        }
        else
        {
            switch (direction)
            {
                case "up":
                    return upWallTile;
                case "down":
                    return downWallTile;
                case "left":
                    return leftWallTile;
                case "right":
                    return rightWallTile;
            }
        }
        return null;
    }

    void SpawnContents()
    {
        // Aquí colocamos el contenido dependiendo del tipo de habitación
        switch (data.roomType)
        {
            case RoomType.Start:
                SpawnPlayer();
                break;
            case RoomType.Normal:
                SpawnEnemies();
                break;
            case RoomType.Shop:
                SpawnShopItems();
                break;
            case RoomType.Loot:
                SpawnLootChest();
                break;
            case RoomType.Boss:
                SpawnBoss();
                break;
        }
    }

    void SpawnPlayer()
    {
        // Asume que tienes un GameManager o algo similar que mueva al jugador
        // Aquí podrías instanciar el jugador si no existe, o situarlo en el centro
        // Ejemplo:
        // Player.Instance.transform.position = transform.position + new Vector3(roomWidth/2, roomHeight/2, 0);
    }

    void SpawnEnemies()
    {
        // Instancia enemigos
        // Podrías tener un EnemySpawner, o simplemente:
        // Instantiate(enemyPrefab, transform.position + new Vector3(8,8,0), Quaternion.identity);
    }

    void SpawnShopItems()
    {
        // Instancia un NPC de tienda y algunos objetos
    }

    void SpawnLootChest()
    {
        // Instancia un cofre de loot
    }

    void SpawnBoss()
    {
        // Instancia el boss en el centro de la sala
    }
}
