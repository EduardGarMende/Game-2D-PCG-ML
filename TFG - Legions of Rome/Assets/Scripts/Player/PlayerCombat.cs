using UnityEngine;
using System;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public PlayerVisuals visuals;
    public PlayerMovement playerMovement;
    public GameObject swordVisuals;

    public float atacDuration = 0.4f;
    public float attackDelay = 0.2f;
    public float attackDamage = 20f;
    public float rangeAttackDamage = 35f;

    public Transform attackCenter;
    public float attackRadius = 0.6f;
    public float attackOffset = 0.8f;
    public LayerMask enemyLayer;

    public GameObject spearPrefab;
    public float throwDuration = 0.4f;
    public float rangedCooldown = 2f;
    private float nextRangedTime = 0f;

    private bool isAttacking = false;
    private GameControls controls;
    public static event Action<float> OnRangedUsed;

    private void Awake() => controls = new GameControls();

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        if (isAttacking) return;
        if (playerMovement.isDead) return;

        if (controls.Gameplay.Attack.triggered)
        {
            StartCoroutine(Attack());
        }

        if (controls.Gameplay.RangedAttack.triggered && Time.time >= nextRangedTime)
        {
            StartCoroutine(ThrowSpear());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        playerMovement.isMovementBloked = true;
        visuals.TriggerAttack();

        yield return new WaitForSeconds(attackDelay);

        Vector2 attackPoint = (Vector2)attackCenter.position + (playerMovement.facingDir * attackOffset);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, attackRadius, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
            }
        }

        yield return new WaitForSeconds(atacDuration - attackDelay);

        playerMovement.isMovementBloked = false;
        isAttacking = false;
    }

    IEnumerator ThrowSpear()
    {
        isAttacking = true;
        playerMovement.isMovementBloked = true;

        nextRangedTime = Time.time + rangedCooldown;

        OnRangedUsed?.Invoke(rangedCooldown);

        if (swordVisuals != null)
        {
            swordVisuals.GetComponent<SpriteRenderer>().enabled = false;
        }

        visuals.TriggerAttack();

        yield return new WaitForSeconds(0.1f);

        Vector2 spawnPosition = (Vector2)attackCenter.position + (playerMovement.facingDir * attackOffset);
        GameObject spearInstance = Instantiate(spearPrefab, spawnPosition, Quaternion.identity);

        Projectile spearProjectile = spearInstance.GetComponent<Projectile>();
        if (spearProjectile != null)
        {
            spearProjectile.Setup(playerMovement.facingDir, rangeAttackDamage);
        }

        yield return new WaitForSeconds(throwDuration - 0.1f);

        if (swordVisuals != null)
        {
            swordVisuals.GetComponent<SpriteRenderer>().enabled = true;
        }

        playerMovement.isMovementBloked = false;
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackCenter == null) return;

        Vector2 dir = Application.isPlaying && playerMovement != null ? playerMovement.facingDir : Vector2.down;
        Vector2 attackPoint = (Vector2)attackCenter.position + (dir * attackOffset);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint, attackRadius);
    }

    public void IncreaseSwordDamage(float amount)
    {
        attackDamage += amount;
    }

    public void IncreaseRangeDamage(float amount)
    {
        rangeAttackDamage += amount;
    }
}
