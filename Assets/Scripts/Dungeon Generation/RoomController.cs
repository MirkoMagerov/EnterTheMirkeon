using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomController : MonoBehaviour
{
    public Tilemap wallTilemap;
    public RoomData roomData;
    public GameObject blackPanel;
    public List<GameObject> skulls;
    public GameObject chestPrefab;

    [Header("Door Positions")]
    public GameObject topDoorLeftPos;
    public GameObject topDoorRightPos;
    public GameObject rightDoorTopPos;
    public GameObject rightDoorBottomPos;
    public GameObject bottomDoorRightPos;
    public GameObject bottomDoorLeftPos;
    public GameObject leftDoorBottomPos;
    public GameObject leftDoorTopPos;

    [Header("Door Tiles")]
    public TileBase upDoorTile;
    public TileBase downDoorTile;
    public TileBase leftDoorTile;
    public TileBase rightDoorTile;

    [Header("Doors")]
    public GameObject doorPrefab;
    public Door topDoorLeft;
    public Door topDoorRight;
    public Door rightDoorTop;
    public Door rightDoorBottom;
    public Door bottomDoorRight;
    public Door bottomDoorLeft;
    public Door leftDoorBottom;
    public Door leftDoorTop;

    [Header("Wall / Floor Tiles")]
    public TileBase upWallTile;
    public TileBase downWallTile;
    public TileBase leftWallTile;
    public TileBase rightWallTile;
    public TileBase floorTile;

    [Header("Corridors")]
    public GameObject topCorridor;
    public GameObject rightCorridor;
    public GameObject bottomCorridor;
    public GameObject leftCorridor;

    [Header("Room Properties")]
    public int roomWidth = 20;
    public int roomHeight = 20;

    private bool roomActivated = false;
    private bool enemiesDefeated = false;

    private void Update()
    {
        CheckIfEnemiesDefeated();
    }

    public void Initialize(RoomData roomData)
    {
        this.roomData = roomData;

        bool connectUp = (roomData.connections & RoomConnections.Up) != 0;
        bool connectDown = (roomData.connections & RoomConnections.Down) != 0;
        bool connectLeft = (roomData.connections & RoomConnections.Left) != 0;
        bool connectRight = (roomData.connections & RoomConnections.Right) != 0;

        GenerateDoorsAndWalls(connectUp, connectDown, connectLeft, connectRight);
        SetCorridors(connectUp, connectDown, connectLeft, connectRight);

        SpawnContents();
    }

    public void GenerateDoorsAndWalls(bool connectTop, bool connectBottom, bool connectLeft, bool connectRight)
    {
        GenerateTopWallOrDoor(connectTop);
        GenerateRightWallOrDoor(connectRight);
        GenerateBottomWallOrDoor(connectBottom);
        GenerateLeftWallOrDoor(connectLeft);
    }

    private void GenerateTopWallOrDoor(bool connectTop)
    {
        if (connectTop)
        {
            InstantiateDoor(topDoorLeftPos.transform.position, DoorSide.TopLeft);
            InstantiateDoor(topDoorRightPos.transform.position, DoorSide.TopRight);
        }
        else
        {
            PlaceTile(topDoorLeftPos, upWallTile);
            PlaceTile(topDoorRightPos, upWallTile);
        }
    }

    private void GenerateRightWallOrDoor(bool connectRight)
    {
        if (connectRight)
        {
            InstantiateDoor(rightDoorTopPos.transform.position, DoorSide.RightTop);
            InstantiateDoor(rightDoorBottomPos.transform.position, DoorSide.RightBottom);
        }
        else
        {
            PlaceTile(rightDoorTopPos, rightWallTile);
            PlaceTile(rightDoorBottomPos, rightWallTile);
        }
    }

    private void GenerateBottomWallOrDoor(bool connectBottom)
    {
        if (connectBottom)
        {
            InstantiateDoor(bottomDoorRightPos.transform.position, DoorSide.BottomRight);
            InstantiateDoor(bottomDoorLeftPos.transform.position, DoorSide.BottomLeft);
        }
        else
        {
            PlaceTile(bottomDoorRightPos, connectBottom ? upDoorTile : downWallTile);
            PlaceTile(bottomDoorLeftPos, connectBottom ? upDoorTile : downWallTile);
        }
    }

    private void GenerateLeftWallOrDoor(bool connectLeft)
    {
        if (connectLeft)
        {
            InstantiateDoor(leftDoorBottomPos.transform.position, DoorSide.LeftBottom);
            InstantiateDoor(leftDoorTopPos.transform.position, DoorSide.LeftTop);
        }
        else
        {
            PlaceTile(leftDoorBottomPos, connectLeft ? leftDoorTile : leftWallTile);
            PlaceTile(leftDoorTopPos, connectLeft ? leftDoorTile : leftWallTile);
        }
    }

    private void SetCorridors(bool connectUp, bool connectDown, bool connectLeft, bool connectRight)
    {
        topCorridor.SetActive(connectUp);

        rightCorridor.SetActive(connectRight);

        bottomCorridor.SetActive(connectDown);

        leftCorridor.SetActive(connectLeft);

        if (roomData.roomType == RoomType.Boss)
        {

            skulls[0].SetActive(connectUp);
            skulls[1].SetActive(connectRight);
            skulls[2].SetActive(connectDown);
            skulls[3].SetActive(connectLeft);
        }
    }

    private void PlaceTile(GameObject doorMarker, TileBase tile)
    {
        if (doorMarker != null && wallTilemap != null)
        {
            Vector3Int tilePosition = wallTilemap.WorldToCell(doorMarker.transform.position);
            wallTilemap.SetTile(tilePosition, tile);
        }
    }

    private void InstantiateDoor(Vector3 position, DoorSide side)
    {
        GameObject doorInstance = Instantiate(doorPrefab, position, Quaternion.identity, gameObject.transform);
        Door doorScript = doorInstance.GetComponent<Door>();
        if (doorScript != null)
        {
            doorScript.side = side;
            doorScript.InitializeDoor();
        }

        switch (side)
        {
            case DoorSide.TopLeft:
                topDoorLeft = doorScript;
                break;
            case DoorSide.TopRight:
                topDoorRight = doorScript;
                break;
            case DoorSide.RightTop:
                rightDoorTop = doorScript;
                break;
            case DoorSide.RightBottom:
                rightDoorBottom = doorScript;
                break;
            case DoorSide.BottomRight:
                bottomDoorRight = doorScript;
                break;
            case DoorSide.BottomLeft:
                bottomDoorLeft = doorScript;
                break;
            case DoorSide.LeftBottom:
                leftDoorBottom = doorScript;
                break;
            case DoorSide.LeftTop:
                leftDoorTop = doorScript;
                break;
        }
    }

    public void DeactivateDoors()
    {
        if (topDoorLeft) { topDoorLeft.Deactivate(); topDoorRight.Deactivate(); }
        if (rightDoorTop) { rightDoorTop.Deactivate(); rightDoorBottom.Deactivate(); }
        if (bottomDoorRight) { bottomDoorRight.Deactivate(); bottomDoorLeft.Deactivate(); }
        if (leftDoorBottom) { leftDoorBottom.Deactivate(); leftDoorTop.Deactivate(); }
    }

    public void ActivateDoors()
    {
        if (topDoorLeft) { topDoorLeft.Activate(); topDoorRight.Activate(); }
        if (rightDoorTop) { rightDoorTop.Activate(); rightDoorBottom.Activate(); }
        if (bottomDoorRight) { bottomDoorRight.Activate(); bottomDoorLeft.Activate(); }
        if (leftDoorBottom) { leftDoorBottom.Activate(); leftDoorTop.Activate(); }
    }

    void SpawnContents()
    {
        switch (roomData.roomType)
        {
            case RoomType.Start:
                SpawnPlayer();
                break;
            case RoomType.Shop:
                SpawnShopItems();
                break;
            case RoomType.Loot:
                SpawnLootChest();
                break;
        }
    }

    void SpawnPlayer()
    {
        Vector3 centerPosition = Vector3.zero + transform.position;
        GameManager.Instance.SpawnPlayerInFirstRoom(centerPosition);
    }

    void SpawnShopItems()
    {
        Vector3 shopPosition = transform.position + new Vector3(0, 0, 0);
        GameObject shopControllerInstance = Instantiate(GameManager.Instance.shopControllerPrefab, shopPosition, Quaternion.identity, transform);

        if (shopControllerInstance.TryGetComponent<ShopController>(out var shopController))
        {
            shopController.allShopItems = GameManager.Instance.globalShopItems;

            shopController.InitializeShop();
        }
    }

    void SpawnLootChest()
    {
        Instantiate(chestPrefab, transform.position + new Vector3(0.55f, 0), Quaternion.identity, transform);
    }


    private void SpawnEnemies()
    {
        if (roomData == null || roomData.enemyPrefabs == null || roomData.spawnPoints == null) return;

        int randomEnemyQuantity = Random.Range(3, Mathf.Min(roomData.spawnPoints.Length) + 1);

        List<int> spawnPointIndices = new List<int>();
        for (int i = 0; i < roomData.spawnPoints.Length; i++)
        {
            spawnPointIndices.Add(i);
        }
        ShuffleList(spawnPointIndices);

        for (int i = 0; i < randomEnemyQuantity; i++)
        {
            int spawnPointIndex = spawnPointIndices[i];
            Vector2 spawnPoint = roomData.spawnPoints[spawnPointIndex];

            // Seleccionar un enemigo aleatorio
            int randomIndex = Random.Range(0, roomData.enemyPrefabs.Length);
            GameObject enemyPrefab = roomData.enemyPrefabs[randomIndex];

            Vector3 spawnPosition = transform.position + new Vector3(spawnPoint.x, spawnPoint.y, 0);
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void CheckIfEnemiesDefeated()
    {
        if (roomActivated && !enemiesDefeated && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            enemiesDefeated = true;
            ActivateDoors();
        }
    }

    void SpawnBoss()
    {
        Vector3 spawnPosition = transform.position + Vector3.zero;
        Instantiate(roomData.bossPrefab, spawnPosition, Quaternion.identity, transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !roomActivated)
        {
            blackPanel.SetActive(false);
            roomActivated = true;

            if (roomData.roomType == RoomType.Normal)
            {
                SpawnEnemies();
                DeactivateDoors();
            }

            else if (roomData.roomType == RoomType.Boss)
            {
                SpawnBoss();
                DeactivateDoors();
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
