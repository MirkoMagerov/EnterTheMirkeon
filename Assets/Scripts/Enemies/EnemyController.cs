using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public StatesSO currentState;
    public GameObject target;
    public Animator anim;

    [SerializeField] private int health = 100;
    [SerializeField] private float attackDistance = 1f;

    private float distanceToPlayer;

    private void Start()
    {
        anim = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        currentState.OnStateUpdate(this);

        if (target != null)
        {
            distanceToPlayer = Vector2.Distance(transform.position, target.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && distanceToPlayer > attackDistance)
        {
            target = collision.gameObject;
            GoToState<ChaseState>();
        }
        else if (collision.gameObject.CompareTag("Player") && distanceToPlayer <= attackDistance)
        {
            target = collision.gameObject;
            GoToState<AttackState>();
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

    public bool IsDead()
    {
        return health <= 0;
    }

    public void GoToState<T>() where T : StatesSO
    {
        if (currentState.StatesToGo.Find(state => state is T))
        {
            currentState.OnStateExit(this);
            currentState = currentState.StatesToGo.Find(obj => obj is T);
            currentState.OnStateEnter(this);
        }
    }
}