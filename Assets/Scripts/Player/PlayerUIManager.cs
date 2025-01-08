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

    public void UpdateCoinsUI(int coins)
    {
        coinsText.text = coins.ToString();
    }
}
