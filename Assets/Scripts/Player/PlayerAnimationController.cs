using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimationController : MonoBehaviour
{
    private const float VelocityThreshold = 0.1f;

    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    private static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
    private static readonly int IsClimbingHash = Animator.StringToHash("IsClimbing");
    private static readonly int ClimbSpeedHash = Animator.StringToHash("ClimbSpeed");

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

        bool isRunning = movement.CurrentStateId == PlayerStateId.Grounded && Mathf.Abs(input.MoveInput) > VelocityThreshold;
        bool isJumping = movement.CurrentStateId == PlayerStateId.Airborne && movement.Velocity.y > VelocityThreshold;
        bool isFalling = movement.CurrentStateId == PlayerStateId.Airborne && movement.Velocity.y < -VelocityThreshold;
        bool isClimbing = movement.CurrentStateId == PlayerStateId.Climbing;

        animator.SetBool(IsRunningHash, isRunning);
        animator.SetBool(IsJumpingHash, isJumping);
        animator.SetBool(IsFallingHash, isFalling);
        animator.SetBool(IsClimbingHash, isClimbing);
        animator.SetFloat(ClimbSpeedHash, isClimbing ? Mathf.Abs(input.ClimbInput) : 0f);
    }
}
