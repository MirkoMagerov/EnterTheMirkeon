using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLife : MonoBehaviour, IDamageable
{
    private float health;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("Enemy dead");
    }
}
