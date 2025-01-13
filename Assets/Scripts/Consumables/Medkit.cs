using UnityEngine;

[CreateAssetMenu(fileName = "NewMedkit", menuName = "Items/Consumable/Medkit")]
public class Medkit : ConsumableItem
{
    protected override void ApplyEffect(GameObject player)
    {
        PlayerLife playerLife = player.GetComponent<PlayerLife>();
        playerLife.RegenerateHealth(healthRestoreAmount);
    }
}