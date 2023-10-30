using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float horizontalSmoothSpeed;
    [SerializeField] private float verticalSmoothSpeed;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float cameraDistance;

    public float cameraControlsOffset = 0;

    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + cameraOffset + new Vector3(cameraDistance * target.localScale.x, cameraControlsOffset, 0);
        Vector3 smoothedPosition = 
            new Vector3(Mathf.Lerp(transform.position.x, desiredPosition.x, horizontalSmoothSpeed),
            Mathf.Lerp(transform.position.y, desiredPosition.y, verticalSmoothSpeed),
            -1);
        transform.position = smoothedPosition;
    }
}