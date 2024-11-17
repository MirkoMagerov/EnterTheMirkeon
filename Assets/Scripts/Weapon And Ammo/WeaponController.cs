using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform bulletSpawnPoint;

    [SerializeField] private WeaponHolder weaponHolder;

    private float lastFireTime;
    private int currentMag;
    private int totalBullets;
    private bool isReloading = false;

    private Weapon currentWeapon;
    private WeaponInventory weaponInventory;

    private void Awake()
    {
        weaponInventory = GetComponent<WeaponInventory>();
    }

    private void Start()
    {
        if (currentWeapon != null)
        {
            currentMag = currentWeapon.magSize;
        }
    }

    void Update()
    {
        if (currentWeapon == null) return;

        HandleShooting();

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentMag < currentWeapon.magSize && (currentWeapon.hasInfiniteAmmo || totalBullets > 0))
        {
            StartCoroutine(Reload());
        }
    }

    void HandleShooting()
    {
        if (isReloading) return;
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - bulletSpawnPoint.position).normalized;

        bool canFire = Time.time - lastFireTime >= 1f / currentWeapon.fireRate;

        switch (currentWeapon.weaponType)
        {
            case WeaponType.Automatic:
                if (Input.GetMouseButton(0) && canFire && (currentWeapon.hasInfiniteAmmo || currentMag > 0))
                {
                    Fire(direction);
                    lastFireTime = Time.time;
                }
                break;

            case WeaponType.SemiAutomatic:
                if (Input.GetMouseButtonDown(0) && canFire && (currentWeapon.hasInfiniteAmmo || currentMag > 0))
                {
                    Fire(direction);
                    lastFireTime = Time.time;
                }
                break;

            case WeaponType.Shotgun:
                if (Input.GetMouseButtonDown(0) && canFire && (currentWeapon.hasInfiniteAmmo || currentMag > 0))
                {
                    FireShotgun(direction);
                    lastFireTime = Time.time;
                }
                break;
        }
    }

    void Fire(Vector2 direction)
    {
        if (currentMag <= 0)
        {
            Debug.Log("Sin munición, recarga necesaria");
            return;
        }

        // Instancia la bala
        GameObject bullet = Instantiate(currentWeapon.ammoType.visualPrefab, bulletSpawnPoint.position, Quaternion.identity);
        if (bullet.TryGetComponent<BulletBehavior>(out var bulletBehavior))
        {
            bulletBehavior.Initialize(direction, currentWeapon.ammoType);
        }

        Debug.Log($"Disparo: Cargador restante: {currentMag}");

        currentMag--;

        // Disminuye la munición en WeaponInventory si no tiene munición infinita
        if (!currentWeapon.hasInfiniteAmmo)
        {
            weaponInventory.SetAmmoData(currentWeapon, currentMag, totalBullets);
        }
    }

    void FireShotgun(Vector2 direction)
    {
        if (currentMag <= 0)
        {
            Debug.Log("Sin munición, recarga necesaria");
            return;
        }

        for (int i = 0; i < currentWeapon.bulletsPerShot; i++)
        {
            float angleOffset = Random.Range(-currentWeapon.spreadAngle / 2, currentWeapon.spreadAngle / 2);
            Vector2 spreadDir = Quaternion.Euler(0, 0, angleOffset) * direction;

            // Instancia cada bala disparada con dispersión
            GameObject bullet = Instantiate(currentWeapon.ammoType.visualPrefab, bulletSpawnPoint.position, Quaternion.identity);
            if (bullet.TryGetComponent<BulletBehavior>(out var bulletBehavior))
            {
                bulletBehavior.Initialize(spreadDir, currentWeapon.ammoType);
            }
        }

        Debug.Log($"Disparo Shotgun: Cargador restante: {currentMag}");

        currentMag -= currentWeapon.bulletsPerShot;
        currentMag = Mathf.Max(currentMag, 0);

        // Disminuye la munición en WeaponInventory si no tiene munición infinita
        if (!currentWeapon.hasInfiniteAmmo)
        {
            weaponInventory.SetAmmoData(currentWeapon, currentMag, totalBullets);
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log($"Recargando: Cargador actual = {currentMag}, Balas totales = {totalBullets}, Tipo de bala = {currentWeapon.ammoType.name}");
        yield return new WaitForSeconds(currentWeapon.reloadTime);

        WeaponAmmoData ammoData = weaponInventory.GetAmmoData(currentWeapon);

        if (!currentWeapon.hasInfiniteAmmo)
        {
            bool reloaded = weaponInventory.ReloadWeapon(currentWeapon);
            if (reloaded)
            {
                currentMag = ammoData.currentMag;
                totalBullets = ammoData.totalBullets;
                Debug.Log("Recarga completada");
            }
            else
            {
                Debug.Log("No se pudo recargar: No hay balas suficientes o el cargador está lleno");
            }
        }
        else
        {
            currentMag = ammoData.currentMag;
            Debug.Log("Recarga completada para arma con balas infinitas");
        }

        isReloading = false;
    }

    public void SetWeapon(Weapon weapon, int mag, int total)
    {
        currentWeapon = weapon;
        currentMag = mag;
        totalBullets = total;
        lastFireTime = 0;
        isReloading = false;
        weaponHolder.EquipWeapon(weapon);
    }
}
