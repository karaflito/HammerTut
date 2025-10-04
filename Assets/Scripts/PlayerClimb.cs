using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerClimb : MonoBehaviour
{
    [Header("Climb Settings")]
    [SerializeField] private PlayerData playerData;

    private Rigidbody2D rb;
    private PlayerInputHandler input;
    private Animator animator;

    private bool isClimbing;
    private bool isNearClimbable;
    private float savedGravity;

    private PlayerJump jump;

    public bool IsClimbing => isClimbing;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        animator = GetComponentInChildren<Animator>();
        jump = GetComponent<PlayerJump>();
    }

    private void FixedUpdate()
    {
        if (isNearClimbable && !isClimbing && Mathf.Abs(input.ClimbInput) > 0.01f)
        {
            StartClimb();
        }


        if(IsClimbing && input.ConsumeJumpPressed())
        {
            StopClimb();
            jump.ForceJumpFromClimb();
            return;
        }

        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, input.ClimbInput * playerData.climbSpeed);

           

            if (!isNearClimbable)
            {
                StopClimb();
            }
        }
        
    }

    private void StartClimb()
    {
        isClimbing = true;
        savedGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        
    }

    private void StopClimb()
    {
        if (!isClimbing) return;
        isClimbing = false;
        rb.gravityScale = savedGravity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Climbing"))
        {
            isNearClimbable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Climbing"))
        {
            isNearClimbable = false;
            StopClimb();
        }
    }
}
