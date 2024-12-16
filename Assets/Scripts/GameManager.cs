using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject player;

    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        Debug.Log("Game Manager");
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
        Debug.Log("Jugador spawneado en la primera habitación.");
    }
}
