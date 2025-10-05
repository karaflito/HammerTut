using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private PlayerData playerData;

    private Rigidbody2D rb;
    private PlayerInputHandler input;
    private Flippable flippable;
    private PlayerDash dash;
    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        flippable = GetComponent<Flippable>();
        dash = GetComponent<PlayerDash>();
    }

    private void FixedUpdate()
    {
        // Horizontal movement only
        rb.linearVelocity = new Vector2(input.MoveInput * playerData.speed, rb.linearVelocity.y);
        if (dash != null && dash.IsDashing)
        {
            // Still handle flip during dash
            HandleFlip();
            return;
        }

        HandleFlip();
    }

    private void HandleFlip()
    {
        if (input.MoveInput > 0 && !facingRight)
        {
            facingRight = true;
            flippable.StartSmoothFlip(true);
        }
        else if (input.MoveInput < 0 && facingRight)
        {
            facingRight = false;
            flippable.StartSmoothFlip(false);
        }
    }
}
