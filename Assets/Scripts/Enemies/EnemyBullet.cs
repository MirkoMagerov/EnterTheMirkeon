using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerLife>().TakeDamage(1);
            Destroy(gameObject);
        }
    }
}
