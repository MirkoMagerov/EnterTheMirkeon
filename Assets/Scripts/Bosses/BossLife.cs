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
        boss = GetComponent<Boss>();
    }

    public void TakeDamage(int damage, GameObject obj)
    {
        int damageReduction = boss.GetDamageReduction();
        int actualDamage = damage * (100 - damageReduction) / 100;

        health -= actualDamage;
        slider.value = Math.Max(0, health);
        if (health <= 0) Die();
    }

    private void Die()
    {
        OnBossDead?.Invoke();
        boss.GetComponentInChildren<Animator>().Play("Die");
    }
}
