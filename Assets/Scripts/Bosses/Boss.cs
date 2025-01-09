using UnityEngine;

public class Boss : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private GameObject player;
    private BossLife bossLife;
    private float cooldown = 1;
    float currentAngle = 0f;
    [SerializeField] GameObject bossBulletPrefab;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        bossLife = GetComponent<BossLife>();
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
        FlipEnemy();

        if (bossLife.health > 1350)
        {
            //PhaseOne();
        }
        else if (bossLife.health > 500)
        {
            //PhaseTwo();
        }
        else
        {
            //PhaseThree();
        }
    }

    private void OnEnable()
    {
        bossLife.OnBossDead += HandleBossDeath;
    }

    private void OnDisable()
    {
        bossLife.OnBossDead -= HandleBossDeath;
    }

    void ShootRing(int bulletCount, float speed)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            SpawnBullet(transform.position, direction, speed);
        }
    }

    void ShootSpiral(float speed)
    {
        currentAngle += 10f;
        Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
        SpawnBullet(transform.position, direction, speed);
    }

    void ShootLine(int bulletCount, Vector2 direction, float spreadAngle, float speed)
    {
        float startAngle = -spreadAngle / 2;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + (spreadAngle / (bulletCount - 1)) * i;
            Vector2 spreadDirection = Quaternion.Euler(0, 0, angle) * direction;
            SpawnBullet(transform.position, spreadDirection, speed);
        }
    }

    void SpawnBullet(Vector2 position, Vector2 direction, float speed)
    {
        GameObject bossBullet = Instantiate(bossBulletPrefab, position, Quaternion.identity);
        bossBullet.GetComponent<BossBullet>().SetDirection(direction);
        bossBullet.GetComponent<BossBullet>().speed = speed;
    }

    private void FlipEnemy()
    {
        if (transform.position.x < player.transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else if (transform.position.x > player.transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void HandleBossDeath()
    {

    }
}
