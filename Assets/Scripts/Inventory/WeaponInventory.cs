using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons = new();
    [SerializeField] private Image currentWeaponImageUI;
    [SerializeField] private Image previousWeaponImageUI;
    [SerializeField] private Image nextWeaponImageUI;
    [SerializeField] private TextMeshProUGUI currentWeaponName;

    private WeaponController weaponController;
    private int currentWeaponIndex = 0;
    private Dictionary<Weapon, WeaponAmmoData> weaponAmmo = new();

    private void Awake()
    {
        if (TryGetComponent(out weaponController))
        {
            InitializeAmmo();
            EquipCurrentWeapon();
            UpdateUI();
        }
    }

    private void Start()
    {
        InputManager.Instance.GetInputActions().WeaponSystem.ScrollWheel.performed += OnScrollWheel;
        currentWeaponImageUI.preserveAspect = true;
        previousWeaponImageUI.preserveAspect = true;
        nextWeaponImageUI.preserveAspect = true;
    }

    private void OnDisable()
    {
        InputManager.Instance.GetInputActions().WeaponSystem.ScrollWheel.performed -= OnScrollWheel;
    }

    void OnScrollWheel(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>();
        weaponController.RestartReloadSlider();

        if (scrollValue < 0)
        {
            SelectNextWeapon();
        }
        else if (scrollValue > 0)
        {
            SelectPreviousWeapon();
        }

        UpdateUI();
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
        }
    }

    private void SelectPreviousWeapon()
    {
        if (currentWeaponIndex > 0)
        {
            currentWeaponIndex--;
            EquipCurrentWeapon();
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
            UpdateUI();
        }
    }

    private void EquipCurrentWeapon()
    {
        if (weapons.Count > 0 && weaponController != null)
        {
            Weapon currentWeapon = weapons[currentWeaponIndex];
            WeaponAmmoData ammoData = weaponAmmo[currentWeapon];
            weaponController.SetWeapon(currentWeapon, ammoData.currentMag, ammoData.totalBullets);
        }
    }

    public WeaponAmmoData GetAmmoData(Weapon weapon)
    {
        if (weaponAmmo.ContainsKey(weapon))
        {
            return weaponAmmo[weapon];
        }
        return null;
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

            if (weapon.hasInfiniteAmmo)
            {
                ammoData.currentMag = weapon.magSize;return true;
            }

            if (ammoData.currentMag >= weapon.magSize)
            {
                return false;
            }

            if (ammoData.totalBullets <= 0)
            {
                return false;
            }

            int bulletsNeeded = weapon.magSize - ammoData.currentMag;
            int bulletsToLoad = Mathf.Min(bulletsNeeded, ammoData.totalBullets);

            ammoData.currentMag += bulletsToLoad;
            ammoData.totalBullets -= bulletsToLoad;

            return true;
        }

        return false;
    }

    public void AddBullets(Weapon weapon, int amount)
    {
        weaponAmmo[weapon].totalBullets = weaponAmmo[weapon].totalBullets + amount;
    }

    public List<Weapon> GetWeapons() { return weapons; }

    public void UpdateUI()
    {
        currentWeaponImageUI.sprite = weapons[currentWeaponIndex].weaponSprite;
        currentWeaponName.text = weapons[currentWeaponIndex].weaponName;

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

    public bool HasWeapon(Weapon weapon)
    {
        return weapons.Contains(weapon);
    }
}
