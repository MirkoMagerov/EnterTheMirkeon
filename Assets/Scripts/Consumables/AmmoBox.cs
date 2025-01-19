using UnityEngine;

[CreateAssetMenu(fileName = "NewAmmoBox", menuName = "Items/Consumable/AmmoBox")]
public class AmmoBox : ConsumableItem
{
    [SerializeField] private int percentatgeTotalBulletsToAdd;
    public override void ApplyEffect(GameObject player)
    {
        WeaponInventory inventory = player.GetComponentInChildren<WeaponInventory>();

        foreach(Weapon weapon in inventory.GetWeapons())
        {
            int bulletsToAdd = Mathf.FloorToInt(weapon.totalBullets * percentatgeTotalBulletsToAdd / 100);
            inventory.AddBullets(weapon, bulletsToAdd);
        }

        WeaponController weaponController = player.GetComponentInChildren<WeaponController>();
        weaponController.UpdateMaxAmmoUI();
    }
}
