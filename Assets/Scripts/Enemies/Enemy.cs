using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Attack
}

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Base Enemy Stats")]
    public float health = 10f;
    public float speed = 2f;

    public EnemyState currentState;
    protected Transform playerTransform;

    protected virtual void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public virtual void Move()
    {
        // Implementación básica o abstracta si se prefiere usar herencia
    }

    public virtual void Attack()
    {
        // Implementación básica o abstracta
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    public virtual void Die()
    {
        // Efectos de muerte, animaciones, sonidos, etc.
        Destroy(gameObject);
    }
}
