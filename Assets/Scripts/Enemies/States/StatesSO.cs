using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatesSO : ScriptableObject
{
    public List<StatesSO> StatesToGo;
    public abstract void OnStateEnter(EnemyController ec);
    public abstract void OnStateUpdate(EnemyController ec);
    public abstract void OnStateExit(EnemyController ec);
}
