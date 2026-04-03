using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using System.Collections.Generic;
using Mediapipe.Tasks.Components.Containers;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraTarget;
    public float rotateSpeed = 120f;
    public float smoothSpeed = 6f;

    [Header("Gesture Settings")]
    public float spreadThreshold = 0.08f; // 2 นิ้ว
    public float palmThreshold = 0.15f;   // แบมือ

    private HandLandmarkerResult latestResult;
    private bool hasResult = false;

    private Vector2 lastPos = Vector2.zero;
    private bool hasLastPos = false;

    private float targetYaw = 0f;
    private float targetPitch = 0f;
    private float currentYaw = 0f;
    private float currentPitch = 0f;

    private Vector2 smoothDelta = Vector2.zero;

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

    void OnReceiveResult(HandLandmarkerResult result)
    {
        latestResult = result;
        hasResult = true;
    }

    void Update()
    {
        if (!hasResult || latestResult.handLandmarks == null)
        {
            ResetTracking();
            ApplyRotation();
            return;
        }

        var landmarks = GetHandLandmarks();
        if (landmarks == null || landmarks.Count < 21)
        {
            ResetTracking();
            ApplyRotation();
            return;
        }

        bool isPalm = IsOpenPalm(landmarks);
        bool isTwoFinger = IsTwoFingerGesture(landmarks);

        // 🔥 ถ้าเป็นฝ่ามือ → ล็อกกล้องทันที (กัน CPR ไปขยับกล้อง)
        if (isPalm)
        {
            ResetTracking();
            ApplyRotation();
            return;
        }

        Vector2 fingerCenter = GetTwoFingerCenter(landmarks);

        if (isTwoFinger)
        {
            if (hasLastPos)
            {
                Vector2 rawDelta = fingerCenter - lastPos;

                smoothDelta = Vector2.Lerp(smoothDelta, rawDelta, Time.deltaTime * 15f);

                targetYaw -= smoothDelta.x * rotateSpeed;
                targetPitch -= smoothDelta.y * rotateSpeed;
                targetPitch = Mathf.Clamp(targetPitch, -80f, 80f);
            }
            else
            {
                smoothDelta = Vector2.zero;
            }

            lastPos = fingerCenter;
            hasLastPos = true;
        }
        else
        {
            ResetTracking();

            // inertia
            smoothDelta = Vector2.Lerp(smoothDelta, Vector2.zero, Time.deltaTime * 8f);

            targetYaw -= smoothDelta.x * rotateSpeed;
            targetPitch -= smoothDelta.y * rotateSpeed;
            targetPitch = Mathf.Clamp(targetPitch, -80f, 80f);
        }

        ApplyRotation();
    }

    // ------------------------
    // ✋ ตรวจจับแบมือ (CPR mode)
    // ------------------------
    bool IsOpenPalm(IList<NormalizedLandmark> lm)
    {
        var wrist = lm[0];

        float index = Vector2.Distance(ToVec2(lm[8]), ToVec2(wrist));
        float middle = Vector2.Distance(ToVec2(lm[12]), ToVec2(wrist));
        float ring = Vector2.Distance(ToVec2(lm[16]), ToVec2(wrist));
        float pinky = Vector2.Distance(ToVec2(lm[20]), ToVec2(wrist));

        return index > palmThreshold &&
               middle > palmThreshold &&
               ring > palmThreshold &&
               pinky > palmThreshold;
    }

    // ------------------------
    // ✌️ ตรวจจับ 2 นิ้ว (หมุนกล้อง)
    // ------------------------
    bool IsTwoFingerGesture(IList<NormalizedLandmark> lm)
    {
        var index = lm[8];
        var middle = lm[12];
        var ring = lm[16];
        var wrist = lm[0];

        float indexDist = Vector2.Distance(ToVec2(index), ToVec2(wrist));
        float middleDist = Vector2.Distance(ToVec2(middle), ToVec2(wrist));
        float ringDist = Vector2.Distance(ToVec2(ring), ToVec2(wrist));

        float spread = Vector2.Distance(ToVec2(index), ToVec2(middle));

        return spread > spreadThreshold &&
               indexDist > ringDist &&
               middleDist > ringDist;
    }

    // ------------------------
    // 🎯 จุดกลาง 2 นิ้ว
    // ------------------------
    Vector2 GetTwoFingerCenter(IList<NormalizedLandmark> lm)
    {
        var index = lm[8];
        var middle = lm[12];

        return new Vector2(
            (index.x + middle.x) / 2f,
            (index.y + middle.y) / 2f
        );
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

    void ResetTracking()
    {
        hasLastPos = false;
    }

    void ApplyRotation()
    {
        currentYaw = Mathf.Lerp(currentYaw, targetYaw, Time.deltaTime * smoothSpeed);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * smoothSpeed);

        if (cameraTarget != null)
        {
            transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

            float dist = Vector3.Distance(transform.position, cameraTarget.position);
            transform.position = cameraTarget.position - transform.forward * dist;
        }
        else
        {
            transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }
    }
}