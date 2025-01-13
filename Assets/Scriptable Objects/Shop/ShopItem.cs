using UnityEngine;

public enum ItemType
{
    Weapon,
    Health,
    Key,
    ConsumableItem
}

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public string description;
    public int price;
    public Sprite sprite;
    public ItemType type;
    public int healthAmount;
    public Weapon weapon;
    public ConsumableItem consumableItem;
}
