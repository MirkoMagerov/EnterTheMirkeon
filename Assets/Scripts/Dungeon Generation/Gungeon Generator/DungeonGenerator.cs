using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    [Header("Tiles")]
    public TileBase floorTile;
    public TileBase wallTile;

    [Header("Dungeon Settings")]
    public int mapWidth = 100;
    public int mapHeight = 100;
    public int minRoomSize = 5;
    public int maxRoomSize = 15;
    public int corridorWidth = 2;

    public List<Room> rooms = new List<Room>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // 1. Generar habitaciones usando BSP
        rooms = BSPSplit(new RectInt(0, 0, mapWidth, mapHeight), minRoomSize, maxRoomSize);

        // 2. Asignar tipos de habitaciones
        AssignRoomTypes();

        // 3. Dibujar habitaciones
        foreach (var room in rooms)
        {
            DrawRoom(room);
        }

        // 4. Conectar habitaciones con pasillos
        ConnectRooms();

        // 5. Dibujar paredes
        DrawWalls();
    }

    List<Room> BSPSplit(RectInt area, int minSize, int maxSize)
    {
        List<Room> finalRooms = new List<Room>();
        Queue<RectInt> partitions = new Queue<RectInt>();
        partitions.Enqueue(area);

        while (partitions.Count > 0)
        {
            RectInt current = partitions.Dequeue();

            if (current.width > maxSize || current.height > maxSize || Random.value > 0.5f)
            {
                bool splitHorizontally = current.width > current.height;
                if (current.width == current.height)
                    splitHorizontally = Random.value > 0.5f;

                if (splitHorizontally)
                {
                    if (current.height < 2 * minSize)
                    {
                        finalRooms.Add(new Room(current));
                        continue;
                    }
                    int split = Random.Range(minSize, current.height - minSize);
                    RectInt top = new RectInt(current.x, current.y, current.width, split);
                    RectInt bottom = new RectInt(current.x, current.y + split, current.width, current.height - split);
                    partitions.Enqueue(top);
                    partitions.Enqueue(bottom);
                }
                else
                {
                    if (current.width < 2 * minSize)
                    {
                        finalRooms.Add(new Room(current));
                        continue;
                    }
                    int split = Random.Range(minSize, current.width - minSize);
                    RectInt left = new RectInt(current.x, current.y, split, current.height);
                    RectInt right = new RectInt(current.x + split, current.y, current.width - split, current.height);
                    partitions.Enqueue(left);
                    partitions.Enqueue(right);
                }
            }
            else
            {
                finalRooms.Add(new Room(current));
            }
        }

        return finalRooms;
    }

    void AssignRoomTypes()
    {
        if (rooms.Count == 0) return;

        // Asignar la primera habitación como Start
        rooms[0].Type = RoomType.Start;

        // Asignar la última habitación como End
        rooms[^1].Type = RoomType.End;

        // Asignar una habitación aleatoria como Boss
        if (rooms.Count > 2)
        {
            int bossIndex = Random.Range(1, rooms.Count - 1);
            rooms[bossIndex].Type = RoomType.Boss;
        }

        // Asignar algunas habitaciones como Loot
        for (int i = 1; i < rooms.Count - 1; i++)
        {
            if (Random.value > 0.7f)
                rooms[i].Type = RoomType.Loot;
        }
    }

    void DrawRoom(Room room)
    {
        // Dibuja el suelo
        for (int x = room.Rect.xMin; x < room.Rect.xMax; x++)
        {
            for (int y = room.Rect.yMin; y < room.Rect.yMax; y++)
            {
                floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
            }
        }

        // Crear un GameObject para la habitación
        GameObject roomObject = new($"Room_{room.Type}");
        roomObject.transform.parent = this.transform;
        roomObject.transform.position = new Vector3(room.Center.x, room.Center.y, 0);

        RoomTypeScript roomTypeScript = roomObject.AddComponent<RoomTypeScript>();
        roomTypeScript.roomType = room.Type;
    }

    void ConnectRooms()
    {
        // Ordenar habitaciones por posición X
        rooms.Sort((a, b) => a.Center.x.CompareTo(b.Center.x));

        for (int i = 1; i < rooms.Count; i++)
        {
            Room current = rooms[i];
            Room previous = rooms[i - 1];
            Vector2Int currentCenter = current.Center;
            Vector2Int previousCenter = previous.Center;

            // Decidir el orden de las conexiones para variedad
            if (Random.value > 0.5f)
            {
                CreateHorizontalCorridor(previousCenter.x, currentCenter.x, previousCenter.y);
                CreateVerticalCorridor(previousCenter.y, currentCenter.y, currentCenter.x);
            }
            else
            {
                CreateVerticalCorridor(previousCenter.y, currentCenter.y, previousCenter.x);
                CreateHorizontalCorridor(previousCenter.x, currentCenter.x, currentCenter.y);
            }

            // Registrar conexiones
            current.ConnectedRooms.Add(previous);
            previous.ConnectedRooms.Add(current);
        }
    }

    void CreateHorizontalCorridor(int xStart, int xEnd, int y)
    {
        int start = Mathf.Min(xStart, xEnd);
        int end = Mathf.Max(xStart, xEnd);

        for (int x = start; x <= end; x++)
        {
            for (int w = 0; w < corridorWidth; w++)
            {
                floorTilemap.SetTile(new Vector3Int(x, y + w, 0), floorTile);
            }
        }
    }

    void CreateVerticalCorridor(int yStart, int yEnd, int x)
    {
        int start = Mathf.Min(yStart, yEnd);
        int end = Mathf.Max(yStart, yEnd);

        for (int y = start; y <= end; y++)
        {
            for (int w = 0; w < corridorWidth; w++)
            {
                floorTilemap.SetTile(new Vector3Int(x + w, y, 0), floorTile);
            }
        }
    }

    void DrawWalls()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (!floorTilemap.HasTile(pos))
                continue;

            // Verificar las 4 direcciones para dibujar paredes
            Vector3Int[] directions = {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

            foreach (var dir in directions)
            {
                Vector3Int neighbor = pos + dir;
                if (!floorTilemap.HasTile(neighbor) && !wallTilemap.HasTile(neighbor))
                {
                    wallTilemap.SetTile(neighbor, wallTile);
                }
            }
        }

        // Eliminar paredes superpuestas en las conexiones de pasillos y habitaciones
        foreach (var room in rooms)
        {
            Vector2Int center = room.Center;
            foreach (var connectedRoom in room.ConnectedRooms)
            {
                Vector2Int connectedCenter = connectedRoom.Center;
                Vector2Int direction = connectedCenter - center;

                // Determinar la dirección de la conexión
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    // Conexión horizontal
                    for (int w = 0; w < corridorWidth; w++)
                    {
                        wallTilemap.SetTile(new Vector3Int((int)(center.x + Mathf.Sign(direction.x)), center.y + w, 0), null);
                    }
                }
                else
                {
                    // Conexión vertical
                    for (int w = 0; w < corridorWidth; w++)
                    {
                        wallTilemap.SetTile(new Vector3Int(center.x + w, (int)(center.y + Mathf.Sign(direction.y)), 0), null);
                    }
                }
            }
        }
    }
}
