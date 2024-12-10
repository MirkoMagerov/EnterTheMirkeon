using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons = new();
    [SerializeField] private Image currentWeaponImageUI;
    [SerializeField] private Image previousWeaponImageUI;
    [SerializeField] private Image nextWeaponImageUI;
    [SerializeField] private TextMeshProUGUI currentBullets;
    [SerializeField] private TextMeshProUGUI totalBullets;

    private WeaponController weaponController;
    private int currentWeaponIndex = 0;
    private Dictionary<Weapon, WeaponAmmoData> weaponAmmo = new();

    private void Start()
    {
        if (TryGetComponent(out weaponController))
        {
            InitializeAmmo();
            EquipCurrentWeapon();
            UpdateUI();
        }

        currentWeaponImageUI.preserveAspect = true;
        previousWeaponImageUI.preserveAspect = true;
        nextWeaponImageUI.preserveAspect = true;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            if (weapons.Count == 0) return;

            if (scroll < 0)
            {
                SelectNextWeapon();
            }
            else if (scroll > 0)
            {
                SelectPreviousWeapon();
            }

            UpdateUI();
        }
    }

    private void InitializeAmmo()
    {
        foreach (var weapon in weapons)
        {
            if (!weaponAmmo.ContainsKey(weapon))
            {
                weaponAmmo[weapon] = new WeaponAmmoData(weapon.magSize, weapon.totalBullets);
            }
        }
    }

    private void SelectNextWeapon()
    {
        if (currentWeaponIndex < weapons.Count - 1)
        {
            currentWeaponIndex++;
            EquipCurrentWeapon();
            Debug.Log("Switched to weapon: " + weapons[currentWeaponIndex].weaponName);
        }
        else
        {
            Debug.Log("Ya tienes equipada la última arma. No se puede cambiar a la siguiente.");
        }
    }

    private void SelectPreviousWeapon()
    {
        if (currentWeaponIndex > 0)
        {
            currentWeaponIndex--;
            EquipCurrentWeapon();
            Debug.Log("Switched to weapon: " + weapons[currentWeaponIndex].weaponName);
        }
        else
        {
            Debug.Log("Ya tienes equipada la primera arma. No se puede cambiar a la anterior.");
        }
    }

    public void PickUpWeapon(Weapon newWeapon)
    {
        if (newWeapon != null && !weapons.Contains(newWeapon))
        {
            weapons.Add(newWeapon);
            weaponAmmo[newWeapon] = new WeaponAmmoData(newWeapon.magSize, newWeapon.totalBullets);
            currentWeaponIndex = weapons.Count - 1;
            EquipCurrentWeapon();
            Debug.Log("Picked up weapon: " + newWeapon.name);
        }
    }

    private void EquipCurrentWeapon()
    {
        if (weapons.Count > 0 && weaponController != null)
        {
            Weapon currentWeapon = weapons[currentWeaponIndex];
            WeaponAmmoData ammoData = weaponAmmo[currentWeapon];
            Debug.Log("Equipping weapon: " + currentWeapon.weaponName);
            weaponController.SetWeapon(currentWeapon, ammoData.currentMag, ammoData.totalBullets);
        }
    }

    public WeaponAmmoData GetAmmoData(Weapon weapon)
    {
        if (weaponAmmo.ContainsKey(weapon))
        {
            return weaponAmmo[weapon];
        }
        else
        {
            Debug.LogError("Weapon not available in WeaponInventory: " + weapon.weaponName);
            weaponAmmo[weapon] = new WeaponAmmoData(weapon.magSize, weapon.totalBullets);
            return weaponAmmo[weapon];
        }
    }

    public void SetAmmoData(Weapon weapon, int newMag, int newTotal)
    {
        if (weaponAmmo.ContainsKey(weapon))
        {
            weaponAmmo[weapon].currentMag = Mathf.Clamp(newMag, 0, weapon.magSize);
            weaponAmmo[weapon].totalBullets = Mathf.Clamp(newTotal, 0, weapon.totalBullets);
        }
        else
        {
            weaponAmmo[weapon] = new WeaponAmmoData(Mathf.Clamp(newMag, 0, weapon.magSize), Mathf.Clamp(newTotal, 0, weapon.totalBullets));
        }
    }

    public bool ReloadWeapon(Weapon weapon)
    {
        if (weaponAmmo.ContainsKey(weapon))
        {
            WeaponAmmoData ammoData = weaponAmmo[weapon];

            if (ammoData.currentMag >= weapon.magSize)
            {
                Debug.Log("El cargador ya está lleno.");
                return false;
            }

            if (ammoData.totalBullets <= 0)
            {
                Debug.Log("No hay balas disponibles para recargar.");
                return false;
            }

            int bulletsNeeded = weapon.magSize - ammoData.currentMag;
            int bulletsToLoad = Mathf.Min(bulletsNeeded, ammoData.totalBullets);

            ammoData.currentMag += bulletsToLoad;
            ammoData.totalBullets -= bulletsToLoad;

            Debug.Log($"Recargado {bulletsToLoad} balas al {weapon.weaponName}. Cargador: {ammoData.currentMag}/{weapon.magSize}, Balas totales: {ammoData.totalBullets}");
            return true;
        }

        Debug.LogError("Weapon not available in WeaponInventory: " + weapon.weaponName);
        return false;
    }

    // Para añadir balas de drops o algo en un futuro
    public void AddBullets(Weapon weapon, int amount)
    {
        if (weaponAmmo.ContainsKey(weapon))
        {
            weaponAmmo[weapon].totalBullets = Mathf.Min(weaponAmmo[weapon].totalBullets + amount, weapon.totalBullets);
        }
        else
        {
            weaponAmmo[weapon] = new WeaponAmmoData(weapon.magSize, Mathf.Min(amount, weapon.totalBullets));
        }
    }

    private void UpdateUI()
    {
        currentWeaponImageUI.sprite = weapons[currentWeaponIndex].weaponSprite;

        previousWeaponImageUI.sprite = null;
        if (currentWeaponIndex - 1 >= 0 && weapons[currentWeaponIndex - 1] != null)
        {
            previousWeaponImageUI.sprite = weapons[currentWeaponIndex - 1].weaponSprite;
            previousWeaponImageUI.color = new Color(255, 255, 255, 1f);
        }
        else
        {
            previousWeaponImageUI.color = new Color(0, 0, 0, 0f);
        }

        nextWeaponImageUI.sprite = null;
        if (currentWeaponIndex + 1 < weapons.Count && weapons[currentWeaponIndex + 1] != null)
        {
            nextWeaponImageUI.sprite = weapons[currentWeaponIndex + 1].weaponSprite;
            nextWeaponImageUI.color = new Color(255, 255, 255, 1f);
        }
        else
        {
            nextWeaponImageUI.color = new Color(0, 0, 0, 0f);
        }
    }
}
