using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBehaviour : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Chase(Transform target, Transform self)
    {
        rb.velocity = (target.position - self.position).normalized * moveSpeed;
        transform.right = (target.position - self.position).normalized;
    }

    public void Run(Transform target, Transform self)
    {
        
    }

    public void StopChasing()
    {
        
    }
}
