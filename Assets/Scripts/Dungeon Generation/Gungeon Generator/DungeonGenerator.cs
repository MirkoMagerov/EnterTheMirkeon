using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditor;

public enum CorridorType
{
    Horizontal,
    Vertical
}

public class DungeonGenerator : MonoBehaviour
{
    // Referencias a los Tilemaps
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    // Referencias a los Tiles
    public TileBase floorTile;
    public TileBase wallTile;

    // Tamaño total del dungeon
    public int dungeonWidth = 50;
    public int dungeonHeight = 50;

    // Parámetros de las habitaciones
    public int roomCount = 10;
    public int roomWidthMin = 5;
    public int roomWidthMax = 10;
    public int roomHeightMin = 5;
    public int roomHeightMax = 10;

    // Lista para almacenar las habitaciones generadas
    private List<Room> rooms = new();

    // Margen entre habitaciones y pasillos
    public int roomMargin = 2;
    public int corridorMargin = 2;

    private void Awake()
    {
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

        ConnectRooms();

        // Generar paredes alrededor del dungeon completo (opcional)
        GenerateDungeonWalls_OPTIONAL();
    }

    void GenerateRoom()
    {
        // Intentar generar una habitación válida
        for (int attempt = 0; attempt < 20; attempt++)
        {
            // Generar tamaño aleatorio para la habitación
            int width = Random.Range(roomWidthMin, roomWidthMax);
            int height = Random.Range(roomHeightMin, roomHeightMax);
            Vector2Int size = new Vector2Int(width, height);

            // Generar posición aleatoria para la habitación dentro de los límites del dungeon, considerando el margen
            int x = Random.Range(roomMargin + 1, dungeonWidth - width - roomMargin - 1);
            int y = Random.Range(roomMargin + 1, dungeonHeight - height - roomMargin - 1);
            Vector2Int position = new Vector2Int(x, y);

            Room newRoom = new Room(position, size, roomMargin);

            // Comprobar si la habitación se solapa con habitaciones existentes
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
                // Añadir la habitación a la lista
                rooms.Add(newRoom);

                // Generar el suelo de la habitación
                for (int iX = newRoom.xMin; iX < newRoom.xMax; iX++)
                {
                    for (int iY = newRoom.yMin; iY < newRoom.yMax; iY++)
                    {
                        floorTilemap.SetTile(new Vector3Int(iX, iY, 0), floorTile);
                    }
                }

                // Generar las paredes de la habitación
                GenerateRoomWalls(newRoom);

                break; // Salir del bucle si la habitación se generó correctamente
            }
        }
    }

    void GenerateRoomWalls(Room room)
    {
        // Paredes horizontales
        for (int x = room.xMin - 1; x <= room.xMax; x++)
        {
            wallTilemap.SetTile(new Vector3Int(x, room.yMin - 1, 0), wallTile); // Pared inferior
            wallTilemap.SetTile(new Vector3Int(x, room.yMax, 0), wallTile);     // Pared superior
        }

        // Paredes verticales
        for (int y = room.yMin; y < room.yMax; y++)
        {
            wallTilemap.SetTile(new Vector3Int(room.xMin - 1, y, 0), wallTile); // Pared izquierda
            wallTilemap.SetTile(new Vector3Int(room.xMax, y, 0), wallTile);     // Pared derecha
        }
    }

    void ConnectRooms()
    {
        // Lista de habitaciones no conectadas
        List<Room> unconnectedRooms = new List<Room>(rooms);

        // Lista de habitaciones ya conectadas
        List<Room> connectedRooms = new List<Room>();

        // Seleccionar una habitación inicial aleatoria
        Room currentRoom = unconnectedRooms[Random.Range(0, unconnectedRooms.Count)];
        unconnectedRooms.Remove(currentRoom);
        connectedRooms.Add(currentRoom);

        // Mientras haya habitaciones no conectadas
        while (unconnectedRooms.Count > 0)
        {
            // Encontrar la habitación no conectada más cercana a la habitación actual
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
                // Crear un pasillo entre currentRoom y closestRoom
                CreateCorridorBetweenRooms(currentRoom, closestRoom);

                // Marcar la habitación como conectada
                unconnectedRooms.Remove(closestRoom);
                connectedRooms.Add(closestRoom);

                // Avanzar a la siguiente habitación
                currentRoom = closestRoom;
            }
            else
            {
                // Si no se encontró una habitación no conectada (lo cual no debería suceder), salimos del bucle
                break;
            }
        }
    }

    void CreateCorridorBetweenRooms(Room roomA, Room roomB)
    {
        // Ajustar los puntos de conexión para mantener el margen con las paredes
        Vector2Int pointA = AdjustedPoint(roomA.center, roomA, corridorMargin);
        Vector2Int pointB = AdjustedPoint(roomB.center, roomB, corridorMargin);

        // Decidir aleatoriamente si conectar primero en x o en y
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
            x = room.xMin - 1; // Ajusta al borde exterior
        else if (room.xMax - center.x < margin)
            x = room.xMax; // Ajusta al borde exterior

        if (center.y - room.yMin < margin)
            y = room.yMin - 1; // Ajusta al borde exterior
        else if (room.yMax - center.y < margin)
            y = room.yMax; // Ajusta al borde exterior

        return new Vector2Int(x, y);
    }

    void CreateHorizontalCorridor(int xStart, int xEnd, int y)
    {
        int xMin = Mathf.Min(xStart, xEnd);
        int xMax = Mathf.Max(xStart, xEnd);

        for (int x = xMin; x <= xMax; x++)
        {
            Vector3Int floorPos1 = new(x, y, 0);
            Vector3Int floorPos2 = new(x, y + 1, 0);

            // Solo colocar el suelo si no hay uno ya
            if (!floorTilemap.HasTile(floorPos1))
                floorTilemap.SetTile(floorPos1, floorTile);

            if (!floorTilemap.HasTile(floorPos2))
                floorTilemap.SetTile(floorPos2, floorTile);

            // Colocar paredes solo si no hay ya una pared o suelo
            Vector3Int wallPosTop = new(x, y - 1, 0);
            Vector3Int wallPosBottom = new(x, y + 2, 0);

            if (!floorTilemap.HasTile(wallPosTop) && !wallTilemap.HasTile(wallPosTop))
                wallTilemap.SetTile(wallPosTop, wallTile);

            if (!floorTilemap.HasTile(wallPosBottom) && !wallTilemap.HasTile(wallPosBottom))
                wallTilemap.SetTile(wallPosBottom, wallTile);
        }
    }

    void CreateVerticalCorridor(int x, int yStart, int yEnd)
    {
        int yMin = Mathf.Min(yStart, yEnd);
        int yMax = Mathf.Max(yStart, yEnd);

        for (int y = yMin; y <= yMax; y++)
        {
            Vector3Int floorPos1 = new(x, y, 0);
            Vector3Int floorPos2 = new(x + 1, y, 0);

            if (!floorTilemap.HasTile(floorPos1))
                floorTilemap.SetTile(floorPos1, floorTile);

            if (!floorTilemap.HasTile(floorPos2))
                floorTilemap.SetTile(floorPos2, floorTile);

            Vector3Int wallPosLeft = new(x - 1, y, 0);
            Vector3Int wallPosRight = new(x + 2, y, 0);

            if (!floorTilemap.HasTile(wallPosLeft) && !wallTilemap.HasTile(wallPosLeft))
                wallTilemap.SetTile(wallPosLeft, wallTile);

            if (!floorTilemap.HasTile(wallPosRight) && !wallTilemap.HasTile(wallPosRight))
                wallTilemap.SetTile(wallPosRight, wallTile);
        }
    }

    void GenerateDungeonWalls_OPTIONAL()
    {
        // Generar paredes alrededor del dungeon completo (opcional)
        for (int x = -1; x <= dungeonWidth; x++)
        {
            wallTilemap.SetTile(new Vector3Int(x, -1, 0), wallTile);            // Pared inferior
            wallTilemap.SetTile(new Vector3Int(x, dungeonHeight, 0), wallTile); // Pared superior
        }

        for (int y = -1; y <= dungeonHeight; y++)
        {
            wallTilemap.SetTile(new Vector3Int(-1, y, 0), wallTile);            // Pared izquierda
            wallTilemap.SetTile(new Vector3Int(dungeonWidth, y, 0), wallTile);  // Pared derecha
        }
    }

    void SpawnPlayerInFirstRoom()
    {
        if (rooms.Count == 0)
        {
            Debug.LogError("No hay habitaciones generadas para spawnear al jugador.");
            return;
        }

        // Obtener la primera habitación
        Room firstRoom = rooms[0];
        Vector2Int centerTile = firstRoom.center;

        // Convertir las coordenadas de la grilla a coordenadas del mundo
        Vector3Int centerTilePos = new Vector3Int(centerTile.x, centerTile.y, 0);
        Vector3 spawnPos = floorTilemap.CellToWorld(centerTilePos) + floorTilemap.cellSize / 2f;

        GameManager.Instance.SpawnPlayerInFirstRoom(spawnPos);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (rooms != null)
        {
            foreach (Room room in rooms)
            {
                // Dibujar el contorno de la habitación
                Gizmos.DrawWireCube(new Vector3(room.center.x, room.center.y, 0), new Vector3(room.size.x, room.size.y, 0));

                // Dibujar el margen alrededor de la habitación
                Gizmos.color = Color.red;
                Vector2Int marginSize = new Vector2Int(room.size.x + room.margin * 2, room.size.y + room.margin * 2);
                Gizmos.DrawWireCube(new Vector3(room.center.x, room.center.y, 0), new Vector3(marginSize.x, marginSize.y, 0));
                Gizmos.color = Color.green;
            }
        }
    }
}
