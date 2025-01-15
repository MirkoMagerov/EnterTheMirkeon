using UnityEngine;

[CreateAssetMenu(fileName = "NewAmmo", menuName = "Weapons/Ammo")]
public class Ammo : ScriptableObject
{
    public string ammoName;
    public float speed;
    public float lifetime = 5f;
    public GameObject visualPrefab;
}
