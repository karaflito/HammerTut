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


    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    // =========================
    // Input
    // =========================
    private PlayerInputActions actions;
    private float moveInput;

    // =========================
    // Components
    // =========================
    private Rigidbody2D rb;
    private Animator animator;

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
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        PlayerAnimation();
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void FixedUpdate()
    {
        PlayerMovementVelocity();
        HandleFlip();
    }

    private void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMove;

        actions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        actions.Player.Disable();
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMove;

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
        PlayerJumping();
        Debug.Log("Jump pressed");
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
        if(isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
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
    }

    private IEnumerator SmoothFlip(bool faceRight)
    {
        facingRight = faceRight;

        float start = transform.localScale.x;
        float end = faceRight ? 1f : -1f;
        float elapsed = 0f;

        while (elapsed < flipDuration)
        {
            float newX = Mathf.Lerp(start, end, elapsed / flipDuration);
            transform.localScale = new Vector3(newX, 1f, 1f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(end, 1f, 1f);
        flipCoroutine = null;
    }
}
