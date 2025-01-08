using System;
using UnityEngine;

public class PlayerLife : MonoBehaviour, IDamageable
{
    public event Action<int> OnHealthChanged;
    public event Action OnPlayerDeath;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 6;
    private int currentHealth;

    private Animator anim;

    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (!PlayerState.Instance.IsInvulnerable)
        {
            anim.SetTrigger("Hit");
            currentHealth -= (int)damage;
            PlayerSounds.Instance.PlayHitSound();
            OnHealthChanged.Invoke(currentHealth);
            if (currentHealth <= 0) Die();
        }
    }

    public void RegenerateHealth(int restoredHP)
    {
        currentHealth = Mathf.Min(currentHealth + restoredHP, maxHealth);
        // Play regenerate sound
        OnHealthChanged.Invoke(currentHealth);
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        Destroy(gameObject);
    }

    public int GetMaxHealth() { return maxHealth; }
}