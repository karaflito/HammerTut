using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // =========================
    // Inspector / Config Values
    // =========================
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float flipDuration = 0.2f;
    [SerializeField] private float inputDeadzone = 0.1f;
    [SerializeField] private float climbSpeed = 1.0f;

    // Collider offsets (for left/right facing)
    [SerializeField] private Vector2 colliderOffsetRight;
    [SerializeField] private Vector2 colliderOffsetLeft;

    // Scene references
    [SerializeField] private Transform visualsTransform;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // =========================
    // State Flags
    // =========================
    private bool isGrounded;
    private bool isClimbing;
    private bool isNearClimbable;
    private float savedGravity = 1f;

    // =========================
    // Input
    // =========================
    private PlayerInputActions actions;
    private float moveInput;
    private float climbingInput;

    // =========================
    // Components
    // =========================
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    [SerializeField] private Animator animator;

    // =========================
    // Flip State
    // =========================
    private bool facingRight = true;
    private Coroutine flipCoroutine;

    // =========================
    // Unity Lifecycle
    // =========================
    private void Awake()
    {
        actions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        PlayerAnimation();
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void FixedUpdate()
    {
        PlayerMovementVelocity();
        if (isNearClimbable && !isClimbing && Mathf.Abs(climbingInput) > 0.01f)
        {
            StartClimb();
        }
            
        PlayerClimbing();
        HandleFlip();
    }

    private void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMove;
        actions.Player.Jump.performed += OnJump;
        actions.Player.Climbing.performed += OnClimbing;
        actions.Player.Climbing.canceled += OnClimbing;
        
    }

    private void OnDisable()
    {
        actions.Player.Disable();
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMove;
        actions.Player.Climbing.performed -= OnClimbing;
        actions.Player.Climbing.canceled -= OnClimbing;
        actions.Player.Jump.performed -= OnJump;
    }

   

    // =========================
    // Input Callbacks
    // =========================
    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<float>();
        
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if(isClimbing)
        {
            StopClimb();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            return;
        }
        else if (isNearClimbable && isGrounded)
        {
            PlayerJumping();
            return;
        }
        PlayerJumping();


    }    

    private void OnClimbing (InputAction.CallbackContext ctx)
    {
        climbingInput = ctx.ReadValue<float>();        
    }
    // =========================
    // Movement & Animation
    // =========================
    private void PlayerMovementVelocity()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    private void PlayerJumping()
    {
        if (isGrounded)
        {
            rb.gravityScale = 1f;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

    }

    private void PlayerClimbing()
    {
        if (!isClimbing)
        {
            animator.SetBool("IsClimbing", false);
            return;
        }
            
         
        rb.linearVelocity = new Vector2 (rb.linearVelocity.x, climbingInput * climbSpeed);
        // Update animation
        animator.SetBool("IsClimbing", true);
        animator.SetFloat("ClimbSpeed", Mathf.Abs(climbingInput));

        if (!isNearClimbable) StopClimb();
    }
    
    private void StartClimb()
    {
        isClimbing = true;
        savedGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x,0f);
    }

    private void StopClimb()
    {
        if (!isClimbing) return;
        isClimbing = false;
        rb.gravityScale = savedGravity;
    }
   

    private void PlayerAnimation()
    {
        animator.SetBool("IsRunning", Mathf.Abs(moveInput) > inputDeadzone);
    }

    // =========================
    // Flipping
    // =========================
    private void HandleFlip()
    {
        if (moveInput > 0 && !facingRight)
            StartSmoothFlip(true);
            
        else if (moveInput < 0 && facingRight)
            StartSmoothFlip(false);
    }

    private void StartSmoothFlip(bool faceRight)
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);

        flipCoroutine = StartCoroutine(SmoothFlip(faceRight));

        ApplyColliderOffset(faceRight);
    }

    private IEnumerator SmoothFlip(bool faceRight)
    {
        facingRight = faceRight;

        float start = visualsTransform.localScale.x;
        float end = faceRight ? 1f : -1f;
        float elapsed = 0f;
       

        while (elapsed < flipDuration)
        {
            float newX = Mathf.Lerp(start, end, elapsed / flipDuration);
            visualsTransform.localScale = new Vector3(newX, 1f, 1f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        visualsTransform.localScale = new Vector3(end, 1f, 1f);
        flipCoroutine = null;
    }
    // =========================
    // Utilities
    // =========================
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    // =========================
    // Colliders
    // =========================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Climbing"))
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


    private void ApplyColliderOffset(bool faceRight)
    {
        boxCollider.offset = faceRight ? colliderOffsetRight : colliderOffsetLeft;
    }




}
