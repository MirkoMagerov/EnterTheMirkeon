using UnityEngine;

public abstract class ConsumableItem : ScriptableObject
{
    [Header("General Settings")]
    public string itemName;
    public string itemDescription;
    public ItemType itemType;
    public Sprite itemIcon;

    [Header("Effect Settings")]
    public int healthRestoreAmount;

    public virtual void UseItem(GameObject player)
    {
        ApplyEffect(player);
    }

    protected virtual void ApplyEffect(GameObject player)
    {

    }
}
