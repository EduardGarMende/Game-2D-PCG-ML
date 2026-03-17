using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    protected float currentHealth;
    public float speed = 2f;

    public Vector2 attackRange = new Vector2(1.2f, 0.8f);
    public float attackCooldown = 1f;
    protected float nextAttackTime = 0f;

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

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        if (Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.y))
            facingDir = new Vector2(Mathf.Sign(directionToPlayer.x), 0);
        else
            facingDir = new Vector2(0, Mathf.Sign(directionToPlayer.y));

        foreach (Animator anim in animators)
        {
            anim.SetFloat("InputX", facingDir.x);
            anim.SetFloat("InputY", facingDir.y);
        }

        float distanceX = Mathf.Abs(player.position.x - transform.position.x);
        float distanceY = Mathf.Abs(player.position.y - transform.position.y);

        bool isInAttackRange = (distanceX <= attackRange.x) && (distanceY <= attackRange.y);

        if (!isInAttackRange && Time.time >= nextAttackTime)
        {
            MoveTowardsPlayer(directionToPlayer);
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
    }

    protected virtual void MoveTowardsPlayer(Vector2 direction)
    {
        rb.linearVelocity = direction * speed;

        foreach (Animator anim in animators)
        {
            anim.SetBool("isMoving", true);
        }
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
