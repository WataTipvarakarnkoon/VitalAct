using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class CPRHandDetector : MonoBehaviour
{
    [Header("Settings")]
    public float maxRayDistance = 50f;
    public LayerMask cprLayer;          // เลือก Layer CPR ใน Inspector

    [Header("Press Sensitivity")]
    public float pressSpeed = 3f;   // ความเร็วสะสมแรงกด
    public float releaseSpeed = 5f;   // ความเร็วคืนค่า
    public float pressThreshold = 0.3f; // ค่าที่ถือว่ากดพอ

    private HandLandmarkerResult latestResult;
    private bool hasResult = false;
    private float pressValue = 0f;     // 0 = ไม่กด, 1 = กดเต็ม

    void Start()
    {
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
        var cprObjects = FindObjectsOfType<CPRObject>();

        bool isHandOverChest = CheckHandOverChest();

        if (isHandOverChest)
        {
            // สะสมแรงกดขึ้นเรื่อยๆ
            pressValue = Mathf.MoveTowards(pressValue, 1f, pressSpeed * Time.deltaTime);
        }
        else
        {
            // ปล่อยแรงกด
            pressValue = Mathf.MoveTowards(pressValue, 0f, releaseSpeed * Time.deltaTime);
        }

        foreach (var obj in cprObjects)
        {
            if (pressValue > pressThreshold)
                obj.ApplyPress(pressValue);
            else
                obj.ReleasePress();
        }
    }

    bool CheckHandOverChest()
    {
        if (!hasResult || latestResult.handLandmarks == null) return false;
        if (latestResult.handLandmarks.Count == 0) return false;

        for (int h = 0; h < latestResult.handLandmarks.Count; h++)
        {
            var landmarks = latestResult.handLandmarks[h].landmarks;
            if (landmarks == null || landmarks.Count < 10) continue;

            var wrist = landmarks[0];
            var middle = landmarks[9];

            // จุดกึ่งกลางฝ่ามือบนหน้าจอ
            Vector2 palmScreen = new Vector2(
                (wrist.x + middle.x) / 2f,
                1f - (wrist.y + middle.y) / 2f
            );

            // ยิง Ray จากกล้องผ่านตำแหน่งมือบนหน้าจอ
            Ray ray = Camera.main.ViewportPointToRay(
                new Vector3(palmScreen.x, palmScreen.y, 0f));

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxRayDistance, cprLayer))
            {
                // โดน Collider ที่อยู่ใน Layer CPR
                return true;
            }
        }

        return false;
    }
}