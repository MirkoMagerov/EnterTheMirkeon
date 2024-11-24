using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap floorTilemap;
    public TileBase floorTile;

    public GameObject[] roomPrefabs;
    public int minRooms = 5;
    public int maxRooms = 15;
    public int margin = 2;

    private List<Vector2Int> gridPositions = new();
    private Dictionary<Vector2Int, Room> dungeonMap = new();

    void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        gridPositions.Clear();
        dungeonMap.Clear();

        // Crear la habitación de Spawn en el origen
        CreateRoomAtPosition(Room.RoomType.Spawn, Vector2Int.zero);

        // Crear otras habitaciones especiales
        CreateRoom(Room.RoomType.End);
        CreateRoom(Room.RoomType.Boss);
        CreateRoom(Room.RoomType.Shop);
        CreateRoom(Room.RoomType.Loot);

        // Crear habitaciones normales
        int normalRooms = Mathf.Max(5, Random.Range(minRooms, maxRooms) - 5);
        for (int i = 0; i < normalRooms; i++)
        {
            CreateRoom(Room.RoomType.Normal);
        }
    }

    private void CreateRoomAtPosition(Room.RoomType roomType, Vector2Int position)
    {
        GameObject roomPrefab = GetRoomPrefabByType(roomType);
        Room roomData = roomPrefab.GetComponent<Room>();

        if (IsPositionOccupied(position, roomData.roomSize))
            return;

        Vector3 worldPosition = new Vector3(position.x, position.y, 0);

        GameObject roomInstance = Instantiate(roomPrefab, worldPosition, Quaternion.identity);
        Room room = roomInstance.GetComponent<Room>();
        room.roomType = roomType;
        room.gridPosition = position;

        dungeonMap[position] = room;
        gridPositions.Add(position);
    }

    private void CreateRoom(Room.RoomType roomType)
    {
        GameObject roomPrefab = GetRoomPrefabByType(roomType);
        Room roomData = roomPrefab.GetComponent<Room>();

        // Obtener una posición válida y la habitación conectada
        Room connectedRoom;
        Vector2Int validPosition = GetValidPosition(roomData.roomSize, out connectedRoom);

        if (validPosition == Vector2Int.zero) return; // Si no se encuentra posición válida, salimos

        Vector3 worldPosition = new(validPosition.x, validPosition.y, 0);

        GameObject roomInstance = Instantiate(roomPrefab, worldPosition, Quaternion.identity);
        Room room = roomInstance.GetComponent<Room>();
        room.roomType = roomType;
        room.gridPosition = validPosition;

        dungeonMap[validPosition] = room;
        gridPositions.Add(validPosition);

        // Crear un pasillo si hay una habitación conectada
        if (connectedRoom != null)
        {
            CreateCorridor(connectedRoom.gridPosition, validPosition);
        }
    }

    private GameObject GetRoomPrefabByType(Room.RoomType type)
    {
        foreach (var prefab in roomPrefabs)
        {
            Room room = prefab.GetComponent<Room>();
            if (room.roomType == type)
                return prefab;
        }
        return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
    }

    private Vector2Int GetValidPosition(Vector2Int roomSize, out Room connectedRoom)
    {
        connectedRoom = null;
        int maxAttempts = 20;
        int maxOffset = 50; // Controla el rango de desplazamiento aleatorio

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Seleccionar una habitación existente al azar
            List<Room> existingRooms = new List<Room>(dungeonMap.Values);
            Room existingRoom = existingRooms[Random.Range(0, existingRooms.Count)];
            Vector2Int existingPosition = existingRoom.gridPosition;

            // Generar desplazamientos aleatorios
            int offsetX = Random.Range(-maxOffset, maxOffset + 1);
            int offsetY = Random.Range(-maxOffset, maxOffset + 1);

            // Calcular la nueva posición
            Vector2Int newPosition = existingPosition + new Vector2Int(offsetX, offsetY);

            // Comprobar si la posición es válida
            if (!IsPositionOccupied(newPosition, roomSize))
            {
                connectedRoom = existingRoom;
                return newPosition;
            }
        }

        // Si no se encuentra una posición válida, devolver Vector2Int.zero
        return Vector2Int.zero;
    }

    private Vector2Int RandomDirection()
    {
        int minStep = 10; // Distancia mínima (tamaño base de una habitación)
        int maxStep = 20; // Distancia máxima permitida para evitar separación excesiva

        int stepX = Random.Range(-2, 3) * Random.Range(minStep, maxStep + 1); // Aleatorio dentro del rango permitido
        int stepY = Random.Range(-2, 3) * Random.Range(minStep, maxStep + 1);

        // Asegurarse de que no ambos valores sean 0 (sin desplazamiento)
        while (stepX == 0 && stepY == 0)
        {
            stepX = Random.Range(-2, 3) * Random.Range(minStep, maxStep + 1);
            stepY = Random.Range(-2, 3) * Random.Range(minStep, maxStep + 1);
        }

        return new Vector2Int(stepX, stepY);
    }

    private bool IsPositionTooFar(Vector2Int position, int maxDistance)
    {
        foreach (var existingRoom in dungeonMap)
        {
            Vector2Int existingPosition = existingRoom.Key;

            // Calcula la distancia Manhattan
            int distance = Mathf.Abs(position.x - existingPosition.x) + Mathf.Abs(position.y - existingPosition.y);

            if (distance <= maxDistance) return false;
        }
        return true;
    }

    private bool IsPositionOccupied(Vector2Int position, Vector2Int roomSize)
    {
        int margin = this.margin; // Usa el margen definido en la clase

        // Definir el rectángulo de la nueva habitación
        RectInt newRoomRect = new RectInt(
            position.x - roomSize.x / 2 - margin,
            position.y - roomSize.y / 2 - margin,
            roomSize.x + 2 * margin,
            roomSize.y + 2 * margin);

        foreach (var existingRoom in dungeonMap.Values)
        {
            Vector2Int existingPosition = existingRoom.gridPosition;
            Vector2Int existingSize = existingRoom.roomSize;

            // Definir el rectángulo de la habitación existente
            RectInt existingRoomRect = new RectInt(
                existingPosition.x - existingSize.x / 2,
                existingPosition.y - existingSize.y / 2,
                existingSize.x,
                existingSize.y);

            // Comprobar si los rectángulos se superponen
            if (newRoomRect.Overlaps(existingRoomRect))
            {
                return true;
            }
        }
        return false;
    }

    private void CreateCorridor(Vector2Int fromPosition, Vector2Int toPosition)
    {
        Vector2Int currentPosition = fromPosition;
        List<Vector2Int> corridorPositions = new List<Vector2Int>();

        // Decidir aleatoriamente si moverse primero en X o en Y
        bool moveFirstInX = Random.value > 0.5f;

        while (currentPosition != toPosition)
        {
            if (moveFirstInX && currentPosition.x != toPosition.x)
            {
                currentPosition.x += (toPosition.x > currentPosition.x) ? 1 : -1;
            }
            else if (!moveFirstInX && currentPosition.y != toPosition.y)
            {
                currentPosition.y += (toPosition.y > currentPosition.y) ? 1 : -1;
            }
            else if (currentPosition.x != toPosition.x)
            {
                currentPosition.x += (toPosition.x > currentPosition.x) ? 1 : -1;
            }
            else if (currentPosition.y != toPosition.y)
            {
                currentPosition.y += (toPosition.y > currentPosition.y) ? 1 : -1;
            }

            corridorPositions.Add(currentPosition);
        }

        // Colocar los tiles de pasillo
        foreach (var position in corridorPositions)
        {
            PlaceCorridorTile(position);
        }
    }

    private void PlaceCorridorTile(Vector2Int position)
    {
        floorTilemap.SetTile((Vector3Int)position, floorTile);
    }
}
