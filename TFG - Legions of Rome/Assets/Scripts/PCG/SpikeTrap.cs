using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float damagePerTick = 10f;
    public float tickRate = 0.5f;
    
    private float timer = 0f;
    private List<Collider2D> entitiesInTrap = new List<Collider2D>();

    private void Update()
    {
        if (entitiesInTrap.Count > 0)
        {
            timer += Time.deltaTime;
            if (timer >= tickRate)
            {
                dealDamage();
                timer = 0f;
            }
        }
        else
        {
            timer = 0f; // Reset timer when no entities are in the trap
        }
    }

    private void dealDamage()
    {
        for (int i = entitiesInTrap.Count - 1; i >= 0; i--)
        {
            Collider2D entity = entitiesInTrap[i];

            if (entity == null)
            {
                entitiesInTrap.RemoveAt(i);
                continue;
            }

            if (entity.CompareTag("Player"))
            {
                PlayerHealth pHealth = entity.GetComponent<PlayerHealth>();
                if (pHealth != null)
                {
                    pHealth.TakeDamage(damagePerTick);
                }
            }
            else if (entity.CompareTag("Enemy"))
            {
                Enemy eHealth = entity.GetComponent<Enemy>();
                if (eHealth != null)
                {
                    eHealth.TakeDamage(damagePerTick);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            if (!entitiesInTrap.Contains(collision))
            {
                entitiesInTrap.Add(collision);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (entitiesInTrap.Contains(collision))
        {
            entitiesInTrap.Remove(collision);
        }
    }
}
