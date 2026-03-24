using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    protected float currentHealth;
    public float speed = 2.5f;

    public Vector2 attackRange = new Vector2(1.2f, 0.8f);
    public float attackCooldown = 1f;
    protected float nextAttackTime = 0f;

    public LayerMask obstacleLayer;
    public int steeringRays = 12;
    public float steeringRayLength = 1.5f;

    public float stuckCheckInterval = 0.5f;
    public float stuckDistanceThreshold = 0.1f;
    private float unstuckTimer = 0f;

    private Vector2 lastCheckedPosition;
    private float nextStuckCheckTime;
    private int stuckCount = 0;

    public Animator[] animators;

    protected Transform player;
    protected Rigidbody2D rb;
    protected Vector2 facingDir;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    protected virtual void Update()
    {
        if (player == null) return;

        CheckIfStuck();

        float distanceX = Mathf.Abs(player.position.x - transform.position.x);
        float distanceY = Mathf.Abs(player.position.y - transform.position.y);
        bool isInAttackRange = (distanceX <= attackRange.x) && (distanceY <= attackRange.y);

        if (!isInAttackRange && Time.time >= nextAttackTime)
        {
            if (Time.time >= unstuckTimer)
            {
                MoveTowardsPlayer();
            }
        }
        else if (isInAttackRange && Time.time >= nextAttackTime)
        {
            StopMovement();
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
        else
        {
            StopMovement();
        }

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            if (Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(rb.linearVelocity.y))
            {
                facingDir = new Vector2(Mathf.Sign(rb.linearVelocity.x), 0);
            }
            else
            {
                facingDir = new Vector2(0, Mathf.Sign(rb.linearVelocity.y));
            }
        }
        else
        {
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            if (Mathf.Abs(dirToPlayer.x) > Mathf.Abs(dirToPlayer.y))
                facingDir = new Vector2(Mathf.Sign(dirToPlayer.x), 0);
            else
                facingDir = new Vector2(0, Mathf.Sign(dirToPlayer.y));
        }


        foreach (Animator anim in animators)
        {
            anim.SetFloat("InputX", facingDir.x);
            anim.SetFloat("InputY", facingDir.y);
        }
    }

    private void CheckIfStuck()
    {
        if (Time.time < nextStuckCheckTime) return;

        float moved = Vector2.Distance(rb.position, lastCheckedPosition);

        if (moved < stuckDistanceThreshold)
        {
            stuckCount++;
            if (stuckCount >= 2) // Lleva 1 segundo sin moverse
            {
                ApplyUnstuckForce();
                stuckCount = 0;
            }
        }
        else
        {
            stuckCount = 0;
        }

        lastCheckedPosition = rb.position;
        nextStuckCheckTime = Time.time + stuckCheckInterval;
    }

    private void ApplyUnstuckForce()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;
        Vector2 perpendicular = new Vector2(-toPlayer.y, toPlayer.x);

        if (Random.value > 0.5f) perpendicular = -perpendicular;

        rb.linearVelocity = perpendicular * speed * 1.5f;

        // Le damos 0.3 segundos para que se mueva libremente antes de volver a perseguir
        unstuckTimer = Time.time + 0.3f;
    }

    protected virtual void MoveTowardsPlayer()
    {
        Vector2 desiredDir = (player.position - transform.position).normalized;
        Vector2 finalDir = GetSteeringDirection(desiredDir);
        rb.linearVelocity = finalDir * speed;

        foreach (Animator anim in animators) anim.SetBool("isMoving", true);
    }

    private Vector2 GetSteeringDirection(Vector2 desiredDir)
    {
        Vector2 bestDir = desiredDir;
        float bestScore = -1f;

        for (int i = 0; i < steeringRays; i++)
        {
            float angle = i * (360f / steeringRays);
            Vector2 candidate = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            float score = Mathf.Max(0f, Vector2.Dot(candidate, desiredDir));

            RaycastHit2D hit = Physics2D.Raycast(transform.position, candidate, steeringRayLength, obstacleLayer);

            if (hit.collider != null)
            {
                float proximity = 1f - (hit.distance / steeringRayLength);
                score -= proximity * 2f;
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestDir = candidate;
            }
        }

        return bestDir;
    }

    protected virtual void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;

        foreach (Animator anim in animators)
        {
            anim.SetBool("isMoving", false);
        }
    }

    protected void TriggerAttackAnimation()
    {
        foreach (Animator anim in animators)
        {
            anim.SetTrigger("Attack");
        }
    }

    protected abstract void AttackPlayer();

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageFlashRoutine());
        }
    }

    protected IEnumerator DamageFlashRoutine()
    {
        foreach (Animator anim in animators)
        {
            anim.GetComponent<SpriteRenderer>().color = Color.red;
        }

        yield return new WaitForSeconds(0.15f);

        foreach (Animator anim in animators)
        {
            anim.GetComponent<SpriteRenderer>().color = Color.white;  // White significa sin filtro de color, mostrando el sprite original
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero;

        this.enabled = false;

        foreach (Animator anim in animators)
        {
                anim.SetTrigger("Die");
        }

        Room currentRoom = FindFirstObjectByType<Room>();
        if (currentRoom != null)
        {
            currentRoom.EnemyKilled();
        }

        Destroy(gameObject, 2f);
    }
}
