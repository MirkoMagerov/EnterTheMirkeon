using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coins;

    public void SetCoins(int coins)
    {
        this.coins = coins;
    }

    public void SetRandomCoins(int minAmount, int maxAmount)
    {
        int coins = Random.Range(minAmount, maxAmount + 1);
        SetCoins(coins);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CoinManager coinManager = collision.GetComponent<CoinManager>();
            coinManager.AddCoins(coins);
            Destroy(gameObject);
        }
    }
}
