using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    private bool movingRight = true;

    [Header("Ground and Wall Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float checkRadius = 0.1f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.linearVelocity = new Vector2 ((movingRight ? 1: -1) * moveSpeed, rb.linearVelocity.y);

        bool wallAhead = Physics2D.OverlapCircle(wallCheck.position, checkRadius,groundLayer);
        bool groundAhead = Physics2D.OverlapCircle (groundCheck.position, checkRadius,groundLayer);

        if (wallAhead ||  !groundAhead )
        {
            Flip();
        }
    }


    void Flip()
    {
        movingRight = !movingRight;
        Vector3 localscale = transform.localScale;
        localscale.x *= -1;
        transform.localScale = localscale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (wallCheck != null)
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);

        Gizmos.color = Color.green;
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
