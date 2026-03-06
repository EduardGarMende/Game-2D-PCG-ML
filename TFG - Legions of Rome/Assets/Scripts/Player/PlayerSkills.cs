using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkills : MonoBehaviour
{
    public PlayerMovement movement;
    public PlayerHealth health;
    public GameObject shieldVisual;

    // Dash parameters
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;
    private float nextDashTime = 0f;

    // Shield parameters
    public float shieldDuration = 2.5f;
    public float shieldCooldown = 5f;
    private float nextShieldTime = 0f;

    public static event Action<float> OnDashUsed;
    public static event Action<float> OnShieldUsed;

    private GameControls controls;

    private void Awake()
    {
        controls = new GameControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        PlayerHealth.OnShieldBroken += BreakShield;
    }

    private void OnDisable()
    {
        controls.Disable();
        PlayerHealth.OnShieldBroken -= BreakShield;
    }

    private void Update()
    {
        if (controls.Gameplay.Dash.triggered && Time.time >= nextDashTime && !movement.isDashing)
            StartCoroutine(DashRoutine());

        if (controls.Gameplay.Shield.triggered && Time.time >= nextShieldTime)
            StartCoroutine(ShieldRoutine());
    }

    IEnumerator DashRoutine()
    {
        movement.isDashing = true;
        health.isInvulnerable = true; // I-frames during dash
        movement.isMovementBloked = true; // Prevent normal movement during dash
        nextDashTime = Time.time + dashCooldown;

        OnDashUsed?.Invoke(dashCooldown); // Notify UI and Telemetry of dash usage

        // Fisically move the player in the dash direction
        Vector2 dashDirection = movement.facingDir;
        movement.rb.linearVelocity = dashDirection * dashForce;

        yield return new WaitForSeconds(dashDuration);

        movement.rb.linearVelocity = Vector2.zero; // Stop the dash movement
        health.isInvulnerable = false; // End I-frames
        movement.isMovementBloked = false; // Allow normal movement again
        movement.isDashing = false;
    }

    IEnumerator ShieldRoutine()
    {
        nextShieldTime = Time.time + shieldCooldown;
        health.isShieldActive = true;
        shieldVisual.SetActive(true); // Show shield visual

        OnShieldUsed?.Invoke(shieldCooldown); // Notify UI and Telemetry of shield usage

        yield return new WaitForSeconds(shieldDuration);

        if (health.isShieldActive) // If shield is still active after duration, break it
        {
            BreakShield();
        }
    }

    private void BreakShield()
    {
        health.isShieldActive = false;
        shieldVisual.SetActive(false); // Hide shield visual
    }
}
