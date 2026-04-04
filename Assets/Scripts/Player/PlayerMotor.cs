using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMotor : MonoBehaviour
{
    public const float InputDeadZone = 0.01f;

    private Rigidbody2D rb;
    private Flippable flippable;
    private bool facingRight = true;

    public Vector2 Velocity => rb.linearVelocity;
    public int FacingSign => facingRight ? 1 : -1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        flippable = GetComponent<Flippable>();
        facingRight = transform.localScale.x >= 0f;
    }

    public void SetHorizontalSpeed(float speed)
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
    }

    public void SetVerticalSpeed(float speed)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, speed);
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public void ResetVerticalVelocity()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }

    public void AddImpulse(Vector2 impulse)
    {
        rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    public void SetGravityScale(float gravityScale)
    {
        rb.gravityScale = gravityScale;
    }

    public void FaceDirection(float direction)
    {
        if (Mathf.Abs(direction) <= InputDeadZone)
        {
            return;
        }

        bool shouldFaceRight = direction > 0f;
        if (shouldFaceRight == facingRight)
        {
            return;
        }

        facingRight = shouldFaceRight;

        if (flippable != null)
        {
            flippable.StartSmoothFlip(facingRight);
            return;
        }

        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * FacingSign;
        transform.localScale = localScale;
    }
}
