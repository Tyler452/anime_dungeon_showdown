using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 offset = new Vector3(0f, 10f, -5f);
    public float smoothTime = 0.2f; // lower = tighter follow, higher = smoother

    private Transform target;
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // desired position based on target + offset
        Vector3 desiredPosition = target.position + offset;

        // SmoothDamp is smoother and less jittery than Lerp for following
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }

    // Set by CharacterSwap
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        // snap immediately on first set to avoid popping or delay
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}