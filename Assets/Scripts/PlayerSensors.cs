using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerSensors : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Climb Check")]
    [SerializeField] private string climbableTag = "Climbing";

    private int climbContactCount;
    private bool hasWarnedAboutMissingGroundCheck;

    public bool IsGrounded { get; private set; }
    public bool IsNearClimbable => climbContactCount > 0;

    private void Awake()
    {
        TryAutoAssignGroundCheck();
    }

    private void Reset()
    {
        TryAutoAssignGroundCheck();
    }

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (groundCheck == null)
        {
            IsGrounded = false;
            if (!hasWarnedAboutMissingGroundCheck)
            {
                Debug.LogWarning("PlayerSensors has no Ground Check assigned. Grounded checks will always be false.", this);
                hasWarnedAboutMissingGroundCheck = true;
            }
            return;
        }

        hasWarnedAboutMissingGroundCheck = false;
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void TryAutoAssignGroundCheck()
    {
        if (groundCheck != null)
        {
            return;
        }

        Transform childGroundCheck = transform.Find("GroundCheck");
        if (childGroundCheck != null)
        {
            groundCheck = childGroundCheck;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(climbableTag))
        {
            return;
        }

        climbContactCount++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(climbableTag))
        {
            return;
        }

        climbContactCount = Mathf.Max(0, climbContactCount - 1);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
