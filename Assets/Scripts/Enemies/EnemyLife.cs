using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLife : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject coin;

    private void Start()
    {
        slider.maxValue = health;
        slider.value = health;
        slider.gameObject.SetActive(false);
    }

    public void TakeDamage(int damage, GameObject obj)
    {
        if (!slider.IsActive()) slider.gameObject.SetActive(true);
        health -= damage;
        slider.value = Math.Max(0, health);
        if (health <= 0) Die();
    }

    private void Die()
    {
        slider.gameObject.SetActive(false);
        Instantiate(coin, transform.position, Quaternion.identity);
        coin.GetComponent<Coin>().SetRandomCoins(2, 5);
        Destroy(gameObject);
    }
}
