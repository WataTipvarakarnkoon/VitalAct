using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float rotateSpeed = 120f;
    public float smoothSpeed = 6f;

    [Header("Gesture Settings")]
    public float fingerOffset = 0.02f;   // กัน jitter ตอนตรวจนิ้ว
    public float gestureHoldTime = 0.08f; // ต้องถือ gesture กี่วิถึงจะติด

    private HandLandmarkerResult latestResult;
    private bool hasResult = false;

    private bool isTwoFinger = false;
    private float gestureTimer = 0f;

    private Vector2 lastPos = Vector2.zero;
    private bool hasLastPos = false;
    private Vector2 smoothDelta = Vector2.zero;

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
        if (!hasResult || latestResult.handLandmarks == null || latestResult.handLandmarks.Count == 0)
        {
            ResetTracking();
            ApplyRotation();
            return;
        }

        var landmarks = latestResult.handLandmarks[0].landmarks;
        if (landmarks == null || landmarks.Count < 21)
        {
            ResetTracking();
            ApplyRotation();
            return;
        }

        bool detected = DetectTwoFinger(landmarks);

        // ⏱️ กัน gesture สั่น
        if (detected)
        {
            gestureTimer += Time.deltaTime;
            if (gestureTimer >= gestureHoldTime)
                isTwoFinger = true;
        }
        else
        {
            gestureTimer = 0f;
            isTwoFinger = false;
        }

        // 🎯 จุดกลางระหว่างนิ้วชี้ + กลาง
        var index = landmarks[8];
        var middle = landmarks[12];

        Vector2 center = new Vector2(
            (index.x + middle.x) / 2f,
            (index.y + middle.y) / 2f
        );

        if (isTwoFinger)
        {
            if (hasLastPos)
            {
                Vector2 rawDelta = center - lastPos;

                // smooth movement
                smoothDelta = Vector2.Lerp(smoothDelta, rawDelta, Time.deltaTime * 15f);

                targetYaw -= smoothDelta.x * rotateSpeed;
                targetPitch -= smoothDelta.y * rotateSpeed;
                targetPitch = Mathf.Clamp(targetPitch, -80f, 80f);
            }
            else
            {
                smoothDelta = Vector2.zero;
            }

            lastPos = center;
            hasLastPos = true;
        }
        else
        {
            hasLastPos = false;

            // ค่อยๆ หยุด
            smoothDelta = Vector2.Lerp(smoothDelta, Vector2.zero, Time.deltaTime * 8f);

            targetYaw -= smoothDelta.x * rotateSpeed;
            targetPitch += smoothDelta.y * rotateSpeed;
            targetPitch = Mathf.Clamp(targetPitch, -80f, 80f);
        }

        ApplyRotation();
    }

    void ResetTracking()
    {
        hasLastPos = false;
        smoothDelta = Vector2.zero;
        gestureTimer = 0f;
        isTwoFinger = false;
    }

    // 🔥 เวอร์ชันใหม่: ตรวจ “งอ/เหยียดนิ้ว” แทนระยะ
    bool DetectTwoFinger(IList<Mediapipe.Tasks.Components.Containers.NormalizedLandmark> lm)
    {
        // index finger
        bool indexUp = lm[8].y < lm[6].y - fingerOffset;

        // middle finger
        bool middleUp = lm[12].y < lm[10].y - fingerOffset;

        // ring finger
        bool ringDown = lm[16].y > lm[14].y + fingerOffset;

        // pinky
        bool pinkyDown = lm[20].y > lm[18].y + fingerOffset;

        return indexUp && middleUp && ringDown && pinkyDown;
    }

    void ApplyRotation()
    {
        currentYaw = Mathf.Lerp(currentYaw, targetYaw, Time.deltaTime * smoothSpeed);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * smoothSpeed);

        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
}