using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public List<ShopItem> allShopItems;
    public Transform[] spawnPoints;
    public GameObject shopItemPrefab;

    public void InitializeShop()
    {
        if (allShopItems == null || allShopItems.Count == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("ShopController is not properly configured.");
            return;
        }

        List<ShopItem> selectedItems = GetRandomShopItems(4);

        for (int i = 0; i < selectedItems.Count && i < spawnPoints.Length; i++)
        {
            SpawnShopItem(selectedItems[i], spawnPoints[i].position);
        }

        Debug.Log($"Shop initialized with {selectedItems.Count} items.");
    }

    private List<ShopItem> GetRandomShopItems(int count)
    {
        List<ShopItem> randomItems = new List<ShopItem>();

        List<ShopItem> availableItems = new List<ShopItem>(allShopItems);
        for (int i = 0; i < count && availableItems.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            randomItems.Add(availableItems[randomIndex]);
            availableItems.RemoveAt(randomIndex);
        }

        return randomItems;
    }

    private void SpawnShopItem(ShopItem item, Vector3 position)
    {
        GameObject shopItemInstance = Instantiate(shopItemPrefab, position, Quaternion.identity, transform);
        ShopItemDisplay itemDisplay = shopItemInstance.GetComponent<ShopItemDisplay>();

        if (itemDisplay != null)
        {
            itemDisplay.shopItem = item;
        }
    }
}
