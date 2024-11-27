using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementPattern { Straight, ZigZag, Circles }

public class EnemyAI : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float moveSpeed = 3f;
    public float fireRate = 1.5f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public Ammo ammo;
    public MovementPattern movementPattern;

    private float nextFireTime;
    private bool seenPlayer = false;
    private Transform player;
    //private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

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
            rb.velocity = Vector2.zero;
            return;
        }

        FlipSprite();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius || seenPlayer)
        {
            seenPlayer = true;
            MoveTowardsPlayer();
            ShootAtPlayer();
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

        switch (movementPattern)
        {
            case MovementPattern.Straight:
                rb.velocity = direction * moveSpeed;
                break;
            case MovementPattern.ZigZag:
                Vector2 zigzagOffset = new Vector2(Mathf.Sin(Time.time * 5f), 0) * 2f;
                rb.velocity = (direction + zigzagOffset).normalized * moveSpeed;
                break;
            case MovementPattern.Circles:
                rb.velocity = new Vector2(Mathf.Cos(Time.time * moveSpeed), Mathf.Sin(Time.time * moveSpeed)) * moveSpeed;
                break;
        }
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

    void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Vector2 direction = (player.position - transform.position).normalized;
        float inaccuracy = Random.Range(-5f, 5f); // Variación en grados
        direction = Quaternion.Euler(0, 0, inaccuracy) * direction;

        bullet.GetComponent<BulletBehavior>().Initialize(direction, ammo, "Enemy");
    }
}
