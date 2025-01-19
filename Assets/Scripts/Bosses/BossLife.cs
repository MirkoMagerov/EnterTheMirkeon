using System;
using UnityEngine;
using UnityEngine.UI;

public class BossLife : MonoBehaviour, IDamageable
{
    public event Action OnBossDead;

    [SerializeField] private Color secondPhaseColor;
    [SerializeField] private Color thirdPhaseColor;
    [SerializeField] public float health;
    [SerializeField] private Slider slider;
    private SpriteRenderer spriteRenderer;
    private Boss boss;

    private void Start()
    {
        slider.maxValue = health;
        slider.value = health;
        boss = GetComponent<Boss>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage, GameObject obj)
    {
        int damageReduction = boss.GetDamageReduction();
        int actualDamage = damage * (100 - damageReduction) / 100;

        health -= actualDamage;
        slider.value = Math.Max(0, health);
        if (health <= 0) Die();
    }

    public void ChangeColorPhase(int phase)
    {
        switch(phase)
        {
            case 2:
                spriteRenderer.color = secondPhaseColor;
                break;
            case 3:
                spriteRenderer.color = thirdPhaseColor;
                break;
            default: break;
        }
    }

    private void Die()
    {
        OnBossDead?.Invoke();
        this.enabled = false;
    }
}
