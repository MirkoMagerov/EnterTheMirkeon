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
    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        currentState = new IdleState(this, transform, player, animator);
    }

    private void Start()
    {
        if (explosive)
        {
            ChangeState(new ExplosiveChaseState(this, transform, player, animator));
        }
        else
        {
            ChangeState(new ChaseState(this, transform, player, animator));
        }
    }

    private void Update()
    {
        FlipEnemy();
        currentState.Update();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void FlipEnemy()
    {
        if (transform.position.x < player.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else if (transform.position.x > player.position.x)
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
