using OutlineFx;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopItemDisplay : MonoBehaviour, IInteractuable
{
    public ShopItem shopItem;

    [Header("UI Elements")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;

    private SpriteRenderer spriteRenderer;
    private CoinManager coinManager;
    private bool playerInShop = false;
    private Outline outlineFx;

    private void Start()
    {
        coinManager = GameObject.FindGameObjectWithTag("Player").GetComponent<CoinManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        outlineFx = GetComponent<Outline>();
        outlineFx.enabled = false;
        InitializeUI();
    }

    private void OnEnable()
    {
        InputManager.Instance.GetInputActions().Interactuable.Interact.performed += TryPurchase;
    }

    private void OnDisable()
    {
        InputManager.Instance.GetInputActions().Interactuable.Interact.performed -= TryPurchase;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInShop = true;
            descriptionText.enabled = true;
            SetOutline(true);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInShop = false;
            descriptionText.enabled = false;
            SetOutline(false);
        }
    }

    public void SetOutline(bool enable)
    {
        outlineFx.enabled = enable;
    }

    private void InitializeUI()
    {
        spriteRenderer.sprite = shopItem.sprite;

        itemNameText.text = shopItem.itemName;
        descriptionText.text = shopItem.description;
        descriptionText.enabled = false;
        priceText.text = shopItem.price.ToString();
    }

    private void TryPurchase(InputAction.CallbackContext context)
    {
        if (playerInShop)
        {
            if (coinManager.GetCoins() >= shopItem.price)
            {
                coinManager.SpendCoins(shopItem.price);

                ApplyItemEffect();

                Destroy(gameObject);
            }
        }
    }

    private void ApplyItemEffect()
    {
        switch (shopItem.type)
        {
            case ItemType.Weapon:
                GameManager.Instance.EquipWeapon(shopItem.weapon);
                break;
            case ItemType.Health:
                if (shopItem.healthAmount > 0) 
                    GameManager.Instance.HealPlayer(shopItem.healthAmount);

                if (shopItem.maxHealthIncrease > 0) 
                    GameManager.Instance.IncreaseMaximumHealth(shopItem.maxHealthIncrease);
                break;
            case ItemType.ConsumableItem:
                GameObject.FindGameObjectWithTag("Player").GetComponent<ConsumablesInventory>().AddItem(shopItem.consumableItem);
                break;
        }
    }
}
