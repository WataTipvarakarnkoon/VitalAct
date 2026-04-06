using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Failed fail;
    public ChecklistUI checklist;
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float smoothTime = 0.05f;

    private float rotationX = 0f;
    private float targetRotationX = 0f;
    private float yawDelta = 0f;
    private float targetYawDelta = 0f;
    private float smoothVelocityX = 0f;
    private float smoothVelocityY = 0f;

    private Quaternion initialRotation;
    private CharacterController characterController;
    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        initialRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Lock or unlock cursor based on game state
        if (checklist.IsOpen || fail.shouldFade || GameManager.instance.CurrentState == GameManager.GameState.Choose)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canMove = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canMove = true;
        }

        if (!canMove) return;

        Vector2 inputDelta = Vector2.zero;

        // Mobile touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
                inputDelta = touch.deltaPosition * 0.1f; // scale touch sensitivity
        }
        else
        {
            // Fallback to mouse input
            inputDelta.x = Input.GetAxis("Mouse X") * lookSpeed;
            inputDelta.y = Input.GetAxis("Mouse Y") * lookSpeed;
        }

        // Apply input
        targetRotationX -= inputDelta.y;
        targetRotationX = Mathf.Clamp(targetRotationX, -lookXLimit, lookXLimit);
        targetYawDelta += inputDelta.x;

        // Smooth rotation
        rotationX = Mathf.SmoothDamp(rotationX, targetRotationX, ref smoothVelocityX, smoothTime);
        yawDelta = Mathf.SmoothDamp(yawDelta, targetYawDelta, ref smoothVelocityY, smoothTime);

        // Apply to camera and player
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.rotation = initialRotation * Quaternion.Euler(0f, yawDelta, 0f);
    }
}