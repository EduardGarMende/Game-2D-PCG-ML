using System.Collections;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    public float attackRadius = 0.6f;
    public float attackOffset = 0.8f;
    public float hitDelay = 0.2f;
    public int damage = 10;
    public LayerMask playerLayer;

    protected override void AttackPlayer()
    {
        TriggerAttackAnimation();
        StartCoroutine(HitDelayCorrutine());
    }

    private IEnumerator HitDelayCorrutine()
    {
        yield return new WaitForSeconds(hitDelay);

        Vector2 attackPosition = (Vector2)transform.position + (facingDir * attackOffset);
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPosition, attackRadius, playerLayer);
        foreach (Collider2D hit in hitPlayers)
        {
            Debug.Log("¡El enemigo ha dado un espadazo al Jugador!");
            // hit.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(attackRange.x * 2, attackRange.y * 2, 0));

        Vector2 drawDir = Application.isPlaying ? facingDir : Vector2.down;
        Vector2 attackPoint = (Vector2)transform.position + (drawDir * attackOffset);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint, attackRadius);
    }
}
