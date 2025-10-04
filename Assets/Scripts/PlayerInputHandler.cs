using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions actions;

    // Public properties that other scripts can read
    public float MoveInput { get; private set; }
    public float ClimbInput { get; private set; }
    public bool JumpPressed { get; private set; }

    private void Awake()
    {
        actions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        actions.Player.Enable();

        // Subscribe to input callbacks
        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMove;

        actions.Player.Jump.performed += OnJump;
        actions.Player.Jump.canceled += OnJump;

        actions.Player.Climbing.performed += OnClimb;
        actions.Player.Climbing.canceled += OnClimb;
    }

    private void OnDisable()
    {
        actions.Player.Disable();

        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMove;

        actions.Player.Jump.performed -= OnJump;
        actions.Player.Jump.canceled -= OnJump;

        actions.Player.Climbing.performed -= OnClimb;
        actions.Player.Climbing.canceled -= OnClimb;
    }

    // ----------------------
    // Input Callbacks
    // ----------------------
    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<float>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        JumpPressed = ctx.performed;
        
    }

    public bool ConsumeJumpPressed()
    {
        if (JumpPressed)
        {
            JumpPressed = false;
            return true;
        }
        return false;
    }

    private void OnClimb(InputAction.CallbackContext ctx)
    {
        ClimbInput = ctx.ReadValue<float>();
    }
}
