using UnityEngine;

public class ExplosiveChaseState : ChaseState
{
    private float explosionRange;
    private bool isExploding = false;

    public ExplosiveChaseState(EnemyStateMachine stateMachine, Transform enemy, Transform player, Animator animator) 
        : base(stateMachine, enemy, player, animator)
    {
        explosionRange = stateMachine.explosionRange;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (Vector2.Distance(enemy.position, player.position) <= explosionRange)
        {
            TriggerExplosion();
            isExploding = true;
        }
        else if (!isExploding)
        {
            base.Update();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void TriggerExplosion()
    {
        if (!isExploding)
        {
            stateMachine.PlayExplosionAudio();
            enemy.GetComponent<BoxCollider2D>().enabled = false;
            enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            PlayAnimation("Explosion");

            if (Vector2.Distance(enemy.position, player.position) <= explosionRange)
            {
                PlayerLife playerHealth = player.GetComponent<PlayerLife>();
                playerHealth?.TakeDamage(2);
            }
        }
    }
}
