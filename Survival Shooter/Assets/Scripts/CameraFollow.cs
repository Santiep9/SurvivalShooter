using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    [SerializeField] float smoothSpeed = 8f;
    [SerializeField] Vector3 offset;

    [SerializeField] float freeCamSpeed = 15f;
    [SerializeField] int edgeSize = 25;

    bool freeCamera = false;

    InputActions input;
    InputAction toggleCameraAction;

    void Awake()
    {
        input = new InputActions();
        input.Main.Enable();

        toggleCameraAction = input.Main.CameraToggle;
    }

    void Update()
    {
        if (toggleCameraAction.WasPressedThisFrame())
        {
            freeCamera = !freeCamera;

            Debug.Log(freeCamera ? "Camara libre" : "Camara bloqueada");
        }

        if (freeCamera)
        {
            MoveFreeCamera();
        }
        else
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    void MoveFreeCamera()
    {
        Vector3 move = Vector3.zero;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (mousePos.x <= edgeSize)
            move.x -= 1;

        if (mousePos.x >= Screen.width - edgeSize)
            move.x += 1;

        if (mousePos.y <= edgeSize)
            move.z -= 1;

        if (mousePos.y >= Screen.height - edgeSize)
            move.z += 1;

        transform.position += move.normalized * freeCamSpeed * Time.deltaTime;
    }
}