using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.5f;
    [SerializeField] private float cameraDistance = 10;

    private void Awake()
    {
        cameraDistance = cameraDistance * -1;
    }

    private void Update()
    {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, cameraDistance);
    }
}