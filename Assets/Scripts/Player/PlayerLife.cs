using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Damage taken: {damage}, Total HP: {health}");
        if (health <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("Player is dead");
    }
}