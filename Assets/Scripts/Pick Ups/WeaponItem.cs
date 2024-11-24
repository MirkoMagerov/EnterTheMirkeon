using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    [SerializeField] private Weapon weapon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            WeaponInventory weaponInventory = collision.gameObject.GetComponentInChildren<WeaponInventory>();
            weaponInventory.PickUpWeapon(weapon);
            Destroy(gameObject);
        }
    }
}
