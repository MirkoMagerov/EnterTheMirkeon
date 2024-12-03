using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    public BulletKin.BulletKinState CurrentState { get; private set; }

    public void ChangeState(BulletKin.BulletKinState newState)
    {
        CurrentState = newState;
    }
}
