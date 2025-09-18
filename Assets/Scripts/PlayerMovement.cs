using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private PlayerInputActions actions;

    private float moveInput;

    private Rigidbody2D rb;

    [SerializeField] private float speed = 5f; 



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        actions = new PlayerInputActions();
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

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }



}
