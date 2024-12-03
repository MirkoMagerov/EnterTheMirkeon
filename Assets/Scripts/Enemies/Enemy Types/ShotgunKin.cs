using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunKin : Enemy
{
    public float attackInterval = 3f;
    public int bulletCount = 5;
    public float spreadAngle = 45f;

    private float attackTimer;

    protected override void Start()
    {
        base.Start();
        attackTimer = attackInterval;
    }

    private void Update()
    {
        Move();
        HandleAttackTimer();
    }

    public override void Move()
    {
        // Se mueve hacia el jugador
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        transform.Translate(speed * Time.deltaTime * direction);
    }

    private void HandleAttackTimer()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = attackInterval;
        }
    }

    public override void Attack()
    {
        float angleStep = spreadAngle / (bulletCount - 1);
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float startAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - spreadAngle / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            GameObject bullet = BulletPool.Instance.GetBullet();
            bullet.transform.SetPositionAndRotation(transform.position, rotation);
            bullet.SetActive(true);
        }
    }
}
