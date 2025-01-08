using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopItemDisplay : MonoBehaviour, IInteractuable
{
    public ShopItem shopItem;

    [Header("UI Elements")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI priceText;

    private SpriteRenderer spriteRenderer;
    private CoinManager coinManager;
    private bool playerInShop = false;

    private void Start()
    {
        coinManager = GameObject.FindGameObjectWithTag("Player").GetComponent<CoinManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            SetOutline(true);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInShop = false;
            SetOutline(false);
        }
    }

    public void SetOutline(bool enable)
    {
        //outlineFx.enabled = enable;
    }

    private void InitializeUI()
    {
        spriteRenderer.sprite = shopItem.sprite;

        itemNameText.text = shopItem.name;
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
            else
            {
                Debug.LogError("Not enough coins!");
            }
        }
    }

    private void ApplyItemEffect()
    {
        switch (shopItem.type)
        {
            case ItemType.Weapon:
                Debug.Log("Weapon purchased!");
                break;
            case ItemType.Health:
                GameManager.Instance.HealPlayer(shopItem.healthAmount);
                break;
            case ItemType.Key:
                Debug.Log("Purchased key");
                break;
            case ItemType.PassiveItem:
                Debug.Log("Passive Item applied!");
                break;
            default:
                Debug.LogWarning("Unknown item type!");
                break;
        }
    }
}
