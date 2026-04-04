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
    private float baseScaleX;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>(); // expects collider on the root

        if (visualsTransform == null)
        {
            visualsTransform = transform;
        }

        baseScaleX = Mathf.Abs(visualsTransform.localScale.x);
    }

    public void StartSmoothFlip(bool faceRight)
    {
        if (visualsTransform == null)
        {
            return;
        }

        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);

        flipCoroutine = StartCoroutine(SmoothFlip(faceRight));

        if (boxCollider != null)
            boxCollider.offset = faceRight ? colliderOffsetRight : colliderOffsetLeft;
    }

    private IEnumerator SmoothFlip(bool faceRight)
    {
        Vector3 scale = visualsTransform.localScale;
        float start = scale.x;
        float end = faceRight ? baseScaleX : -baseScaleX;
        float t = 0f;

        while (t < flipDuration)
        {
            scale.x = Mathf.Lerp(start, end, t / flipDuration);
            visualsTransform.localScale = scale;
            t += Time.deltaTime;
            yield return null;
        }

        scale.x = end;
        visualsTransform.localScale = scale;
        flipCoroutine = null;
    }
}

