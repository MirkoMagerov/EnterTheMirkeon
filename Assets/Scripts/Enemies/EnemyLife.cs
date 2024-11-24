using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLife : MonoBehaviour, IDamageable
{
    private int health;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= Mathf.Max(health - damage, 0);
        if (health == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy dead");
    }
}
