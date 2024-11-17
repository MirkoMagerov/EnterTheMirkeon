using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Automatic,
    SemiAutomatic,
    Shotgun
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public float fireRate;
    public int bulletsPerShot = 1;
    public float spreadAngle = 0f;
    public int magSize;
    public int totalBullets;
    public float reloadTime;
    public WeaponType weaponType;
    public Sprite weaponSprite;
    public Ammo ammoType;

    public bool hasInfiniteAmmo;
}
