using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }
    public float armor = 0f;

    public bool isInvulnerable = false;
    public bool isShieldActive = false;
    public float shieldProtection = 0.5f;

    public PlayerMovement movement;
    public PlayerVisuals visuals;

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
            damage = (1f - shieldProtection) * damage;
            OnShieldBroken?.Invoke();
        }

        damage = Mathf.Max(1f, damage - armor);

        currentHealth -= damage;
        if (GameModeManager.Instance != null && GameModeManager.Instance.currentMode == GameModeManager.GameMode.GodMode)
        {
            currentHealth = Mathf.Clamp(currentHealth, 1f, maxHealth); // Nunca baja de 1
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }

        OnDamageTaken?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (movement.isDead) return;

        movement.isDead = true;
        visuals.TriggerDeath();

        OnPlayerDeath?.Invoke();
        
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void IncreaseMaxHelath(float amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void AddArmor(float amount)
    {
        armor += amount;
    }

    public void ImproveShield(float amount)
    {
        shieldProtection = Mathf.Clamp(shieldProtection + amount, 0, 1);
    }

    public void ResetToDefault()
    {
        currentHealth = maxHealth;
        armor = 0f;
        shieldProtection = 0.5f;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
