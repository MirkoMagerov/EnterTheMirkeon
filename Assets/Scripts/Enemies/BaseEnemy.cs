using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    [Header("Base Enemy Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float attackCooldown = 2f;

    protected float currentHealth;
    protected Transform playerTransform;

    public virtual void Initialize(Transform player)
    {
        playerTransform = player;
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"ENEMY DAMAGED - Damage: {damage}, total HP: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        // Lógica base de muerte
        gameObject.SetActive(false);
    }

    public abstract void Attack();
    public abstract void UpdateBehavior();
}
