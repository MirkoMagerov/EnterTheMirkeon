using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    private PlayerLife playerLife;
    private CoinManager coinManager;

    [Header("Health UI")]
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite emptyHeart;

    [Header("Coins")]
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Consumables")]
    [SerializeField] private Image currentItemImage;
    [SerializeField] private TextMeshProUGUI currentItemName;

    private void Awake()
    {
        coinManager = GetComponent<CoinManager>();
        playerLife = GetComponent<PlayerLife>();
    }

    private void Start()
    {
        UpdateHealthUI(playerLife.GetMaxHealth());
        UpdateCoinsUI(coinManager.GetCoins());
    }

    private void OnEnable()
    {
        playerLife.OnHealthChanged += UpdateHealthUI;
        coinManager.OnCoinsChanged += UpdateCoinsUI;
    }

    private void OnDisable()
    {
        playerLife.OnHealthChanged -= UpdateHealthUI;
        coinManager.OnCoinsChanged -= UpdateCoinsUI;
    }

    public void UpdateHealthUI(int health)
    {
        AdjustHeartImages(playerLife.GetMaxHealth());

        for (int i = 0; i < heartImages.Length; i++)
        {
            int heartValue = (i + 1) * 2;

            if (health >= heartValue)
            {
                heartImages[i].sprite = fullHeart;
            }
            else if (health == heartValue - 1)
            {
                heartImages[i].sprite = halfHeart;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
            }
        }
    }

    private void AdjustHeartImages(int maxHealth)
    {
        int requiredHearts = Mathf.CeilToInt(maxHealth / 2f);

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < requiredHearts)
            {
                heartImages[i].sprite = emptyHeart;
                heartImages[i].color = new Color(255,255,255, 255);
            }
            else
            {
                heartImages[i].sprite = null;
                heartImages[i].color = new Color(255, 255, 255, 0);
            }
        }
    }

    public void UpdateCoinsUI(int coins)
    {
        coinsText.text = coins.ToString();
    }

    public void UpdateConsumableUI(ConsumableItem item, int consumableLength)
    {
        if (consumableLength == 0)
        {
            currentItemName.text = string.Empty;
            currentItemImage.color = new Color(255, 255, 255, 0);
        }
        else
        {
            currentItemName.text = item.itemName;
            currentItemImage.sprite = item.itemIcon;
            currentItemImage.color = new Color(255, 255, 255, 255);
        }
    }
}
