using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    private bool canPierce;
    private WeaponController weapon;
    private float maxLifeTime = 5f;

    private void Update()
    {
        maxLifeTime -= Time.deltaTime;
        if (maxLifeTime <= 0)
        {
            ReturnToPool();
        }
    }

    public void Initialize(AmmoSO ammoData)
    {
        damage = ammoData.damage;
        canPierce = ammoData.canPierce;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            //Enemy enemy = collision.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    enemy.TakeDamage(damage);
            //}

            if (!canPierce)
            {
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {
        weapon.ReturnBulletToPool(gameObject);
    }

    public void SetWeapon(WeaponController weapon)
    {
        this.weapon = weapon;
    }
}
