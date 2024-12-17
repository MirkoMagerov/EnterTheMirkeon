using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform bulletSpawnPoint;

    [SerializeField] private WeaponHolder weaponHolder;
    [SerializeField] private TextMeshProUGUI currentBulletsUI;
    [SerializeField] private TextMeshProUGUI totalBulletsUI;
    [SerializeField] private LineRenderer lineRenderer;

    private float lastFireTime;
    private int currentMag;
    private int totalBullets;
    private bool isReloading = false;

    private Weapon currentWeapon;
    private WeaponInventory weaponInventory;
    private Coroutine reloadCoroutine;

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
        if (PlayerState.IsDashing) return;

        HandleShooting();

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentMag < currentWeapon.magSize && (currentWeapon.hasInfiniteAmmo || totalBullets > 0))
        {
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
            }
            reloadCoroutine = StartCoroutine(Reload());
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
            case WeaponType.Laser:
                if (Input.GetMouseButton(0))
                {
                    Vector2 adjustedDirection = ((Vector2)mousePos - (Vector2)bulletSpawnPoint.position).normalized;

                    float dotProduct = Vector2.Dot(adjustedDirection, (Vector2)bulletSpawnPoint.position - (Vector2)transform.position);
                    if (dotProduct < 0)
                    {
                        adjustedDirection = -adjustedDirection;
                    }

                    FireLaser(adjustedDirection);
                    lastFireTime = Time.time;
                }
                break;
        }
    }

    void FireLaser(Vector2 direction)
    {
        // Lanzar un Raycast en la dirección especificada
        RaycastHit2D hit = Physics2D.Raycast(bulletSpawnPoint.position, direction, 15);

        Vector2 endPoint;

        if (hit.collider)
        {
            // Si hay colisión, la línea termina en el punto de impacto
            endPoint = hit.point;
        }
        else
        {
            // Si no hay colisión, la línea se extiende en la dirección especificada
            endPoint = (Vector2)bulletSpawnPoint.position + direction.normalized * 15;
        }

        Draw2DRay(bulletSpawnPoint.position, endPoint);
    }

    void Draw2DRay(Vector2 start, Vector2 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    public float waveFrequency = 5f;   // Frecuencia de la onda
    public float waveAmplitude = 0.5f; // Altura de la onda
    public int segmentCount = 20;      // Cantidad de segmentos en la línea
    public float animationSpeed = 2f;  // Velocidad de animación de la onda

    void DrawWavyLaser(Vector2 startPoint, Vector2 endPoint)
    {
        lineRenderer.positionCount = segmentCount + 1;

        Vector2 direction = (endPoint - startPoint).normalized;
        float distance = Vector2.Distance(startPoint, endPoint);
        float segmentLength = distance / segmentCount;

        for (int i = 0; i <= segmentCount; i++)
        {
            // Posición base del punto (lineal)
            Vector2 point = startPoint + direction * (segmentLength * i);

            // Agregar una onda sinusoidal
            float waveOffset = Mathf.Sin((Time.time * animationSpeed) + i * waveFrequency) * waveAmplitude;

            // Perpendicular a la dirección del láser para la onda
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            point += perpendicular * waveOffset;

            lineRenderer.SetPosition(i, point);
        }
    }

    void Fire(Vector2 direction)
    {
        if (currentMag <= 0)
        {
            Debug.Log("Sin munición, recarga necesaria");
            return;
        }

        // Aplica el SpreadAngle
        float angleOffset = Random.Range(-currentWeapon.spreadAngle / 2f, currentWeapon.spreadAngle / 2f);
        Vector2 spreadDir = Quaternion.Euler(0, 0, angleOffset) * direction;

        // Instancia la bala con la dirección dispersa
        GameObject bullet = Instantiate(currentWeapon.ammoType.visualPrefab, bulletSpawnPoint.position, Quaternion.identity);
        if (bullet.TryGetComponent<BulletBehavior>(out var bulletBehavior))
        {
            bulletBehavior.Initialize(spreadDir, currentWeapon.ammoType, "player");
        }

        Debug.Log($"Disparo: Cargador restante: {currentMag}");

        currentMag--;
        currentBulletsUI.text = currentMag.ToString();

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
                bulletBehavior.Initialize(spreadDir, currentWeapon.ammoType, "Player");
            }
        }

        Debug.Log($"Disparo Shotgun: Cargador restante: {currentMag}");

        currentMag -= currentWeapon.bulletsPerShot;
        currentMag = Mathf.Max(currentMag, 0);
        currentBulletsUI.text = currentMag.ToString();

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
        currentBulletsUI.text = currentMag.ToString();
        if (currentWeapon.hasInfiniteAmmo) totalBulletsUI.text = "∞";
        else totalBulletsUI.text = totalBullets.ToString();

        reloadCoroutine = null;
    }

    public void SetWeapon(Weapon weapon, int mag, int total)
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            isReloading = false;
            reloadCoroutine = null;
            Debug.Log("Reload canceled");
        }

        currentWeapon = weapon;
        currentMag = mag;
        totalBullets = total;
        lastFireTime = 0;
        isReloading = false;
        weaponHolder.EquipWeapon(weapon);

        if (weapon.hasInfiniteAmmo) totalBulletsUI.text = "∞";
        else totalBulletsUI.text = totalBullets.ToString();
        currentBulletsUI.text = currentMag.ToString();
    }
}
