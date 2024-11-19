using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    // Parámetros de generación
    public int dungeonWidth = 50;
    public int dungeonHeight = 50;

    public int roomCount = 10;
    public int roomWidthMin = 5;
    public int roomWidthMax = 10;
    public int roomHeightMin = 5;
    public int roomHeightMax = 10;

    public int roomMargin = 2;
    public int corridorMargin = 2;

    // Lista de habitaciones y pasillos generados
    private List<Room> rooms = new();
    private List<Corridor> corridors = new();

    // Referencia al DungeonRenderer
    private DungeonRenderer dungeonRenderer;

    private void Awake()
    {
        // Obtener referencia al DungeonRenderer
        dungeonRenderer = GetComponent<DungeonRenderer>();

        GenerateDungeon();
        SpawnPlayerInFirstRoom();
    }

    void GenerateDungeon()
    {
        // Generar habitaciones
        for (int i = 0; i < roomCount; i++)
        {
            GenerateRoom();
        }

        // Conectar habitaciones
        ConnectRooms();

        // Generar paredes alrededor del dungeon (opcional)
        GenerateDungeonWalls_OPTIONAL();
    }

    void GenerateRoom()
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            int width = Random.Range(roomWidthMin, roomWidthMax);
            int height = Random.Range(roomHeightMin, roomHeightMax);
            Vector2Int size = new Vector2Int(width, height);

            int x = Random.Range(roomMargin + 1, dungeonWidth - width - roomMargin - 1);
            int y = Random.Range(roomMargin + 1, dungeonHeight - height - roomMargin - 1);
            Vector2Int position = new Vector2Int(x, y);

            Room newRoom = new Room(position, size, roomMargin);

            // Comprobar superposición con habitaciones existentes
            bool overlaps = false;
            foreach (Room room in rooms)
            {
                if (newRoom.Overlaps(room))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                rooms.Add(newRoom);

                // Notificar al DungeonRenderer para que dibuje la habitación
                dungeonRenderer.DrawRoom(newRoom);

                // Marcar las paredes de la habitación
                MarkRoomWalls(newRoom);

                break;
            }
        }
    }

    void MarkRoomWalls(Room room)
    {
        // Calcular posiciones de las paredes y notificar al DungeonRenderer
        dungeonRenderer.DrawRoomWalls(room);
    }

    void ConnectRooms()
    {
        List<Room> unconnectedRooms = new(rooms);
        List<Room> connectedRooms = new();

        Room currentRoom = unconnectedRooms[Random.Range(0, unconnectedRooms.Count)];
        unconnectedRooms.Remove(currentRoom);
        connectedRooms.Add(currentRoom);

        while (unconnectedRooms.Count > 0)
        {
            Room closestRoom = null;
            float closestDistance = Mathf.Infinity;

            foreach (Room room in unconnectedRooms)
            {
                float distance = Vector2Int.Distance(currentRoom.center, room.center);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestRoom = room;
                }
            }

            if (closestRoom != null)
            {
                CreateCorridorBetweenRooms(currentRoom, closestRoom);

                unconnectedRooms.Remove(closestRoom);
                connectedRooms.Add(closestRoom);

                currentRoom = closestRoom;
            }
            else
            {
                break;
            }
        }
    }

    void CreateCorridorBetweenRooms(Room roomA, Room roomB)
    {
        // Ahora usaremos directamente los centros de las habitaciones
        Vector2Int pointA = roomA.center;
        Vector2Int pointB = roomB.center;

        // Crear la abertura en las paredes de las habitaciones
        dungeonRenderer.CreateDoorway(roomA, pointA);
        dungeonRenderer.CreateDoorway(roomB, pointB);

        if (Random.value < 0.5f)
        {
            // Conectar en x, luego en y
            CreateHorizontalCorridor(pointA.x, pointB.x, pointA.y);
            CreateVerticalCorridor(pointB.x, pointA.y, pointB.y);
        }
        else
        {
            // Conectar en y, luego en x
            CreateVerticalCorridor(pointA.x, pointA.y, pointB.y);
            CreateHorizontalCorridor(pointA.x, pointB.x, pointB.y);
        }
    }

    Vector2Int AdjustedPoint(Vector2Int center, Room room, int margin)
    {
        int x = center.x;
        int y = center.y;

        if (center.x - room.xMin < margin)
            x = room.xMin - margin;
        else if (room.xMax - center.x < margin)
            x = room.xMax + margin;

        if (center.y - room.yMin < margin)
            y = room.yMin - margin;
        else if (room.yMax - center.y < margin)
            y = room.yMax + margin;

        return new Vector2Int(x, y);
    }

    void CreateHorizontalCorridor(int xStart, int xEnd, int y)
    {
        Corridor corridor = new Corridor
        {
            isHorizontal = true,
            start = new Vector2Int(Mathf.Min(xStart, xEnd), y),
            end = new Vector2Int(Mathf.Max(xStart, xEnd), y)
        };

        corridors.Add(corridor);

        // Notificar al DungeonRenderer para que dibuje el pasillo
        dungeonRenderer.DrawCorridor(corridor);
    }

    void CreateVerticalCorridor(int x, int yStart, int yEnd)
    {
        Corridor corridor = new Corridor
        {
            isHorizontal = false,
            start = new Vector2Int(x, Mathf.Min(yStart, yEnd)),
            end = new Vector2Int(x, Mathf.Max(yStart, yEnd))
        };

        corridors.Add(corridor);

        // Notificar al DungeonRenderer para que dibuje el pasillo
        dungeonRenderer.DrawCorridor(corridor);
    }

    void GenerateDungeonWalls_OPTIONAL()
    {
        // Notificar al DungeonRenderer para que dibuje las paredes externas
        dungeonRenderer.DrawDungeonWalls(dungeonWidth, dungeonHeight);
    }

    void SpawnPlayerInFirstRoom()
    {
        if (rooms.Count == 0)
        {
            Debug.LogError("No hay habitaciones generadas para spawnear al jugador.");
            return;
        }

        Room firstRoom = rooms[0];
        Vector2Int centerTile = firstRoom.center;

        Vector3 spawnPos = new Vector3(centerTile.x + 0.5f, centerTile.y + 0.5f, 0);

        GameManager.Instance.SpawnPlayerInFirstRoom(spawnPos);
    }
}