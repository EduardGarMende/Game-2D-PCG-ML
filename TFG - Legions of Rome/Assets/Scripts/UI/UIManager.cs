using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public Slider healthBar;
    public Image dashCooldownImage;
    public Image shieldCooldownImage;
    public Image rangedCooldownImage;

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateHealthUI;
        PlayerSkills.OnDashUsed += StartDashCooldown;
        PlayerSkills.OnShieldUsed += StartShieldCooldown;   
        PlayerCombat.OnRangedUsed += StartRangedCooldown;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateHealthUI;
        PlayerSkills.OnDashUsed -= StartDashCooldown;
        PlayerSkills.OnShieldUsed -= StartShieldCooldown;
        PlayerCombat.OnRangedUsed -= StartRangedCooldown;
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthBar.value = currentHealth / maxHealth;
    }

    private void StartDashCooldown(float cooldown)
    {
        StartCoroutine(CooldownRoutine(dashCooldownImage, cooldown));
    }

    private void StartShieldCooldown(float cooldown)
    {
        StartCoroutine(CooldownRoutine(shieldCooldownImage, cooldown));
    }

    private void StartRangedCooldown(float cooldown)
    {
        StartCoroutine(CooldownRoutine(rangedCooldownImage, cooldown));
    }

    IEnumerator CooldownRoutine(Image cooldownImage, float cooldownDuration)
    {
        cooldownImage.fillAmount = 0f;
        float elapsed = 0f;
        while (elapsed < cooldownDuration)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = elapsed / cooldownDuration;
            yield return null;
        }
        cooldownImage.fillAmount = 1f;
    }
}
