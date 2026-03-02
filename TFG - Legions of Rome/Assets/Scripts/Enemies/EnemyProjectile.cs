using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifetime = 1.75f;
    public int damage = 10;
    public LayerMask playerLayer;

    public void Setup(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("¡La flecha ha impactado al jugador!");
            Destroy(gameObject);
        }
    }
}
