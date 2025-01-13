using UnityEngine;

[CreateAssetMenu(fileName = "NewOrange", menuName = "Items/Consumable/Orange")]
public class Orange : ConsumableItem
{
    protected override void ApplyEffect(GameObject player)
    {
        PlayerLife playerLife = player.GetComponent<PlayerLife>();
        playerLife.RegenerateHealth(healthRestoreAmount);
        playerLife.AddHearts(2);
    }
}