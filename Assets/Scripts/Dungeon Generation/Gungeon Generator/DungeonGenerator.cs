using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generation settings")]
    public int gridWidth = 4;
    public int gridHeight = 4;

    public int roomWidthInUnityUnits = 20;
    public int roomHeightInUnityUnits = 20;

    public int maxRooms = 10;

    [Header("Room Prefabs")]
    public GameObject startRoomPrefab;
    public GameObject normalRoomPrefab;
    public GameObject shopRoomPrefab;
    public GameObject lootRoomPrefab;
    public GameObject bossRoomPrefab;

    private RoomData[,] roomLayout;
    private bool shopPlaced = false;

    void Start()
    {
        GenerateLayoutData();
        InstantiateRooms();
    }

    void GenerateLayoutData()
    {
        roomLayout = new RoomData[gridWidth, gridHeight];

        // Habitación inicial
        int startX = Random.Range(0, gridWidth);
        int startY = Random.Range(0, gridHeight);
        roomLayout[startX, startY] = new RoomData(startX, startY, RoomType.Start);

        // Generar un camino aleatorio hasta el boss
        int currentX = startX;
        int currentY = startY;

        for (int i = 0; i < maxRooms - 1; i++)
        {
            // Moverse a una dirección aleatoria sin salir del grid
            int dir = Random.Range(0, 4); // 0: arriba, 1: abajo, 2: izquierda, 3: derecha
            int newX = currentX;
            int newY = currentY;

            if (dir == 0 && newY < gridHeight - 1) newY++;
            if (dir == 1 && newY > 0) newY--;
            if (dir == 2 && newX > 0) newX--;
            if (dir == 3 && newX < gridWidth - 1) newX++;

            if (roomLayout[newX, newY] == null)
            {
                RoomType randomType;

                float rand = Random.value;
                if (rand < 0.7f)
                {
                    randomType = RoomType.Normal;
                }
                else if (!shopPlaced && rand < 0.8f)
                {
                    randomType = RoomType.Shop;
                    shopPlaced = true;
                }
                else if (rand < 0.9f)
                {
                    randomType = RoomType.Loot;
                }
                else
                {
                    randomType = RoomType.Normal;
                }

                roomLayout[newX, newY] = new RoomData(newX, newY, randomType);
            }

            currentX = newX;
            currentY = newY;
        }

        // Ultima habitación del camino como Boss
        roomLayout[currentX, currentY].roomType = RoomType.Boss;

        // Ahora calcula conexiones
        CalculateConnections();
    }

    void CalculateConnections()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (roomLayout[x, y] != null)
                {
                    // Conexión a la derecha
                    if (x < gridWidth - 1 && roomLayout[x + 1, y] != null)
                    {
                        roomLayout[x, y].connectRight = true;
                        roomLayout[x + 1, y].connectLeft = true;
                    }
                    // Conexión arriba
                    if (y < gridHeight - 1 && roomLayout[x, y + 1] != null)
                    {
                        roomLayout[x, y].connectUp = true;
                        roomLayout[x, y + 1].connectDown = true;
                    }
                }
            }
        }
    }

    void InstantiateRooms()
    {
        float offsetX = gridWidth * roomWidthInUnityUnits / 2f;
        float offsetY = gridHeight * roomHeightInUnityUnits / 2f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (roomLayout[x, y] == null) continue;

                RoomData data = roomLayout[x, y];
                GameObject prefab = GetPrefabForRoomType(data.roomType);

                Vector3 position = new Vector3(
                    (x * roomWidthInUnityUnits) - offsetX,
                    (y * roomHeightInUnityUnits) - offsetY,
                    0
                );

                GameObject roomInstance = Instantiate(prefab, position, Quaternion.identity, transform);

                // Inicializar la sala
                RoomController rc = roomInstance.GetComponent<RoomController>();
                rc?.Initialize(data);
            }
        }
    }

    GameObject GetPrefabForRoomType(RoomType type)
    {
        return type switch
        {
            RoomType.Start => startRoomPrefab,
            RoomType.Normal => normalRoomPrefab,
            RoomType.Shop => shopRoomPrefab,
            RoomType.Loot => lootRoomPrefab,
            RoomType.Boss => bossRoomPrefab,
            _ => normalRoomPrefab,
        };
    }
}
