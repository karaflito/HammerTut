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

    public bool IsDashing => isDashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        jump = GetComponent<PlayerJump>();
    }

    private void Update()
    {
        //Handle dash timing
        HandleDashTiming();
        HandleDashCooldown();
        if(input.ConsumeDashPressed() && canDash)
        {
            TryStartDash();
        }
    }

    private void FixedUpdate()
    {
        if(isDashing)
        {
            rb.linearVelocity = dashDirection * playerData.dashSpeed;
        }    
    }

    private void HandleDashTiming ()
    {
        if (isDashing)
        {
            dashTimeRemaining -= Time.deltaTime;
            if (dashTimeRemaining <= 0)
            {
                EndDash();
            }
        }
    }

    private void HandleDashCooldown()
    {
        if(!canDash)
        {
            dashCooldownRemaining -= Time.deltaTime;
            if(dashCooldownRemaining <= 0)
            {
                canDash = true;
            }
        }
    }

    private void TryStartDash()
    {
        Vector2 dir = Vector2.zero;

        if (Mathf.Abs(input.MoveInput) > 0.1f)
        {
            dir.x = Mathf.Sign(input.MoveInput);
        }

        if(dir.x == 0)
        {
            dir.x = transform.localScale.x > 0 ? 1 : -1;
        }

        StartDash(dir.normalized);
        Debug.Log($"Dash direction: {dir}, dashSpeed: {playerData.dashSpeed}");
    }
    
    private void StartDash (Vector2 dir)
    {
        isDashing = true;
        canDash = false;
        dashTimeRemaining = playerData.dashDuration;
        dashCooldownRemaining = playerData.dashCoolDown;
        dashDirection = dir;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
    }    

    private void EndDash()
    {
        isDashing = false;
        rb.gravityScale = playerData.normalGravity;

        rb.linearVelocity = dashDirection * playerData.speed;
    }
}
