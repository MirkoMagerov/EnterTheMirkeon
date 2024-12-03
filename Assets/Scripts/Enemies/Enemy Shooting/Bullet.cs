using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public float damage = .5f;

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * transform.up, Space.World);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (gameObject.CompareTag("Player"))
        {
            PlayerLife playerLife = gameObject.GetComponent<PlayerLife>();
            PlayerDash playerDash = gameObject.GetComponent<PlayerDash>();

            if (playerDash.GetIsInvulnerable() == false)
            {
                playerLife.TakeDamage(damage);
            }

            Deactivate();
        }
    }
}
