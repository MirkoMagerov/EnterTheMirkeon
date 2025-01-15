using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private BossLife bossLife;

    [SerializeField] private GameObject bossBulletPrefab;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float phaseOneAttackCooldown;
    [SerializeField] private float phaseTwoAttackCooldown;
    [SerializeField] private float phaseThreeAttackCooldown;
    [SerializeField] private Vector2 movementBounds = new(0, 0);

    private bool playerAlive = true;
    private bool lastMoveRight = true;
    private int damageReduction = 0;
    private float attackCooldown;
    private float spiralAngle = 0f;
    private Coroutine mainCoroutine;
    private GameObject playerTarget;

    private void Awake()
    {
        bossLife = GetComponent<BossLife>();
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerTarget = GameObject.FindGameObjectWithTag("EnemyTarget");

        StartMainCoroutine();
    }

    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>().OnPlayerDeath += OnPlayerDeath;
        bossLife.OnBossDead += OnBossDeath;
    }

    private void OnDisable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>().OnPlayerDeath -= OnPlayerDeath;
        bossLife.OnBossDead -= OnBossDeath;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && bossLife.health > 0)
        {
            PlayerLife playerLife = collision.gameObject.GetComponent<PlayerLife>();

            if (playerLife != null)
            {
                int damage = 1;
                playerLife.TakeDamage(damage, gameObject);
            }
        }
    }

    private void StartMainCoroutine()
    {
        if (mainCoroutine != null) StopCoroutine(mainCoroutine);
        mainCoroutine = StartCoroutine(BossBehavior());
    }

    private IEnumerator BossBehavior()
    {
        while (bossLife.health > 0 && playerAlive)
        {
            if (bossLife.health > 2850)
            {
                yield return PhaseOne();
            }
            else if (bossLife.health > 1350)
            {
                yield return PhaseTwo();
            }
            else
            {
                yield return PhaseThree();
            }
        }
    }

    private IEnumerator Move()
    {
        anim.SetBool("Moving", true);

        float rangeStart = transform.position.x - Mathf.Abs(movementBounds.x);
        float rangeEnd = transform.position.x + Mathf.Abs(movementBounds.y);

        float targetX = lastMoveRight ? rangeEnd : rangeStart;
        lastMoveRight = !lastMoveRight;

        FlipEnemy(targetX > transform.position.x);

        Vector2 targetPosition = new(targetX, transform.position.y);

        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        anim.SetBool("Moving", false);
        yield return new WaitForSeconds(.75f);
    }

    private IEnumerator PhaseOne()
    {
        attackCooldown = phaseOneAttackCooldown;

        while (bossLife.health > 2850 && playerAlive)
        {
            yield return Move();
            yield return AttackPatternOne();
        }
    }

    private IEnumerator PhaseTwo()
    {
        attackCooldown = phaseTwoAttackCooldown;
        damageReduction = 15;

        while (bossLife.health > 1350 && playerAlive)
        {
            yield return Move();
            yield return AttackPatternTwo();
        }
    }

    private IEnumerator PhaseThree()
    {
        attackCooldown = phaseThreeAttackCooldown;
        damageReduction = 25;

        while (bossLife.health > 0 && playerAlive)
        {
            yield return Move();
            yield return AttackPatternThree();
        }
    }

    private IEnumerator AttackPatternOne()
    {
        anim.SetTrigger("Attack");
        ShootRing(30, 11f);
        yield return new WaitForSeconds(0.7f);

        anim.SetTrigger("Attack");
        for (int i = 0; i < 45; i++)
        {
            ShootSpiral(10f);
            yield return new WaitForSeconds(0.09f);
        }
        spiralAngle = 0f;

        yield return new WaitForSeconds(0.3f);
        anim.SetTrigger("Attack");
        ShootRing(30, 11f);

        yield return new WaitForSeconds(attackCooldown);
    }

    private IEnumerator AttackPatternTwo()
    {
        anim.SetTrigger("Attack");
        ShootRing(35, 8f);

        yield return new WaitForSeconds(0.3f);
        anim.SetTrigger("Attack");
        ShootRing(35, 12.5f);

        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < 75; i++)
        {
            ShootSpiral(12f);
            yield return new WaitForSeconds(0.065f);
        }
        spiralAngle = 0f;

        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        ShootRing(35, 9f);

        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("Attack");
        ShootRing(35, 13.5f);
        yield return new WaitForSeconds(attackCooldown);
    }

    private IEnumerator AttackPatternThree()
    {
        anim.SetTrigger("Attack");

        yield return ShootSequentially(playerTarget.transform, 25, 15f, 0.15f, 20);

        yield return new WaitForSeconds(0.4f);
        ShootRing(35, 14f);

        for (int i = 0; i < 70; i++)
        {
            ShootSpiral(i % 2 == 0 ? 12f : 8f);
            yield return new WaitForSeconds(0.045f);
        }
        spiralAngle = 0f;

        yield return new WaitForSeconds(0.3f);
        ShootRing(40, 16f);
        yield return new WaitForSeconds(attackCooldown);
    }

    private void ShootRing(int bulletCount, float speed)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount);
            Vector2 direction = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            SpawnBullet(transform.position, direction, speed);
        }
    }

    private void ShootSpiral(float speed)
    {
        float angleIncrement = 10f;
        spiralAngle += angleIncrement;
        spiralAngle %= 360f;

        Vector2 direction = new(
            Mathf.Cos(spiralAngle * Mathf.Deg2Rad),
            Mathf.Sin(spiralAngle * Mathf.Deg2Rad)
        );

        SpawnBullet(transform.position, direction, speed);
    }

    private IEnumerator ShootSequentially(Transform playerTransform, int bulletCount, float bulletSpeed, float interval, float spreadAngle)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            Vector2 playerPosition = playerTransform.position;
            Vector2 directionToPlayer = (playerPosition - (Vector2)transform.position).normalized;

            float randomAngle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            Vector2 spreadDirection = RotateVector(directionToPlayer, randomAngle);

            SpawnBullet(transform.position, spreadDirection, bulletSpeed);

            yield return new WaitForSeconds(interval);
        }
    }

    private Vector2 RotateVector(Vector2 v, float angleInDegrees)
    {
        float radians = angleInDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    void SpawnBullet(Vector2 position, Vector2 direction, float speed)
    {
        GameObject bossBullet = Instantiate(bossBulletPrefab, position, Quaternion.identity);
        bossBullet.GetComponent<BossBullet>().SetDirectionAndDamage(direction, 1);
        bossBullet.GetComponent<BossBullet>().speed = speed;
    }

    private void FlipEnemy(bool right)
    {
        spriteRenderer.flipX = right;
    }

    public int GetDamageReduction() { return damageReduction; }

    private void OnBossDeath()
    {
        StopCoroutine(mainCoroutine);
        anim.SetBool("Moving", false);
        anim.Play("Dead");
        GameManager.Instance.ChangeSceneWithTransition("Credits", () =>
        {
            GameManager.Instance.AfterBossDefeath();
        });
    }

    private void OnPlayerDeath()
    {
        playerAlive = false;
        playerTarget = null;
        StopCoroutine(mainCoroutine);
        anim.SetBool("Moving", false);
        anim.Play("Idle");
    }
}
