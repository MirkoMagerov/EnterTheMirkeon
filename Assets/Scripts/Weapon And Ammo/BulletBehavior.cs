using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float lifetime;
    private string shooterTag;
    private int damage;

    public void Initialize(Vector2 dir, Ammo ammo, string shooterTag, int damage)
    {
        direction = dir.normalized;
        speed = ammo.speed;
        this.damage = damage;
        lifetime = ammo.lifetime;
        this.shooterTag = shooterTag;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(shooterTag) || collision.gameObject.CompareTag("Player")) return;

        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage, gameObject);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
