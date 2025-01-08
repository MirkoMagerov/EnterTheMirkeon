using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    private float lifetime;
    private string shooterTag;

    public void Initialize(Vector2 dir, Ammo ammo, string shooterTag)
    {
        direction = dir.normalized;
        speed = ammo.speed;
        damage = ammo.damage;
        lifetime = ammo.lifetime;
        this.shooterTag = shooterTag;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(shooterTag)) return;

        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
