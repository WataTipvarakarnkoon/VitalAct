using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    GameManager gameManager;
    public Failed fail;
    public ChecklistUI checklist;
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float smoothTime = 0.05f;
    private float rotationX = 0;
    private float targetRotationX = 0;
    private float yawDelta = 0;
    private float targetYawDelta = 0;
    private float smoothVelocityX = 0;
    private float smoothVelocityY = 0;
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
        if (checklist.IsOpen || fail.shouldFade == true || GameManager.instance.CurrentState == GameManager.GameState.Choose)
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

        if (canMove)
        {
            targetRotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
            targetRotationX = Mathf.Clamp(targetRotationX, -lookXLimit, lookXLimit);
            targetYawDelta += Input.GetAxis("Mouse X") * lookSpeed;

            rotationX = Mathf.SmoothDamp(rotationX, targetRotationX, ref smoothVelocityX, smoothTime);
            yawDelta = Mathf.SmoothDamp(yawDelta, targetYawDelta, ref smoothVelocityY, smoothTime);

            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation = initialRotation * Quaternion.Euler(0, yawDelta, 0);
        }
    }
}