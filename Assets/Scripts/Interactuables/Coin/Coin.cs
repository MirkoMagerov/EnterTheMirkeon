using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private Material pickUpMaterial;
    [SerializeField] private int coins;
    [SerializeField] private float moveSpeed;
    private bool hasTarget;
    private Vector3 targetPosition;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            spriteRenderer.material = pickUpMaterial;
            CoinManager coinManager = collision.GetComponent<CoinManager>();
            coinManager.AddCoins(coins);
            PlayerSounds.Instance.PlayPickUpCoinSound();
            anim.SetTrigger("PickUp");
        }
    }

    public void DestroyCoin()
    {
        Destroy(gameObject);
    }
}
