using UnityEngine;

public abstract class EnemyState
{
    protected EnemyStateMachine stateMachine;
    protected Transform enemy;
    protected Transform player;
    protected Animator animator;

    public EnemyState(EnemyStateMachine stateMachine, Transform enemy, Transform player, Animator animator)
    {
        this.stateMachine = stateMachine;
        this.enemy = enemy;
        this.player = player;
        this.animator = animator;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    protected void PlayAnimation(string animationName)
    {
        animator?.Play(animationName);
    }
}
