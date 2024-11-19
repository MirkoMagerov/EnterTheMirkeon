using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonRenderer : MonoBehaviour
{
    // Tilemaps y Tiles
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    public TileBase floorTile;
    public TileBase wallTile;

    // Referencia al DungeonGenerator
    private DungeonGenerator dungeonGenerator;

    private void Awake()
    {
        dungeonGenerator = GetComponent<DungeonGenerator>();
    }

    public void DrawRoom(Room room)
    {
        // Dibujar suelo de la habitación
        for (int x = room.xMin; x < room.xMax; x++)
        {
            for (int y = room.yMin; y < room.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                floorTilemap.SetTile(tilePos, floorTile);
            }
        }
    }

    public void DrawRoomWalls(Room room)
    {
        // Paredes horizontales
        for (int x = room.xMin - 1; x <= room.xMax; x++)
        {
            Vector3Int bottomWallPos = new Vector3Int(x, room.yMin - 1, 0);
            Vector3Int topWallPos = new Vector3Int(x, room.yMax, 0);

            if (!floorTilemap.HasTile(bottomWallPos))
                wallTilemap.SetTile(bottomWallPos, wallTile);

            if (!floorTilemap.HasTile(topWallPos))
                wallTilemap.SetTile(topWallPos, wallTile);
        }

        // Paredes verticales
        for (int y = room.yMin; y < room.yMax; y++)
        {
            Vector3Int leftWallPos = new Vector3Int(room.xMin - 1, y, 0);
            Vector3Int rightWallPos = new Vector3Int(room.xMax, y, 0);

            if (!floorTilemap.HasTile(leftWallPos))
                wallTilemap.SetTile(leftWallPos, wallTile);

            if (!floorTilemap.HasTile(rightWallPos))
                wallTilemap.SetTile(rightWallPos, wallTile);
        }
    }

    public void CreateDoorway(Room room, Vector2Int doorPosition)
    {
        // Remover la pared en la posición de la puerta y colocar suelo

        Vector3Int doorPos = new Vector3Int(doorPosition.x, doorPosition.y, 0);

        // Lista de posiciones de paredes que podrían ser reemplazadas por suelo
        List<Vector3Int> possibleWallPositions = new List<Vector3Int>();

        // Verificar si la puerta está en el borde de la habitación y agregar la posición correspondiente
        if (doorPosition.x == room.xMin - 1)
            possibleWallPositions.Add(new Vector3Int(room.xMin - 1, doorPosition.y, 0));
        else if (doorPosition.x == room.xMax)
            possibleWallPositions.Add(new Vector3Int(room.xMax, doorPosition.y, 0));
        else if (doorPosition.y == room.yMin - 1)
            possibleWallPositions.Add(new Vector3Int(doorPosition.x, room.yMin - 1, 0));
        else if (doorPosition.y == room.yMax)
            possibleWallPositions.Add(new Vector3Int(doorPosition.x, room.yMax, 0));
        else
        {
            // Si la puerta está dentro de la habitación, no es necesario hacer nada
            return;
        }

        // Reemplazar la pared por suelo
        foreach (var pos in possibleWallPositions)
        {
            wallTilemap.SetTile(pos, null);
            floorTilemap.SetTile(pos, floorTile);
        }
    }

    public void DrawCorridor(Corridor corridor)
    {
        if (corridor.isHorizontal)
        {
            int y = corridor.start.y;

            for (int x = corridor.start.x; x <= corridor.end.x; x++)
            {
                Vector3Int floorPos = new Vector3Int(x, y, 0);

                if (!floorTilemap.HasTile(floorPos))
                    floorTilemap.SetTile(floorPos, floorTile);

                // Paredes superior e inferior
                Vector3Int wallPosTop = new Vector3Int(x, y + 1, 0);
                Vector3Int wallPosBottom = new Vector3Int(x, y - 1, 0);

                if (!floorTilemap.HasTile(wallPosTop) && !wallTilemap.HasTile(wallPosTop))
                    wallTilemap.SetTile(wallPosTop, wallTile);

                if (!floorTilemap.HasTile(wallPosBottom) && !wallTilemap.HasTile(wallPosBottom))
                    wallTilemap.SetTile(wallPosBottom, wallTile);
            }
        }
        else
        {
            int x = corridor.start.x;

            for (int y = corridor.start.y; y <= corridor.end.y; y++)
            {
                Vector3Int floorPos = new Vector3Int(x, y, 0);

                if (!floorTilemap.HasTile(floorPos))
                    floorTilemap.SetTile(floorPos, floorTile);

                // Paredes izquierda y derecha
                Vector3Int wallPosLeft = new Vector3Int(x - 1, y, 0);
                Vector3Int wallPosRight = new Vector3Int(x + 1, y, 0);

                if (!floorTilemap.HasTile(wallPosLeft) && !wallTilemap.HasTile(wallPosLeft))
                    wallTilemap.SetTile(wallPosLeft, wallTile);

                if (!floorTilemap.HasTile(wallPosRight) && !wallTilemap.HasTile(wallPosRight))
                    wallTilemap.SetTile(wallPosRight, wallTile);
            }
        }
    }

    public void DrawDungeonWalls(int dungeonWidth, int dungeonHeight)
    {
        // Dibujar paredes alrededor del dungeon completo

        for (int x = -1; x <= dungeonWidth; x++)
        {
            Vector3Int bottomWallPos = new Vector3Int(x, -1, 0);
            Vector3Int topWallPos = new Vector3Int(x, dungeonHeight, 0);

            if (!floorTilemap.HasTile(bottomWallPos))
                wallTilemap.SetTile(bottomWallPos, wallTile);

            if (!floorTilemap.HasTile(topWallPos))
                wallTilemap.SetTile(topWallPos, wallTile);
        }

        for (int y = -1; y <= dungeonHeight; y++)
        {
            Vector3Int leftWallPos = new Vector3Int(-1, y, 0);
            Vector3Int rightWallPos = new Vector3Int(dungeonWidth, y, 0);

            if (!floorTilemap.HasTile(leftWallPos))
                wallTilemap.SetTile(leftWallPos, wallTile);

            if (!floorTilemap.HasTile(rightWallPos))
                wallTilemap.SetTile(rightWallPos, wallTile);
        }
    }
}
