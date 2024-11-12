using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlip : MonoBehaviour
{
    [SerializeField] private GameObject playerSpriteGameObject;

    public void SetPlayerSpriteLocalScale(Vector3 newLocalScale)
    {
        playerSpriteGameObject.transform.localScale = new(newLocalScale.x, newLocalScale.y, newLocalScale.z);
    }

    public Vector3 GetPlayerSpriteScale()
    {
        return playerSpriteGameObject.transform.localScale;
    }
}
