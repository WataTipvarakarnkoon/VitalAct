using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class CPRHandDetector : MonoBehaviour
{
    [Header("Settings")]
    public float maxRayDistance = 50f;
    public LayerMask cprLayer;
    public Animator victimAnimator;

    [Header("Press Sensitivity")]
    public float cooldown = 0.8f;

    private HandLandmarkerResult latestResult;
    private bool hasResult = false;
    private float cooldownTimer = 0f;

    void Start()
    {
        // Always subscribe — NoCameraMode may be auto-enabled after camera failure
        HandLandmarkerRunner.OnHandLandmarkResult += OnReceiveResult;
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
        if (GameManager.NoCameraMode)
        {
            HandleKeyboardInput();
            return;
        }

        cooldownTimer -= Time.deltaTime;

        if (!hasResult || latestResult.handLandmarks == null) return;
        if (latestResult.handLandmarks.Count == 0) return;
        if (cooldownTimer > 0f) return;

        bool isHandOverChest = CheckHandOverChest();

        if (isHandOverChest)
        {
            Debug.Log("CPR PRESS!");

            if (victimAnimator != null)
                victimAnimator.SetTrigger("compress");

            var cprObjects = FindObjectsOfType<CPRObject>();
            foreach (var obj in cprObjects)
                obj.ApplyPress(1f);

            cooldownTimer = cooldown;
        }
        else
        {
            var cprObjects = FindObjectsOfType<CPRObject>();
            foreach (var obj in cprObjects)
                obj.ReleasePress();
        }
    }

    void HandleKeyboardInput()
    {
        // No camera mode CPR is handled by Mouseraycast.cs (click on chest)
    }

    bool CheckHandOverChest()
    {
        for (int h = 0; h < latestResult.handLandmarks.Count; h++)
        {
            var landmarks = latestResult.handLandmarks[h].landmarks;
            if (landmarks == null || landmarks.Count < 10) continue;

            var wrist = landmarks[0];
            var middle = landmarks[9];

            Vector2 palmScreen = new Vector2(
                (wrist.x + middle.x) / 2f,
                1f - (wrist.y + middle.y) / 2f
            );

            Ray ray = Camera.main.ViewportPointToRay(
                new Vector3(palmScreen.x, palmScreen.y, 0f));

            Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.green);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxRayDistance, cprLayer))
            {
                Debug.Log("HIT: " + hit.collider.name);
                return true;
            }
        }
        return false;
    }
}