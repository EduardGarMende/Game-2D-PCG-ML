using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }

    public bool isInvulnerable = false;
    public bool isShieldActive = false;

    public static event Action<float, float> OnHealthChanged;
    public static event Action<float> OnDamageTaken;
    public static event Action OnShieldBroken;
    public static event Action OnPlayerDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        if (isShieldActive)
        {
            isShieldActive = false;
            currentHealth -= 0.25f * damage;
            OnShieldBroken?.Invoke();
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnDamageTaken?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        // Additional death logic (e.g., play animation, disable controls) can be added here
    }
}
