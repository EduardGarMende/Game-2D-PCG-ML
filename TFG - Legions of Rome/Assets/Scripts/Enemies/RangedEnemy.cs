using UnityEngine;
using System.Collections;

public class RangedEnemy : Enemy
{
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float attackDelay = 0.3f;

    protected override void AttackPlayer()
    {
        TriggerAttackAnimation();
        StartCoroutine(ShootCorrutine());
    }

    private IEnumerator ShootCorrutine()
    {
        float mult = GetAnimSpeedMultiplier();

        yield return new WaitForSeconds(attackDelay / mult);

        if (player != null && arrowPrefab != null && firePoint != null && !isDead)
        {
            Vector2 exactDirection = (player.position - firePoint.position).normalized;

            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

            EnemyProjectile projScript = arrow.GetComponent<EnemyProjectile>();
            if (projScript != null)
            {
                projScript.Setup(exactDirection);
            }
        }

        yield return new WaitForSeconds(attackDelay / mult);
        ResetAnimSpeed();
    }
}
