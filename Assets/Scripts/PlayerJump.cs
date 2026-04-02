using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerJump : MonoBehaviour
{
    private PlayerMovement movement;

    public bool IsGrounded => movement != null && movement.IsGrounded;
    public bool IsJumping => movement != null && movement.CurrentStateId == PlayerStateId.Airborne && movement.Velocity.y > 0.1f;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }
}
