using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerClimb : MonoBehaviour
{
    private PlayerMovement movement;

    public bool IsClimbing => movement != null && movement.IsClimbing;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }
}
