using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 100;

    private List<GameObject> bulletPool;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }

    public void ShootBullet(Vector3 startPosition, Vector2 direction)
    {
        GameObject bullet = GetPooledBullet();
        if (bullet != null)
        {
            bullet.transform.position = startPosition;
            bullet.GetComponent<NewBullet>().Initialize(direction);
            bullet.SetActive(true);
        }
    }

    private GameObject GetPooledBullet()
    {
        return bulletPool.Find(b => !b.activeInHierarchy);
    }
}
