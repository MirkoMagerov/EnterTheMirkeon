using UnityEngine;

public enum ItemType
{
    Weapon,
    Health,
    ConsumableItem
}

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/ShopItem")]
public class ShopItem : ScriptableObject
{
    [Header("Generic attributes")]
    public string itemName;
    [TextArea] public string description;
    public int price;
    public Sprite sprite;
    public ItemType type;

    [Header("Health attributes")]
    public int healthAmount;
    public int maxHealthIncrease;

    [Header("Weapon")]
    public Weapon weapon;

    [Header("Consumable Item")]
    public ConsumableItem consumableItem;
}
