using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private const float InputBufferDuration = 0.1f;

    private PlayerInputActions actions;

    public float MoveInput { get; private set; }
    public float ClimbInput { get; private set; }

    private float jumpBufferRemaining;
    private float dashBufferRemaining;

    private void Awake()
    {
        actions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        actions.Player.Enable();

        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMove;

        actions.Player.Climbing.performed += OnClimb;
        actions.Player.Climbing.canceled += OnClimb;

        actions.Player.Jump.performed += OnJump;
        actions.Player.Dash.performed += OnDash;
    }

    private void OnDisable()
    {
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMove;

        actions.Player.Climbing.performed -= OnClimb;
        actions.Player.Climbing.canceled -= OnClimb;

        actions.Player.Jump.performed -= OnJump;
        actions.Player.Dash.performed -= OnDash;

        actions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<float>();
    }

    private void OnClimb(InputAction.CallbackContext context)
    {
        ClimbInput = context.ReadValue<float>();
    }

    private void Update()
    {
        if (jumpBufferRemaining > 0f)
            jumpBufferRemaining -= Time.deltaTime;

        if (dashBufferRemaining > 0f)
            dashBufferRemaining -= Time.deltaTime;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        jumpBufferRemaining = InputBufferDuration;
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        dashBufferRemaining = InputBufferDuration;
    }

    public bool ConsumeJumpPressed()
    {
        if (jumpBufferRemaining <= 0f)
        {
            return false;
        }

        jumpBufferRemaining = 0f;
        return true;
    }

    public bool ConsumeDashPressed()
    {
        if (dashBufferRemaining <= 0f)
        {
            return false;
        }

        dashBufferRemaining = 0f;
        return true;
    }
}
