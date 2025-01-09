using System;
using UnityEngine;
using UnityEngine.UI;

public class BossLife : MonoBehaviour, IDamageable
{
    public event Action OnBossDead;

    [SerializeField] public float health;
    [SerializeField] private Slider slider;
    private Boss boss;

    private void Start()
    {
        slider.maxValue = health;
        slider.value = health;
        slider.gameObject.SetActive(false);
        boss = GetComponent<Boss>();
    }

    public void TakeDamage(float damage)
    {
        if (!slider.IsActive()) slider.gameObject.SetActive(true);
        health -= damage;
        slider.value = Math.Max(0, health);
        if (health <= 0) Die();
    }

    private void Die()
    {
        OnBossDead?.Invoke();
        boss.GetComponentInChildren<Animator>().Play("Die");
    }
}
