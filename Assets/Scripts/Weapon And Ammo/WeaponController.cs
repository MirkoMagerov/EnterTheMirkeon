using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    public Transform bulletSpawnPoint;

    [SerializeField] private WeaponHolder weaponHolder;
    [SerializeField] private TextMeshProUGUI currentBulletsUI;
    [SerializeField] private TextMeshProUGUI totalBulletsUI;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Slider reloadSlider;

    private float lastFireTime;
    private int currentMag;
    private int totalBullets;
    private bool isReloading = false;
    private bool isMeleeAttacking = false;
    private Coroutine shootingCoroutine;

    private Weapon currentWeapon;
    private WeaponInventory weaponInventory;
    private Coroutine reloadCoroutine;

    private void Awake()
    {
        weaponInventory = GetComponent<WeaponInventory>();
        reloadSlider.gameObject.SetActive(false);
    }

    private void Start()
    {
        currentMag = currentWeapon.magSize;
    }

    private void OnEnable()
    {
        var inputActions = InputManager.Instance.GetInputActions();
        inputActions.WeaponSystem.Shoot.performed += StartShooting;
        inputActions.WeaponSystem.Shoot.canceled += StopShooting;
        inputActions.WeaponSystem.Reload.performed += HandleReload;
    }

    private void OnDestroy()
    {
        var inputActions = InputManager.Instance.GetInputActions();
        inputActions.WeaponSystem.Shoot.performed -= StartShooting;
        inputActions.WeaponSystem.Shoot.canceled -= StopShooting;
        inputActions.WeaponSystem.Reload.performed -= HandleReload;
    }

    private void StartShooting(InputAction.CallbackContext context)
    {
        if (isReloading || PlayerState.Instance.IsDashing || isMeleeAttacking) return;

        if (currentWeapon.weaponType == WeaponType.Automatic)
        {
            shootingCoroutine ??= StartCoroutine(AutomaticFire());
        }
        else
        {
            bool canFire = Time.time - lastFireTime >= 1f / currentWeapon.fireRate;

            if (!canFire || !(currentWeapon.hasInfiniteAmmo || currentMag > 0)) return;

            Vector2 directionToMouse = GetCorrectMousePosition();
            Shake.Instance.TriggerShake();

            switch (currentWeapon.weaponType)
            {
                case WeaponType.SemiAutomatic:
                    FireSingleBulletWeapon(directionToMouse);
                    lastFireTime = Time.time;
                    break;

                case WeaponType.Shotgun:
                    FireShotgunWeapon(directionToMouse);
                    lastFireTime = Time.time;
                    break;
            }
        }
    }

    private void StopShooting(InputAction.CallbackContext context)
    {
        if (currentWeapon.weaponType == WeaponType.Automatic && shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    private IEnumerator AutomaticFire()
    {
        while (true)
        {
            if (Time.time - lastFireTime >= 1f / currentWeapon.fireRate)
            {
                Vector2 directionToMouse = GetCorrectMousePosition();
                Shake.Instance.TriggerShake();

                if (currentWeapon.hasInfiniteAmmo || currentMag > 0)
                {
                    FireSingleBulletWeapon(directionToMouse);
                    lastFireTime = Time.time;
                }
                else
                {
                    PlayerSounds.Instance.PlayEmptyMgazineSound();
                    StopShooting(default);
                    yield break;
                }
            }

            yield return null;
        }
    }

    Vector2 GetCorrectMousePosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 directionToMouse = (mousePos - bulletSpawnPoint.position).normalized;
        Vector2 directionToPlayer = (transform.position - bulletSpawnPoint.position).normalized;

        float dotProduct = Vector2.Dot(directionToMouse, directionToPlayer);
        return dotProduct > 0 ? -directionToMouse : directionToMouse;
    }

    void FireSingleBulletWeapon(Vector2 direction)
    {
        if (currentMag <= 0)
        {
            PlayerSounds.Instance.PlayEmptyMgazineSound();
            return;
        }

        Vector2 spreadDir = ApplySpreadAngle(direction);

        GameObject bullet = Instantiate(currentWeapon.ammoType.visualPrefab, bulletSpawnPoint.position, Quaternion.identity);
        if (bullet.TryGetComponent<BulletBehavior>(out var bulletBehavior))
        {
            bulletBehavior.Initialize(spreadDir, currentWeapon.ammoType, gameObject.tag);
        }

        PlayerSounds.Instance.PlayLaserShot();
        currentMag--;
        currentBulletsUI.text = currentMag.ToString();

        weaponInventory.SetAmmoData(currentWeapon, currentMag, totalBullets);
    }

    void FireShotgunWeapon(Vector2 direction)
    {
        if (currentMag <= 0)
        {
            PlayerSounds.Instance.PlayEmptyMgazineSound();
            return;
        }

        for (int i = 0; i < currentWeapon.bulletsPerShot; i++)
        {
            Vector2 spreadDir = ApplySpreadAngle(direction);

            GameObject bullet = Instantiate(currentWeapon.ammoType.visualPrefab, bulletSpawnPoint.position, Quaternion.identity);
            if (bullet.TryGetComponent<BulletBehavior>(out var bulletBehavior))
            {
                bulletBehavior.Initialize(spreadDir, currentWeapon.ammoType, gameObject.tag);
            }
        }

        PlayerSounds.Instance.PlayLaserShot();
        currentMag -= currentWeapon.bulletsPerShot;
        currentMag = Mathf.Max(currentMag, 0);
        currentBulletsUI.text = currentMag.ToString();

        weaponInventory.SetAmmoData(currentWeapon, currentMag, totalBullets);
    }

    private void HandleReload(InputAction.CallbackContext context)
    {
        if (!isReloading && currentMag < currentWeapon.magSize && (currentWeapon.hasInfiniteAmmo || totalBullets > 0))
        {
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
            }
            reloadCoroutine = StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        PlayerSounds.Instance.PlayReloadSound();

        reloadSlider.gameObject.SetActive(true);
        reloadSlider.value = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < currentWeapon.reloadTime)
        {
            elapsedTime += Time.deltaTime;

            if (reloadSlider != null)
            {
                reloadSlider.value = elapsedTime / currentWeapon.reloadTime;
            }

            yield return null;
        }

        WeaponAmmoData ammoData = weaponInventory.GetAmmoData(currentWeapon);

        bool reloaded = weaponInventory.ReloadWeapon(currentWeapon);
        if (reloaded)
        {
            currentMag = ammoData.currentMag;
            totalBullets = ammoData.totalBullets;
        }

        reloadSlider.gameObject.SetActive(false);
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
            PlayerSounds.Instance.StopActualSoundClip();
            StopCoroutine(reloadCoroutine);
            isReloading = false;
            reloadCoroutine = null;
            reloadSlider.value = 0;
            reloadSlider.gameObject.SetActive(false);
        }

        currentWeapon = weapon;
        currentMag = mag;
        totalBullets = total;
        lastFireTime = 0;
        isReloading = false;
        weaponHolder.EquipWeapon(weapon);

        bulletSpawnPoint.localPosition = weapon.bulletSpawnOffset;

        if (weapon.hasInfiniteAmmo) totalBulletsUI.text = "∞";
        else totalBulletsUI.text = totalBullets.ToString();
        currentBulletsUI.text = currentMag.ToString();
    }

    public void UpdateMaxAmmoUI()
    {
        if (currentWeapon.hasInfiniteAmmo) return;
        WeaponAmmoData weaponAmmoData = weaponInventory.GetAmmoData(currentWeapon);
        totalBulletsUI.text = weaponAmmoData.totalBullets.ToString();
    }

    public void RestartReloadSlider()
    {
        reloadSlider.value = 0;
        reloadSlider.gameObject.SetActive(false);
    }

    Vector2 ApplySpreadAngle(Vector2 direction)
    {
        float angleOffset = Random.Range(-currentWeapon.spreadAngle / 2f, currentWeapon.spreadAngle / 2f);
        Vector2 spreadDir = Quaternion.Euler(0, 0, angleOffset) * direction;
        return spreadDir;
    }

    public void StartMeleeAttack()
    {
        isMeleeAttacking = true;
        if (reloadCoroutine != null)
        {
            PlayerSounds.Instance.StopActualSoundClip();
            isReloading = false;
            reloadSlider.value = 0f;
            reloadSlider.gameObject.SetActive(false);
            StopCoroutine(reloadCoroutine);
        }
    }

    public void StopMeleeAttack()
    {
        isMeleeAttacking = false;
    }
}
