using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlip : MonoBehaviour
{
    public void SetPlayerLocalScale(Vector3 newLocalScale)
    {
        transform.localScale = new(newLocalScale.x, newLocalScale.y, newLocalScale.z);
    }

    public Vector3 GetPlayerLocalScale()
    {
        return transform.localScale;
    }
}
