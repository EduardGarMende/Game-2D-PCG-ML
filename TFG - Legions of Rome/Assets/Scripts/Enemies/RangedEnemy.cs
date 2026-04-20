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
        yield return new WaitForSeconds(attackDelay);

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
    }
}
