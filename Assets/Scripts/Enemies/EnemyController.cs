using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public StatesSO CurrentState;
    public GameObject target;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            target = collision.gameObject;
            GoToState<ChaseState>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GoToState<IdleState>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GoToState<AttackState>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GoToState<ChaseState>();
    }

    public void CheckIfAlife()
    {
        
    }

    private void Update()
    {
        CurrentState.OnStateUpdate(this);
    }

    public void GoToState<T>() where T : StatesSO
    {
        if (CurrentState.StatesToGo.Find(state => state is T))
        {
            CurrentState.OnStateExit(this);
            CurrentState = CurrentState.StatesToGo.Find(obj => obj is T);
            CurrentState.OnStateEnter(this);
        }
    }
}