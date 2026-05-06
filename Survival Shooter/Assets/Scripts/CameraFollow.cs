using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 8f;
    [SerializeField] Vector3 offset;

    void Update()
    {
        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition,
        smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
