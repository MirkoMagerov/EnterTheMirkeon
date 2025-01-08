using UnityEngine;

public class ChaseState : EnemyState
{
    private float chaseSpeed = 5f;
    private float attackRange = 5f;
    private bool melee;
    private bool explosive;

    public ChaseState(EnemyStateMachine stateMachine, Transform enemy, Transform player, Animator animator)
        : base(stateMachine, enemy, player, animator) {
        melee = stateMachine.melee;
        explosive = stateMachine.explosive;
    }

    public override void Enter()
    {
        PlayAnimation("Chase");
    }

    public override void Update()
    {
        if (melee || explosive)
        {
            Vector2 direction = (player.position - enemy.position).normalized;
            enemy.GetComponent<Rigidbody2D>().velocity = direction * chaseSpeed;
        }
        else
        {
            if (Vector2.Distance(enemy.position, player.position) <= attackRange)
            {
                stateMachine.ChangeState(new AttackState(stateMachine, enemy, player, animator));
            }
            else
            {
                Vector2 direction = (player.position - enemy.position).normalized;
                enemy.GetComponent<Rigidbody2D>().velocity = direction * chaseSpeed;
            }
        }
    }

    public override void Exit()
    {
        enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
