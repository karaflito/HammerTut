using UnityEngine;


[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    private PlayerInputHandler input;
    private Rigidbody2D rb;
    private PlayerClimb climb;
    private PlayerJump jump;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody2D>();
        climb = GetComponent<PlayerClimb>();
        jump = GetComponent<PlayerJump>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // =========================
        // Running
        // =========================
        
        animator.SetBool("IsRunning", jump.isGrounded && Mathf.Abs(input.MoveInput) > 0.1f);
          
        
        

        // =========================
        // Jumping / Falling
        // =========================
        animator.SetBool("IsJumping", rb.linearVelocity.y > 0.1f);
        animator.SetBool("IsFalling", rb.linearVelocity.y < -0.1f);

        // =========================
        // Climbing
        // =========================
        if (climb.IsClimbing)
        {
            animator.SetBool("IsClimbing", true);
            animator.SetFloat("ClimbSpeed", Mathf.Abs(input.ClimbInput));
        }
        else
        {
            animator.SetBool("IsClimbing", false);
            animator.SetFloat("ClimbSpeed", 0f);
        }
    }
}
