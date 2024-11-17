using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class BulletBehavior : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    private float lifetime;

    public void Initialize(Vector2 dir, Ammo ammo)
    {
        direction = dir.normalized;
        speed = ammo.speed;
        damage = ammo.damage;
        lifetime = ammo.lifetime;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Enemy"))
        //{
        //    Enemy enemy = collision.GetComponent<Enemy>();
        //    if (enemy != null)
        //    {
        //        enemy.TakeDamage(damage);
        //    }
        //    Destroy(gameObject);
        //}
    }
}
