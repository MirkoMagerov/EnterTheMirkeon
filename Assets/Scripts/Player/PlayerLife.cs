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

    public void TakeDamage(int damage)
    {
        if (!PlayerState.Instance.IsInvulnerable)
        {
            anim.SetTrigger("Hit");
            currentHealth -= damage;
            PlayerSounds.Instance.PlayHitSound();
            OnHealthChanged?.Invoke(currentHealth);
            if (currentHealth <= 0)
            {
                bool rationUsed = GetComponent<ConsumablesInventory>().UseRationBeforeDeath();
                if (rationUsed)
                {
                    return;
                }
                else
                {
                    Die();
                }
            }
        }
    }

    public void TakeDamage(int damage, GameObject obj)
    {
        if (!PlayerState.Instance.IsInvulnerable)
        {
            Debug.Log($"Taked damage from {obj.name}");
            anim.SetTrigger("Hit");
            currentHealth -= damage;
            PlayerSounds.Instance.PlayHitSound();
            OnHealthChanged?.Invoke(currentHealth);
            if (currentHealth <= 0)
            {
                bool rationUsed = GetComponent<ConsumablesInventory>().UseRationBeforeDeath();
                if (rationUsed)
                {
                    return;
                }
                else
                {
                    Die();
                }
            }
        }
    }

    public void RegenerateHealth(int restoredHP)
    {
        currentHealth = Mathf.Min(currentHealth + restoredHP, maxHealth);
        // Play regenerate sound
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void AddHearts(int hearts)
    {
        maxHealth += hearts;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        GameManager.Instance.ActivateDeathCanvas();
        Destroy(gameObject);
    }

    public int GetMaxHealth() { return maxHealth; }
}