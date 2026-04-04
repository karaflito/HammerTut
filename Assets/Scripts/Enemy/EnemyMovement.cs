using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool startMovingRight = true;

    [Header("Ground and Wall Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkRadius = 0.1f;

    private Rigidbody2D rb;
    private bool movingRight;
    private bool hasLoggedMissingChecks;
    private float flipCooldown;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movingRight = startMovingRight;
    }

    private void FixedUpdate()
    {
        if (groundCheck == null || wallCheck == null)
        {
            if (!hasLoggedMissingChecks)
            {
                Debug.LogWarning("EnemyMovement needs both Ground Check and Wall Check assigned.", this);
                hasLoggedMissingChecks = true;
            }

            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2((movingRight ? 1f : -1f) * moveSpeed, rb.linearVelocity.y);

        if (flipCooldown > 0f)
        {
            flipCooldown -= Time.fixedDeltaTime;
            return;
        }

        bool wallAhead = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
        bool groundAhead = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (wallAhead || !groundAhead)
        {
            Flip();
            flipCooldown = 0.15f;
        }
    }

    private void Flip()
    {
        movingRight = !movingRight;

        Vector3 localScale = transform.localScale;
        localScale.x = -localScale.x;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (wallCheck != null)
        {
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }

        Gizmos.color = Color.green;
        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
