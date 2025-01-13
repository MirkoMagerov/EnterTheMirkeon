using UnityEngine;

public class AttackState : EnemyState
{
    private float attackCooldown = 1f;
    private float lastAttackTime;
    private float stayInAttackDistance = 18f;

    public AttackState(EnemyStateMachine stateMachine, Transform enemy, Transform player, Animator animator)
        : base(stateMachine, enemy, player, animator) { }

    public override void Enter()
    {
        PlayAnimation("Idle");
        lastAttackTime = Time.time;
    }

    public override void Update()
    {
        float distance = Vector2.Distance(enemy.position, player.position);

        if (distance > stayInAttackDistance)
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

    }

    private void Shoot()
    {
        Vector2 direction = (player.position - enemy.position).normalized;
        GameObject bullet = Object.Instantiate(
            Resources.Load<GameObject>("EnemyBulletPrefab"),
            stateMachine.bulletSpawnPoint.transform.position,
            Quaternion.identity
        );

        bullet.GetComponent<EnemyBullet>().SetDirection(direction);
        Object.Destroy(bullet, 6f);
    }
}
