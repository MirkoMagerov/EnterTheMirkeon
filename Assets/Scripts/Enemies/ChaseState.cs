using UnityEngine;

public class ChaseState : EnemyState
{
    private float chaseSpeed = 8f;
    private float optimalDistance = 17f;
    private bool melee;
    private bool explosive;
    private Rigidbody2D rb;

    public ChaseState(EnemyStateMachine stateMachine, Transform enemy, Transform player, Animator animator)
        : base(stateMachine, enemy, player, animator) {
        melee = stateMachine.melee;
        explosive = stateMachine.explosive;
        rb = enemy.GetComponent<Rigidbody2D>();
    }

    public override void Enter()
    {
        PlayAnimation("Chase");
    }

    public override void Update()
    {
        Vector2 direction = (player.position - enemy.position).normalized;

        if (melee || explosive)
        {
            rb.velocity = direction * chaseSpeed;
        }
        else
        {
            float distance = Vector2.Distance(enemy.position, player.position);

            if (distance <= optimalDistance)
            {
                rb.velocity = Vector2.zero;
                stateMachine.ChangeState(new AttackState(stateMachine, enemy, player, animator));
                return;
            }

            rb.velocity = direction * chaseSpeed;
        }
    }

    public override void Exit()
    {
        enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
