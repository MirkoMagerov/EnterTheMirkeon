using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coins;
    [SerializeField] private float moveSpeed;
    private bool hasTarget;
    private Vector3 targetPosition;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (hasTarget)
        {
            Vector2 targetDirection = (targetPosition - transform.position).normalized;
            rb.velocity = new Vector2(targetDirection.x, targetDirection.y) * moveSpeed;
        }
    }

    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

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
