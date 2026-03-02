using UnityEngine;

public class EnemyDummy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.white;
    private Color originalColor;

    private void Start()
    {
        originalColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Golpe Recibido");
        spriteRenderer.color = hitColor;

        Invoke("ResetColor", 0.1f);
    }

    void ResetColor()
    {
        spriteRenderer.color = originalColor;
    }
}
