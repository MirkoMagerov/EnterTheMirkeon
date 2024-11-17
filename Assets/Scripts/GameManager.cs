using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnPlayerInFirstRoom(Vector3 spawnPoint)
    {
        if (playerPrefab != null)
        {
            Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
            Debug.Log("Jugador spawneado en la primera habitación.");
        }
        else
        {
            Debug.LogError("El prefab del jugador no está asignado en el DungeonGenerator.");
        }
    }
}
