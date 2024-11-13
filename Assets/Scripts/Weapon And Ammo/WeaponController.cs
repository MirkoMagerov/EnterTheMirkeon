using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponData;
    [SerializeField] private Transform bulletSpawnPoint;

    private float nextFireTime = 0f;
    private SpriteRenderer weaponSpriteRenderer;
    private Stack<GameObject> bulletPool;

    private void Start()
    {
        weaponSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (weaponSpriteRenderer != null)
        {
            weaponSpriteRenderer.sprite = weaponData.weaponSprite;
        }

        InitializeAmmoPool();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            FireWeaponSingleBullet();

            nextFireTime = Time.time + weaponData.fireRate;
        }
    }

    void FireWeaponSingleBullet()
    {
        for (int i = 0; i < weaponData.bulletsPerShot; i++)
        {
            // Calcular dirección con dispersión
            float spread = Random.Range(-weaponData.spread, weaponData.spread);
            Vector3 shootDirection = Quaternion.Euler(0, 0, spread) * transform.right;

            // Obtener el proyectil del pool
            GameObject projectile = GetBulletFromPool();
            if (projectile == null) return;

            // Configurar el proyectil
            projectile.transform.position = bulletSpawnPoint.position;
            projectile.transform.rotation = Quaternion.identity;
            projectile.SetActive(true);

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.velocity = shootDirection * weaponData.ammoType.speed;
        }
    }

    private void InitializeAmmoPool()
    {
        bulletPool = new Stack<GameObject>();
        for (int i = 0; i < weaponData.magSize; i++)
        {
            GameObject bullet = Instantiate(weaponData.ammoType.projectilePrefab);
            bullet.SetWeapon(this);
            bullet.SetActive(false);
            bulletPool.Push(bullet);
        }
    }

    private GameObject GetBulletFromPool()
    {
        if (bulletPool.Count > 0)
        {
            return bulletPool.Pop();
        }
        else
        {
            Debug.LogWarning("EL BULLET POOL ESTA VACÍO.");
            return null;
        }
    }

    public void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Push(bullet);
    }
}
