using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public bool isMovementBloked = false;
    public bool isDashing = false;
    [HideInInspector] public bool isDead = false;

    public Rigidbody2D rb;
    public PlayerVisuals visuals;

    private GameControls controls;
    Vector2 moveInput;
    [HideInInspector] public Vector2 facingDir = Vector2.down;

    private void Awake() => controls = new GameControls();

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        if (isDead) return;
        moveInput = controls.Gameplay.Move.ReadValue<Vector2>();

        if (!isMovementBloked)
        {
            if (moveInput.sqrMagnitude > 0.01f)
            {
                if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                {
                    facingDir = new Vector2(Mathf.Sign(moveInput.x), 0);
                }
                else
                {
                    facingDir = new Vector2(0, Mathf.Sign(moveInput.y));
                }
            }

            visuals.UpdateVisuals(moveInput);
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isDashing) return;

        if (isMovementBloked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = moveInput * speed;
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
    }
}
