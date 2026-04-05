using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using System.Collections.Generic;
using Mediapipe.Tasks.Components.Containers;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraTarget;
    public float smoothSpeed = 8f;

    [Header("Input Settings")]
    public bool useMouseInput = true;
    public bool useHandTracking = true;
    public float mouseLookSpeed = 2f;
    public ChecklistUI checklist;

    [Header("Gesture Settings")]
    public float rotateSpeed = 150f;
    public float fistEnter = 0.07f;   // ต้องกำจริงถึงเข้า
    public float fistExit = 0.11f;    // ต้องแบจริงถึงออก
    public float deadzone = 0.01f;    // กัน jitter

    private HandLandmarkerResult latestResult;
    private bool hasResult = false;

    private bool isFist = false;
    private bool isPalm = false;

    private Vector2 lastHandPos = Vector2.zero;
    private bool hasLastPos = false;

    private float targetYaw = 0f;
    private float targetPitch = 0f;
    private float currentYaw = 0f;
    private float currentPitch = 0f;

    void Start()
    {
        HandLandmarkerRunner.OnHandLandmarkResult += OnReceiveResult;

        currentYaw = transform.eulerAngles.y;
        currentPitch = transform.eulerAngles.x;
        targetYaw = currentYaw;
        targetPitch = currentPitch;
    }

    void OnDestroy()
    {
        HandLandmarkerRunner.OnHandLandmarkResult -= OnReceiveResult;
    }

    private void OnReceiveResult(HandLandmarkerResult result)
    {
        latestResult = result;
        hasResult = true;
    }

    void Update()
    {
        HandleMouseInput();
        HandleHandTracking();
        ApplyRotation();
    }

    void HandleMouseInput()
    {
        if (!useMouseInput) return;

        bool checklistOpen = checklist != null && checklist.IsOpen;

        if (checklistOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        // Lock cursor for free look
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(mouseX) > 0f || Mathf.Abs(mouseY) > 0f)
        {
            targetYaw   += mouseX * mouseLookSpeed;
            targetPitch -= mouseY * mouseLookSpeed;
            targetPitch  = Mathf.Clamp(targetPitch, -80f, 80f);
        }
    }

    void HandleHandTracking()
    {
        if (!useHandTracking)
        {
            hasLastPos = false;
            return;
        }

        if (!hasResult || latestResult.handLandmarks == null)
        {
            hasLastPos = false;
            return;
        }

        var landmarks = GetHandLandmarks();
        if (landmarks == null || landmarks.Count < 21)
        {
            hasLastPos = false;
            return;
        }

        // ===== ตรวจ gesture =====
        int extendedCount = CountExtendedFingers(landmarks);
        float avgDist = AverageFingerDistance(landmarks);

        // ✋ แบมือ (CPR mode)
        isPalm = extendedCount >= 3;

        // ✊ กำปั้น (ใช้ hysteresis กันเด้ง)
        if (!isFist && avgDist < fistEnter && extendedCount <= 1)
            isFist = true;
        else if (isFist && avgDist > fistExit)
            isFist = false;

        // ===== ตำแหน่งมือ =====
        var wrist = landmarks[0];
        var middleMCP = landmarks[9];

        Vector2 handPos = new Vector2(
            (wrist.x + middleMCP.x) / 2f,
            (wrist.y + middleMCP.y) / 2f
        );

        // ===== logic =====
        if (isPalm)
        {
            // ✋ CPR = ล็อคกล้อง
            hasLastPos = false;
        }
        else if (isFist)
        {
            if (hasLastPos)
            {
                Vector2 delta = handPos - lastHandPos;

                if (delta.magnitude > deadzone)
                {
                    targetYaw -= delta.x * rotateSpeed;
                    targetPitch -= delta.y * rotateSpeed;
                    targetPitch = Mathf.Clamp(targetPitch, -80f, 80f);
                }
            }

            lastHandPos = handPos;
            hasLastPos = true;
        }
        else
        {
            hasLastPos = false;
        }
    }

    // ===== Helper =====

    int CountExtendedFingers(IList<NormalizedLandmark> lm)
    {
        int count = 0;

        if (lm[8].y < lm[6].y) count++;   // index
        if (lm[12].y < lm[10].y) count++; // middle
        if (lm[16].y < lm[14].y) count++; // ring
        if (lm[20].y < lm[18].y) count++; // pinky

        return count;
    }

    float AverageFingerDistance(IList<NormalizedLandmark> lm)
    {
        float index  = Vector2.Distance(ToVec2(lm[8]),  ToVec2(lm[5]));
        float middle = Vector2.Distance(ToVec2(lm[12]), ToVec2(lm[9]));
        float ring   = Vector2.Distance(ToVec2(lm[16]), ToVec2(lm[13]));
        float pinky  = Vector2.Distance(ToVec2(lm[20]), ToVec2(lm[17]));

        return (index + middle + ring + pinky) / 4f;
    }

    Vector2 ToVec2(NormalizedLandmark lm)
    {
        return new Vector2(lm.x, lm.y);
    }

    IList<NormalizedLandmark> GetHandLandmarks()
    {
        if (latestResult.handLandmarks == null || latestResult.handLandmarks.Count == 0)
            return null;
        return latestResult.handLandmarks[0].landmarks;
    }

    void ApplyRotation()
    {
        currentYaw   = Mathf.Lerp(currentYaw,   targetYaw,   Time.deltaTime * smoothSpeed);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * smoothSpeed);

        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        if (cameraTarget != null)
        {
            float dist = Vector3.Distance(transform.position, cameraTarget.position);
            transform.position = cameraTarget.position - transform.forward * dist;
        }
    }
}
