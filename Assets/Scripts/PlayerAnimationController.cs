using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    private PlayerInputHandler input;
    private PlayerMovement movement;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        movement = GetComponent<PlayerMovement>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (animator == null)
        {
            return;
        }

        bool isRunning = movement.CurrentStateId == PlayerStateId.Grounded && Mathf.Abs(input.MoveInput) > 0.1f;
        bool isJumping = movement.CurrentStateId == PlayerStateId.Airborne && movement.Velocity.y > 0.1f;
        bool isFalling = movement.CurrentStateId == PlayerStateId.Airborne && movement.Velocity.y < -0.1f;
        bool isClimbing = movement.CurrentStateId == PlayerStateId.Climbing;

        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsClimbing", isClimbing);
        animator.SetFloat("ClimbSpeed", isClimbing ? Mathf.Abs(input.ClimbInput) : 0f);
    }
}
