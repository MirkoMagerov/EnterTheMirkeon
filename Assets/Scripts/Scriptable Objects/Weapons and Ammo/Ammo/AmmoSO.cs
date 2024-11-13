using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAmmo", menuName = "Weapons/Ammo")]
public class AmmoSO : ScriptableObject
{
    public string ammoName;
    public GameObject projectilePrefab;
    public float speed;
    public int damage;
    public bool canPierce;
}
