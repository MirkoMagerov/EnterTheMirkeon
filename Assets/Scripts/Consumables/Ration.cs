using UnityEngine;

[CreateAssetMenu(fileName = "NewRation", menuName = "Items/Consumable/Ration")]
public class Ration : ConsumableItem
{
    protected override void ApplyEffect(GameObject player)
    {
        PlayerLife playerLife = player.GetComponent<PlayerLife>();
        playerLife.RegenerateHealth(healthRestoreAmount);
    }
}