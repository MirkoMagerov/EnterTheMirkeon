using System.Collections.Generic;
using UnityEngine;

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

    [Header("Enemies")]
    public GameObject[] enemies;
    public GameObject[] bosses;

    private RoomData[,] roomLayout;
    private bool shopPlaced = false;

    public void GenerateDungeon()
    {
        GenerateLayoutData();
        InstantiateRooms();
    }

    void GenerateLayoutData()
    {
        roomLayout = new RoomData[gridWidth, gridHeight];

        // 1. Colocar la Start Room
        int startX = Random.Range(0, gridWidth);
        int startY = Random.Range(0, gridHeight);
        roomLayout[startX, startY] = new RoomData(startX, startY, RoomType.Start);

        int currentX = startX;
        int currentY = startY;

        // 2. Generar las habitaciones
        for (int i = 0; i < maxRooms - 1; i++)
        {
            int dir = Random.Range(0, 4);
            int newX = currentX;
            int newY = currentY;

            if (dir == 0 && newY < gridHeight - 1) newY++;
            if (dir == 1 && newY > 0) newY--;
            if (dir == 2 && newX > 0) newX--;
            if (dir == 3 && newX < gridWidth - 1) newX++;

            if (roomLayout[newX, newY] == null)
            {
                RoomType roomType = GetRandomRoomType();
                roomLayout[newX, newY] = new RoomData(newX, newY, roomType)
                {
                    enemyPrefabs = enemies,
                    spawnPoints = GetSpawnPointsForRoomType(roomType)
                };
                currentX = newX;
                currentY = newY;
            }
        }

        // 3. Identificar la habitación más lejana
        Vector2Int bossRoomPosition = FindFurthestRoom(new Vector2Int(startX, startY));

        // 4. Marcarla como Boss Room
        RoomData bossRoom = roomLayout[bossRoomPosition.x, bossRoomPosition.y];
        roomLayout[bossRoomPosition.x, bossRoomPosition.y] = new RoomData(bossRoomPosition.x, bossRoomPosition.y, RoomType.Boss)
        {
            bossPrefab = bosses[Random.Range(0, bosses.Length)],
        };
        bossRoom.roomType = RoomType.Boss;

        // 5. Asegurar que haya una tienda
        if (!shopPlaced)
        {
            PlaceShopRoom();
        }

        // 6. Calcular conexiones
        CalculateConnections();
    }

    void PlaceShopRoom()
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (roomLayout[x, y] != null && roomLayout[x, y].roomType == RoomType.Normal)
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (validPositions.Count > 0)
        {
            Vector2Int shopPosition = validPositions[Random.Range(0, validPositions.Count)];
            roomLayout[shopPosition.x, shopPosition.y].roomType = RoomType.Shop;
            shopPlaced = true;
        }
    }

    RoomType GetRandomRoomType()
    {
        float rand = Random.value;

        if (!shopPlaced && rand < 0.2f)
        {
            shopPlaced = true;
            return RoomType.Shop;
        }
        if (rand < 0.65f) return RoomType.Normal;
        if (rand < 0.85f) return RoomType.Loot;
        return RoomType.Normal;
    }

    Vector2[] GetSpawnPointsForRoomType(RoomType type)
    {
        return type switch
        {
            RoomType.Normal => new Vector2[]
            {
                new Vector2(-1.5f, 1.5f),
                new Vector2(0f, 1.5f),
                new Vector2(1.5f, 1.5f),
                new Vector2(-1.5f, 0),
                new Vector2(0, 0),
                new Vector2(1.5f, 0),
                new Vector2(-1.5f, -1.5f),
                new Vector2(0, -1.5f),
                new Vector2(1.5f, -1.5f),
            },
            RoomType.Boss => new Vector2[]
            {
                new Vector2(0, 0),
            },
            _ => null,
        };
    }

    void CalculateConnections()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (roomLayout[x, y] != null)
                {
                    if (x < gridWidth - 1 && roomLayout[x + 1, y] != null)
                    {
                        roomLayout[x, y].connections |= RoomConnections.Right;
                        roomLayout[x + 1, y].connections |= RoomConnections.Left;
                    }
                    if (y < gridHeight - 1 && roomLayout[x, y + 1] != null)
                    {
                        roomLayout[x, y].connections |= RoomConnections.Up;
                        roomLayout[x, y + 1].connections |= RoomConnections.Down;
                    }
                }
            }
        }
    }

    void InstantiateRooms()
    {
        float offsetX = (gridWidth - 1) * roomWidthInUnityUnits / 2f;
        float offsetY = (gridHeight - 1) * roomHeightInUnityUnits / 2f;

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

    Vector2Int FindFurthestRoom(Vector2Int startRoomPosition)
    {
        Vector2Int furthestRoom = startRoomPosition;
        int maxDistance = 0;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (roomLayout[x, y] != null)
                {
                    int distance = Mathf.Abs(x - startRoomPosition.x) + Mathf.Abs(y - startRoomPosition.y);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        furthestRoom = new Vector2Int(x, y);
                    }
                }
            }
        }

        return furthestRoom;
    }
}
