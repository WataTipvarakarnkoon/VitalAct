using UnityEngine;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;

// ควบคุมกล้องด้วย left_wrist(15) จาก Pose tracking
// ขยับข้อมือซ้ายไปทิศไหน → กล้องหันตาม
public class CameraController : MonoBehaviour
{
    [Header(“Camera Settings”)]
    public float rotateSpeed = 120f;
    public float smoothSpeed = 6f;

    [Header(“Wrist Tracking”)]
    public float gestureHoldTime = 0.08f;

    private PoseLandmarkerResult latestResult;
    private bool hasResult = false;

    private Vector2 lastPos = Vector2.zero;
    private bool hasLastPos = false;
    private Vector2 smoothDelta = Vector2.zero;

    private float targetYaw = 0f;
    private float targetPitch = 0f;
    private float currentYaw = 0f;
    private float currentPitch = 0f;

    void Start()
    {
        PoseLandmarkerRunner.OnPoseLandmarkResult += OnReceiveResult;

        currentYaw = transform.eulerAngles.y;
        currentPitch = transform.eulerAngles.x;
        targetYaw = currentYaw;
        targetPitch = currentPitch;
    }

    void OnDestroy()
    {
        PoseLandmarkerRunner.OnPoseLandmarkResult -= OnReceiveResult;
    }

    private void OnReceiveResult(PoseLandmarkerResult result)
    {
        latestResult = result;
        hasResult = true;
    }

    void Update()
    {
        if (!hasResult || latestResult.poseLandmarks == null || latestResult.poseLandmarks.Count == 0)
        {
            ResetTracking();
            ApplyRotation();
            return;
        }

        var landmarks = latestResult.poseLandmarks[0].landmarks;
        if (landmarks == null || landmarks.Count < 17)
        {
            ResetTracking();
            ApplyRotation();
            return;
        }

        // ใช้ left_wrist(15) ควบคุมกล้อง
        var wrist = landmarks[15];
        Vector2 center = new Vector2(wrist.x, wrist.y);

        if (hasLastPos)
        {
            Vector2 rawDelta = center - lastPos;
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

        ApplyRotation();
    }

    void ResetTracking()
    {
        hasLastPos = false;
        smoothDelta = Vector2.zero;
    }

    void ApplyRotation()
    {
        currentYaw = Mathf.Lerp(currentYaw, targetYaw, Time.deltaTime * smoothSpeed);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * smoothSpeed);

        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
}