using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float cameraDistance;

    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + cameraOffset + new Vector3(cameraDistance * target.localScale.x, 0, 0);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}