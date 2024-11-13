using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;
    public float fireRate; // Tiempo entre disparos
    public int bulletsPerShot = 1;
    public float spread; // Dispersión en grados
    public AmmoSO ammoType;
    public int magSize;
    public AudioClip fireSound;
}
