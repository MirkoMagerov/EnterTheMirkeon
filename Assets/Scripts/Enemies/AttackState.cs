using UnityEngine;

public class AttackState : EnemyState
{
    private float attackCooldown = 1.5f;
    private float lastAttackTime;

    public AttackState(EnemyStateMachine stateMachine, Transform enemy, Transform player, Animator animator) 
        : base(stateMachine, enemy, player, animator) { }

    public override void Enter()
    {
        Debug.Log("Entering Attack State");
        lastAttackTime = Time.time;
    }

    public override void Update()
    {
        if (Vector2.Distance(enemy.position, player.position) > 5f)
        {
            stateMachine.ChangeState(new ChaseState(stateMachine, enemy, player, animator));
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Shoot();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Attack State");
    }

    private void Shoot()
    {
        Debug.Log("Enemy Shoots!");
        // Lógica para disparar balas
    }
}
