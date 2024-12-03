using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    private Vector2 moveDirection;

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction;
        Invoke(nameof(Deactivate), lifetime);
    }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * moveDirection);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Manejar colisiones
        Deactivate();
    }
}
