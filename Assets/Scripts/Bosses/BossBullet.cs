using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed;
    public int damage;
    private Vector2 direction;
    private Rigidbody2D rb;
    private float lifetime = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        MoveBullet();
    }

    private void MoveBullet()
    {
        rb.velocity = direction * speed;
    }

    public void SetDirectionAndDamage(Vector2 newDirection, int dmg)
    {
        direction = newDirection.normalized;
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.gameObject.TryGetComponent(out PlayerLife playerLife))
        {
            playerLife.TakeDamage(damage, gameObject);
            Destroy(gameObject);
        }
    }
}
