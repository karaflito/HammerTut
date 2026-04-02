using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions actions;

    public float MoveInput { get; private set; }
    public float ClimbInput { get; private set; }

    private bool jumpPressed;
    private bool dashPressed;

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

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            dashPressed = true;
        }
    }

    public bool ConsumeJumpPressed()
    {
        if (!jumpPressed)
        {
            return false;
        }

        jumpPressed = false;
        return true;
    }

    public bool ConsumeDashPressed()
    {
        if (!dashPressed)
        {
            return false;
        }

        dashPressed = false;
        return true;
    }
}
