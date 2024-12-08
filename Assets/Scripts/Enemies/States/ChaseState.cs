using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaseState", menuName = "StatesSO/Chase")]
public class ChaseState : StatesSO
{
    public override void OnStateEnter(EnemyController ec)
    {
        Debug.Log($"Entering chase state");
    }

    public override void OnStateExit(EnemyController ec)
    {
        ec.GetComponent<ChaseBehaviour>().StopChasing();
    }

    public override void OnStateUpdate(EnemyController ec)
    {
        ec.GetComponent<ChaseBehaviour>().Chase(ec.target.transform, ec.transform);
    }
}