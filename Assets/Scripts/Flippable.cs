using UnityEngine;
using System.Collections;

public class Flippable : MonoBehaviour
{
    [Header("Flip Settings")]
    [SerializeField] private Transform visualsTransform;   // child with SpriteRenderer + Animator
    [SerializeField] private float flipDuration = 0.2f;

    [Header("Collider Offsets (optional)")]
    [SerializeField] private Vector2 colliderOffsetRight;
    [SerializeField] private Vector2 colliderOffsetLeft;

    private BoxCollider2D boxCollider;
    private Coroutine flipCoroutine;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>(); // expects collider on the root
    }

    public void StartSmoothFlip(bool faceRight)
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);

        flipCoroutine = StartCoroutine(SmoothFlip(faceRight));

        if (boxCollider != null)
            boxCollider.offset = faceRight ? colliderOffsetRight : colliderOffsetLeft;
    }

    private IEnumerator SmoothFlip(bool faceRight)
    {
        // Smoothly lerp the child’s X scale from current to target (+1 or -1)
        float start = visualsTransform.localScale.x;
        float end = faceRight ? 1f : -1f;
        float t = 0f;

        while (t < flipDuration)
        {
            float x = Mathf.Lerp(start, end, t / flipDuration);
            visualsTransform.localScale = new Vector3(x, 1f, 1f);
            t += Time.deltaTime;
            yield return null;
        }

        visualsTransform.localScale = new Vector3(end, 1f, 1f);
        flipCoroutine = null;
    }
}
