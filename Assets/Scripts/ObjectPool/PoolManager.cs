using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreatePool(string poolKey, GameObject prefab, int initialSize)
    {
        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary[poolKey] = new Queue<GameObject>();

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                poolDictionary[poolKey].Enqueue(obj);
            }
        }
    }

    public GameObject SpawnFromPool(string poolKey, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolKey))
        {
            Debug.LogError($"No pool exists with key {poolKey}");
            return null;
        }

        GameObject objToSpawn = poolDictionary[poolKey].Count > 0 ? poolDictionary[poolKey].Dequeue() : null;

        if (objToSpawn == null)
        {
            Debug.LogWarning($"Pool for {poolKey} exhausted. Instantiating new object.");
            objToSpawn = Instantiate(Resources.Load<GameObject>(poolKey)); // Assumes prefab is in Resources folder.
        }

        objToSpawn.SetActive(true);
        objToSpawn.transform.SetPositionAndRotation(position, rotation);
        return objToSpawn;
    }

    public void ReturnToPool(string poolKey, GameObject objToReturn)
    {
        if (!poolDictionary.ContainsKey(poolKey))
        {
            Debug.LogWarning($"No pool exists with key {poolKey}, destroying object.");
            Destroy(objToReturn);
            return;
        }

        objToReturn.SetActive(false);
        poolDictionary[poolKey].Enqueue(objToReturn);
    }
}
