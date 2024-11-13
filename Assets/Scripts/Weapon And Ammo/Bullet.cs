using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private int damage;
    private bool canPierce;
    private WeaponController weapon;

    public void Initialize(AmmoSO ammoData)
    {
        speed = ammoData.speed;
        damage = ammoData.damage;
        canPierce = ammoData.canPierce;
    }

    void Update()
    {
        // Mover la bala
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Lógica de impacto
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

        if (collision.CompareTag("Wall") && !canPierce)
        {
            ReturnToPool();
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
