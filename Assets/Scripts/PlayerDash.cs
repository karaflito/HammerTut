using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerDash : MonoBehaviour
{
    private PlayerMovement movement;

    public bool IsDashing => movement != null && movement.IsDashing;
    public bool CanDash => movement != null && movement.CanDash;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }
}
