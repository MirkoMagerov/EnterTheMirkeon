using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private int swordDamage = 30;
    [SerializeField] private float knockbackForce = 50f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyLife>(out var enemyLife))
        {
            enemyLife.TakeDamage(swordDamage, gameObject);

            if (collision.TryGetComponent<Rigidbody2D>(out var enemyRigidbody))
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                enemyRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}
