using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLife : MonoBehaviour, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject deathParticlesPrefab;
    private Rigidbody2D rb;

    private void Start()
    {
        slider.maxValue = health;
        slider.value = health;
        slider.gameObject.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        if (!slider.IsActive()) slider.gameObject.SetActive(true);
        health -= damage;
        health = Mathf.Max(0, health);
        slider.value = Math.Max(0, health);
        if (health <= 0) Die();
    }

    private void Die()
    {
        rb.velocity = Vector3.zero;
        slider.gameObject.SetActive(false);
        GameObject deathParticles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        Destroy(deathParticles, 3);
        Instantiate(coin, transform.position, Quaternion.identity);
        coin.GetComponent<Coin>().SetRandomCoins(2, 5);
        Destroy(gameObject);
    }
}
