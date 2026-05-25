using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private const float FlipCooldownDuration = 0.15f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool startMovingRight = true;
    [Tooltip("Tag of the player. The enemy turns around when it bumps into it.")]
    [SerializeField] private string playerTag = "Player";

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
            flipCooldown = FlipCooldownDuration;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(playerTag))
        {
            return;
        }

        // Only react to a side bump. Ignore contact from above so landing on the
        // enemy's head (the future damage hook) doesn't make it turn around.
        Vector2 toPlayer = collision.collider.transform.position - transform.position;
        if (Mathf.Abs(toPlayer.x) < Mathf.Abs(toPlayer.y))
        {
            return;
        }

        // Turn to walk away from the player (only flip if currently heading at it).
        bool playerOnRight = toPlayer.x > 0f;
        if (movingRight == playerOnRight)
        {
            Flip();
            flipCooldown = FlipCooldownDuration;
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
