using UnityEngine;

[CreateAssetMenu(fileName = "NewAmmoBox", menuName = "Items/Consumable/AmmoBox")]
public class AmmoBox : ConsumableItem
{
    [SerializeField] private int percentatgeTotalBulletsToAdd;
    public override void UseItem(GameObject player)
    {
        WeaponInventory inventory = player.GetComponentInChildren<WeaponInventory>();

        foreach(Weapon weapon in inventory.GetWeapons())
        {
            WeaponAmmoData ammoData = inventory.GetAmmoData(weapon);
            inventory.AddBullets(weapon, ammoData.totalBullets * percentatgeTotalBulletsToAdd / 100);
        }

        WeaponController weaponController = player.GetComponentInChildren<WeaponController>();
        weaponController.UpdateMaxAmmoUI();
    }
}
