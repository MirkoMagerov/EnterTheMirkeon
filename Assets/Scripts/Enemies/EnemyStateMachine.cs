using System.Collections;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public bool melee;
    public bool explosive;
    public float explosionRange;
    public float knockbackStrength;
    private bool isDealingDamage = false;

    private EnemyState currentState;
    private GameObject player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool playerAlive = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        currentState = new IdleState(this, transform, player.transform, animator);
    }

    private void Start()
    {
        if (explosive)
        {
            ChangeState(new ExplosiveChaseState(this, transform, player.transform, animator));
        }
        else
        {
            ChangeState(new ChaseState(this, transform, player.transform, animator));
        }
    }

    private void Update()
    {
        if (!playerAlive) return;
        FlipEnemy();
        currentState.Update();
    }

    private void OnEnable()
    {
        player.GetComponent<PlayerLife>().OnPlayerDeath += PlayerDead;
    }

    private void OnDisable()
    {
        player.GetComponent<PlayerLife>().OnPlayerDeath -= PlayerDead;
    }

    public void ChangeState(EnemyState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void PlayerDead()
    {
        playerAlive = false;
        ChangeState(new IdleState(this, transform, player.transform, animator));
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

    private IEnumerator DealContinuousDamage(PlayerLife playerLife, PlayerMovement playerMov, Vector2 knockbackDirection)
    {
        isDealingDamage = true;

        while (isDealingDamage)
        {
            playerLife.TakeDamage(1);
            playerMov.ApplyKnockback(knockbackDirection * knockbackStrength);

            yield return new WaitForSeconds(1f);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
            PlayerMovement playerMov = collision.gameObject.GetComponent<PlayerMovement>();
            PlayerLife playerLife = collision.gameObject.GetComponent<PlayerLife>();

            if (!isDealingDamage)
            {
                StartCoroutine(DealContinuousDamage(playerLife, playerMov, knockbackDirection));
            }
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isDealingDamage = false;
        }
    }

    public void DestroyGameObject()
    {
        Debug.Log("Destroying explosive enemy");
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (explosive)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, explosionRange);
        }
    }
}
