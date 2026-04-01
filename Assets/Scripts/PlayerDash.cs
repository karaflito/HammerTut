using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private PlayerData playerData;
    private Rigidbody2D rb;
    private PlayerInputHandler input;
    private PlayerJump jump;
    private bool isDashing;
    private bool canDash = true;
    private float dashTimeRemaining;
    private float dashCooldownRemaining;
    private Vector2 dashDirection;
    private float savedGravity; // NEW: Save gravity to restore it

    public bool IsDashing => isDashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        jump = GetComponent<PlayerJump>();
              
        
    }
//testing part2
    private void Update()
    {
        //Handle dash timing
        if (isDashing)
        {
            dashTimeRemaining -= Time.deltaTime;
            if (dashTimeRemaining <= 0)
            {
                EndDash();
            }
        }

        //Handle dash cooldown
        if (!canDash)
        {
            dashCooldownRemaining -= Time.deltaTime;
            if (dashCooldownRemaining <= 0)
            {
                canDash = true;
            }
        }

        if (input.ConsumeDashPressed() && canDash)
        {
           
            TryStartDash();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            // Force the velocity every frame during dash
            rb.linearVelocity = dashDirection * playerData.dashSpeed;
            rb.gravityScale = 0f; // Keep gravity at 0 during dash
            
        }
    }

    private void TryStartDash()
    {
        

        Vector2 dir = Vector2.zero;
        if (Mathf.Abs(input.MoveInput) > 0.1f)
        {
            dir.x = Mathf.Sign(input.MoveInput);
        }
        if (dir.x == 0)
        {
            dir.x = transform.localScale.x > 0 ? 1 : -1;
        }

        
        StartDash(dir.normalized);
    }

    private void StartDash(Vector2 dir)
    {
        
        isDashing = true;
        canDash = false;
        dashTimeRemaining = playerData.dashDuration;
        dashCooldownRemaining = playerData.dashCoolDown;
        dashDirection = dir;

        // Save current gravity
        savedGravity = rb.gravityScale;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
    }

    private void EndDash()
    {
        
        isDashing = false;

        // Restore gravity
        rb.gravityScale = savedGravity;

        // Keep some horizontal momentum
        rb.linearVelocity = new Vector2(dashDirection.x * playerData.speed, rb.linearVelocity.y);
    }
}