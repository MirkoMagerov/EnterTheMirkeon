using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float stoppingDistance = 5f;
    public float moveSpeed = 3f;
    public float fireRate = 1.5f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public Ammo ammo;

    private Transform player;
    private Rigidbody2D rb;
    private float nextFireTime;
    //private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null)
        {
            // El jugador no está en la escena
            rb.velocity = Vector2.zero;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            if (distanceToPlayer > stoppingDistance)
            {
                // Perseguir al jugador
                MoveTowardsPlayer();
                //animator.SetBool("isMoving", true);
            }
            else
            {
                // Detenerse y disparar
                rb.velocity = Vector2.zero;
                //animator.SetBool("isMoving", false);

                // Mirar hacia el jugador
                FlipSprite();

                // Disparar
                ShootAtPlayer();
            }
        }
        else
        {
            // No detecta al jugador
            rb.velocity = Vector2.zero;
            //animator.SetBool("isMoving", false);
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        FlipSprite();
    }

    void FlipSprite()
    {
        if (player.position.x < transform.position.x)
        {
            // El jugador está a la izquierda
            spriteRenderer.flipX = true;
        }
        else
        {
            // El jugador está a la derecha
            spriteRenderer.flipX = false;
        }
    }

    void ShootAtPlayer()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;

            // Iniciar animación de disparo
            //animator.SetTrigger("Shoot");
            FireBullet();
            Debug.Log("Enemy has shoot!");
        }
    }

    public void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        bullet.GetComponent<BulletBehavior>().Initialize(direction, ammo, "Enemy");
    }
}
