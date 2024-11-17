using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAmmo", menuName = "Weapons/Ammo")]
public class Ammo : ScriptableObject
{
    public string ammoName;
    public float speed;
    public int damage;
    public float lifetime = 5f;
    public GameObject visualPrefab;
}
