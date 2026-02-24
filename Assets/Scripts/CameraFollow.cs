using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;          // The player
    public Vector3 offset = new Vector3(0, 10, 0); // Example: above

    void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + offset;
    }
}