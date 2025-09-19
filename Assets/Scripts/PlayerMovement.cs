using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // === Configurable (show in Inspector) ===
    [SerializeField] private float speed = 5f;
    [SerializeField] private float flipDuration = 0.2f;
    [SerializeField] private float inputDeadzone = 0.1f;

    // === Input ===
    private PlayerInputActions actions;
    private float moveInput;

    // === Components ===
    private Rigidbody2D rb;
    private Animator animator;

    // === Flip system ===
    private bool facingRight = true;
    private Coroutine flipCoroutine;
    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        actions = new PlayerInputActions();
    }
    private void FixedUpdate()
    {
        PlayerVelocity();
        HandleFlip();
    }
    private void Update()
    {
        PlayerAnimation();
    }

    private void OnEnable()
    {
        actions.Player.Enable();

        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMove;

        actions.Player.Disable();
    }
      


    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    private void HandleFlip()
    {
        if (moveInput > 0 && !facingRight)
            StartSmoothFlip(true);
        else if (moveInput < 0 && facingRight)
            StartSmoothFlip(false);
    }

    private void StartSmoothFlip(bool faceRight)
    {
        // Stop any ongoing flip so we don’t stack coroutines
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);

        flipCoroutine = StartCoroutine(SmoothFlip(faceRight));
    }

    private IEnumerator SmoothFlip(bool faceRight)
    {
        facingRight = faceRight;

        float start = transform.localScale.x;
        float end = faceRight ? 1f : -1f;
        float time = 0f;
        float duration = 0.2f; // tweak for flip speed

        while (time < duration)
        {
            float t = time / duration;
            float scaleX = Mathf.Lerp(start, end, t);

            // lock Y = 1, Z = 1
            transform.localScale = new Vector3(scaleX, 1f, 1f);

            time += Time.deltaTime;
            yield return null;
        }

        // Snap to final
        transform.localScale = new Vector3(end, 1f, 1f);
        flipCoroutine = null;
    }

    private void PlayerVelocity()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
        
            
    }
    private void PlayerAnimation()
    {
        
        animator.SetBool("IsRunning", Mathf.Abs(moveInput) > inputDeadzone);
    }    
}
