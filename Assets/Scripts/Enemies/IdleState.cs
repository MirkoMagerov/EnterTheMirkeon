using UnityEngine;

public class IdleState : EnemyState
{
    public IdleState(EnemyStateMachine stateMachine, Transform enemy, Transform player, Animator animator)
        : base(stateMachine, enemy, player, animator) { }

    public override void Enter()
    {
        PlayAnimation("Idle");
    }

    public override void Exit()
    {
        
    }
}
