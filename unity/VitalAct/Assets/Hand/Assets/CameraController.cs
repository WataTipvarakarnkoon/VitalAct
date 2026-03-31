using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraTarget;
    public float rotateSpeed = 180f;
    public float smoothSpeed = 8f;

    [Header("Pinch Detection")]
    public float pinchThreshold = 0.05f;  // ระยะที่ถือว่าจีบ

    private HandLandmarkerResult latestResult;
    private bool hasResult = false;

    private bool isPinching = false;
    private Vector2 lastPinchPos = Vector2.zero;
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
        if (!hasResult || latestResult.handLandmarks == null)
        {
            hasLastPos = false;
            ApplyRotation();
            return;
        }

        var landmarks = GetHandLandmarks();
        if (landmarks == null || landmarks.Count < 21)
        {
            hasLastPos = false;
            ApplyRotation();
            return;
        }

        // ตำแหน่งนิ้วโป้ง (4) และนิ้วชี้ (8)
        var thumb = landmarks[4];
        var index = landmarks[8];

        Vector2 thumbPos = new Vector2(thumb.x, thumb.y);
        Vector2 indexPos = new Vector2(index.x, index.y);

        float pinchDist = Vector2.Distance(thumbPos, indexPos);
        isPinching = pinchDist < pinchThreshold;

        // จุดกึ่งกลางระหว่างสองนิ้ว
        Vector2 pinchCenter = (thumbPos + indexPos) / 2f;

        if (isPinching)
        {
            if (hasLastPos)
            {
                Vector2 delta = pinchCenter - lastPinchPos;

                // เลื่อนซ้าย → หันขวา (กลับทิศ)
                // เลื่อนลง → หันขึ้น (กลับทิศ)
                targetYaw -= delta.x * rotateSpeed;
                targetPitch -= delta.y * rotateSpeed;
                targetPitch = Mathf.Clamp(targetPitch, -80f, 80f);
            }

            lastPinchPos = pinchCenter;
            hasLastPos = true;
        }
        else
        {
            hasLastPos = false;
        }

        ApplyRotation();
    }

    System.Collections.Generic.IList<Mediapipe.Tasks.Components.Containers.NormalizedLandmark> GetHandLandmarks()
    {
        if (latestResult.handLandmarks == null || latestResult.handLandmarks.Count == 0)
            return null;
        return latestResult.handLandmarks[0].landmarks;
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
