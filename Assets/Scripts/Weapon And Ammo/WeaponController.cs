using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponHolder weaponHolder;

    public Weapon currentWeapon;
    public Transform bulletSpawnPoint;

    private float lastFireTime;
    private int currentAmmo;
    private bool isReloading = false;

    private void Awake()
    {
        if (weaponHolder != null && currentWeapon != null)
        {
            SetWeapon(currentWeapon);
        }
    }

    private void Start()
    {
        if (currentWeapon != null)
        {
            currentAmmo = currentWeapon.magSize;
        }
    }

    void Update()
    {
        if (currentWeapon == null) return;

        HandleShooting();

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < currentWeapon.magSize)
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
                if (Input.GetMouseButton(0) && canFire && currentAmmo > 0)
                {
                    Fire(direction);
                    lastFireTime = Time.time;
                }
                break;

            case WeaponType.SemiAutomatic:
                if (Input.GetMouseButtonDown(0) && canFire && currentAmmo > 0)
                {
                    Fire(direction);
                    lastFireTime = Time.time;
                }
                break;

            case WeaponType.Shotgun:
                if (Input.GetMouseButtonDown(0) && canFire && currentAmmo > 0)
                {
                    FireShotgun(direction);
                    lastFireTime = Time.time;
                }
                break;
        }
    }

    void Fire(Vector2 direction)
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Sin munición, recarga necesaria");
            return;
        }

        GameObject bullet = Instantiate(currentWeapon.ammoType.visualPrefab, bulletSpawnPoint.position, Quaternion.identity);
        if (bullet.TryGetComponent<BulletBehavior>(out var bulletBehavior))
        {
            bulletBehavior.Initialize(direction, currentWeapon.ammoType);
        }
        currentAmmo--;
    }

    void FireShotgun(Vector2 direction)
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Sin munición, recarga necesaria");
            return;
        }

        for (int i = 0; i < currentWeapon.bulletsPerShot; i++)
        {
            float angleOffset = Random.Range(-currentWeapon.spreadAngle / 2, currentWeapon.spreadAngle / 2);
            Vector2 spreadDir = Quaternion.Euler(0, 0, angleOffset) * direction;

            GameObject bullet = Instantiate(currentWeapon.ammoType.visualPrefab, bulletSpawnPoint.position, Quaternion.identity);
            if (bullet.TryGetComponent<BulletBehavior>(out var bulletBehavior))
            {
                bulletBehavior.Initialize(spreadDir, currentWeapon.ammoType);
            }
        }
        currentAmmo -= currentWeapon.bulletsPerShot;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log($"Reloading: {currentAmmo}, ammoType: {currentWeapon.ammoType.name}");
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        currentAmmo = currentWeapon.magSize;
        isReloading = false;
        Debug.Log("Recarga completada");
    }

    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        lastFireTime = 0;
        currentAmmo = weapon.magSize;
        isReloading = false;
        weaponHolder.EquipWeapon(weapon);
    }
}
