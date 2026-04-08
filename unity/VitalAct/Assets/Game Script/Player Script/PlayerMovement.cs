using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public CprTimerFill timerFill;
    public ChecklistUI checklist;
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float smoothTime = 0.05f;

    [Header("CPR Lock (GameState.Do)")]
    public Transform cprLockPoint;

    private float rotationX = 0f;
    private float targetRotationX = 0f;
    private float currentYaw = 0f;
    private float targetYaw = 0f;
    private float smoothVelocityX = 0f;
    private float smoothVelocityY = 0f;

    private CharacterController characterController;
    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        targetYaw = transform.eulerAngles.y;
        currentYaw = targetYaw;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        bool isDo = GameManager.instance.CurrentState == GameManager.GameState.Do;

        // Snap to lock point in GameState.Do
        if (isDo)
        {
            if (cprLockPoint != null)
            {
                transform.position = cprLockPoint.position;
                transform.rotation = Quaternion.Euler(0f, cprLockPoint.eulerAngles.y, 0f);
                playerCamera.transform.localRotation = Quaternion.Euler(cprLockPoint.eulerAngles.x, 0f, 0f);

                // Sync internal state so camera doesn't snap back when unlocked
                currentYaw = cprLockPoint.eulerAngles.y;
                targetYaw  = currentYaw;
                rotationX  = cprLockPoint.eulerAngles.x;
                targetRotationX = rotationX;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        if (checklist.IsOpen || GameManager.instance.CurrentState == GameManager.GameState.Choose ||                GameManager.instance.CurrentState == GameManager.GameState.Result || timerFill != null && timerFill.timer <= 0f)
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
                inputDelta = touch.deltaPosition * 0.1f * lookSpeed;
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
        targetYaw += inputDelta.x;

        // Smooth rotation
        rotationX  = Mathf.SmoothDamp(rotationX,  targetRotationX, ref smoothVelocityX, smoothTime);
        currentYaw = Mathf.SmoothDamp(currentYaw, targetYaw,       ref smoothVelocityY, smoothTime);

        // Apply to camera and player
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
    }
}
