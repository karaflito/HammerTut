using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float maxVelocity = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float coyoteTime = 0.1f;
    public float groundCheckRadius = 0.2f;

    [Header("Gravity Settings")]
    public float normalGravity = 3f;
    public float fallGravityMultiplier = 2f;

    [Header("Climb Settings")]
    public float climbSpeed = 1f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCoolDown = 1f;











}
