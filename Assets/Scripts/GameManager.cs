using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject player;
    public GameObject shopControllerPrefab;
    public List<ShopItem> globalShopItems;

    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            player = Instantiate(playerPrefab);
            player.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnPlayerInFirstRoom(Vector3 spawnPoint)
    {
        player.SetActive(true);
        player.transform.position = spawnPoint;
        CameraController.Instance.EnableCameraFollow(player.transform);
    }

    public void HealPlayer(int healAmount)
    {
        PlayerLife playerLife = player.GetComponent<PlayerLife>();
        playerLife.RegenerateHealth(healAmount);
    }
}
