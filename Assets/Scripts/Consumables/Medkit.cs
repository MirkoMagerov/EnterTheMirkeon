using UnityEngine;

[CreateAssetMenu(fileName = "NewMedkit", menuName = "Items/Consumable/Medkit")]
public class Medkit : ConsumableItem
{
    public override void ApplyEffect(GameObject player)
    {
        PlayerLife playerLife = player.GetComponent<PlayerLife>();
        playerLife.RegenerateHealth(healthRestoreAmount);
    }
}