using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    event Action<IPoolable> OnReturnToPool;
    void ReturnToPool();
}
