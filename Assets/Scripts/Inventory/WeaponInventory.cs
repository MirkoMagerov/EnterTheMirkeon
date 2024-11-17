using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons = new();
    private WeaponController weaponController;
    private int currentWeaponIndex = 0;

    private void Start()
    {
        if (TryGetComponent<WeaponController>(out weaponController))
        {
            EquipCurrentWeapon();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchWeapon();
        }
    }

    public void PickUpWeapon(Weapon newWeapon)
    {
        if (newWeapon != null && !weapons.Contains(newWeapon))
        {
            weapons.Add(newWeapon);
            currentWeaponIndex = weapons.Count - 1;
            EquipCurrentWeapon();
            Debug.Log("Picked up weapon: " + newWeapon.name);
        }
    }

    private void SwitchWeapon()
    {
        if (weapons.Count == 0) return;

        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        EquipCurrentWeapon();
        Debug.Log("Switched to weapon: " + weapons[currentWeaponIndex].name);
    }

    private void EquipCurrentWeapon()
    {
        if (weapons.Count > 0 && weaponController != null)
        {
            Debug.Log("Equipping weapon: " + weapons[currentWeaponIndex].name);
            weaponController.SetWeapon(weapons[currentWeaponIndex]);
        }
    }
}
