using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ConsumablesInventory : MonoBehaviour
{
    public List<ConsumableItem> consumableItems;
    private ConsumableItem currentItem;
    private GameObject player;
    private PlayerUIManager playerUIManager;

    private void Start()
    {
        player = gameObject;
        playerUIManager = GetComponent<PlayerUIManager>();
        currentItem = consumableItems.Count > 0 ? consumableItems[0] : null;
        UpdateUI();
    }

    private void OnEnable()
    {
        var inputActions = InputManager.Instance.GetInputActions();
        inputActions.Consumable.UseItem.performed += HandleUseItem;
        inputActions.Consumable.SwitchItem.performed += SwitchItem;
    }

    private void OnDisable()
    {
        var inputActions = InputManager.Instance.GetInputActions();
        inputActions.Consumable.UseItem.performed -= HandleUseItem;
        inputActions.Consumable.SwitchItem.performed -= SwitchItem;
    }

    private void HandleUseItem(InputAction.CallbackContext context)
    {
        if (consumableItems.Count == 0) return;

        currentItem.UseItem(player);
        consumableItems.RemoveAt(consumableItems.IndexOf(currentItem));
        
        if (consumableItems.Count == 0)
        {
            currentItem = null;
        }
        else
        {
            currentItem = consumableItems[0];
        }

        UpdateUI();
    }

    private void SwitchItem(InputAction.CallbackContext context)
    {
        if (consumableItems.Count <= 1) return;

        int currentIndex = consumableItems.IndexOf(currentItem);

        currentIndex = (currentIndex + 1) % consumableItems.Count;

        currentItem = consumableItems[currentIndex];

        UpdateUI();
    }

    public void AddItem(ConsumableItem consumableItem)
    {
        consumableItems.Add(consumableItem);
        currentItem = consumableItem;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (consumableItems.Count == 0)
        {
            playerUIManager.UpdateConsumableUI(null, consumableItems.Count);
        }
        else
        {
            playerUIManager.UpdateConsumableUI(currentItem.itemIcon, consumableItems.Count);
        }
    }

    public bool UseRationBeforeDeath()
    {
        for (int i = 0; i < consumableItems.Count; i++)
        {
            if (consumableItems[i].itemName == "Ration")
            {
                consumableItems[i].UseItem(player);
                consumableItems.RemoveAt(i);
                UpdateUI();
                return true;
            }
        }

        return false;
    }
}
