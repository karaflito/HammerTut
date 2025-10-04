using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private PlayerData playerData;

    private bool jumpQueued;
    private float coyoteTimer;

    private Rigidbody2D rb;
    private PlayerInputHandler input;
    private PlayerClimb climb;

    public bool isGrounded { get; private set; }

    private void Awake()
    {
        climb = GetComponent<PlayerClimb>();
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        // === Ground check ===
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, groundLayer);


        if (climb != null && climb.IsClimbing)
        {
            isGrounded = false;        // optional: keep false to avoid weird animations
            coyoteTimer = playerData.coyoteTime; // treat as if freshly grounded
            return;                    // skip the rest of Update
        }

        if (isGrounded)
        {
            // Reset the coyote timer to max whenever grounded
            coyoteTimer = playerData.coyoteTime;
        }
        else
        {
            // Countdown while in air
            coyoteTimer -= Time.deltaTime;
        }

        // === Input ===
        if (input.ConsumeJumpPressed())
        {
            jumpQueued = true;
        }
    }

    private void FixedUpdate()
    {
        if (jumpQueued)
        {
            jumpQueued = false;
            TryJump();
        }

        ApplyBetterGravity();
    }

    private void TryJump()
    {
        // Only jump if within coyote time
        if (coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // reset Y velocity for consistent jumps
            rb.AddForce(Vector2.up * playerData.jumpForce, ForceMode2D.Impulse);

            // Lock out further jumps until grounded again
            coyoteTimer = 0f;
        }
    }


    private void ApplyBetterGravity()
    {
        if (climb != null && climb.IsClimbing)
        {
            
            return;
        }
            

        if(rb.linearVelocity.y < 0f)
        {
            rb.gravityScale = playerData.normalGravity * playerData.fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = playerData.normalGravity;
        }
    }

    public void ForceJumpFromClimb()
    {
        rb.gravityScale = playerData.normalGravity;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * playerData.jumpForce, ForceMode2D.Impulse);
        coyoteTimer = 0f; // lock out another jump until grounded
    }

   

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, playerData.groundCheckRadius);
        }
    }
}
