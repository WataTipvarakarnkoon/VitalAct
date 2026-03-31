using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class CPRHandDetector : MonoBehaviour
{
    [Header("Settings")]
    public float handDepth = 3f;       // ต้อง = |Camera.z - Object.z|
    public float detectRadius = 0.5f;  // เพิ่มให้ติดง่าย

    private HandLandmarkerResult latestResult;
    private bool hasResult = false;

    void Start()
    {
        HandLandmarkerRunner.OnHandLandmarkResult += OnReceiveResult;
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
        if (!hasResult || latestResult.handLandmarks == null) return;

        var cprObjects = FindObjectsOfType<CPRObject>();
        if (cprObjects.Length == 0) return;

        // reset ทุก object ก่อน
        foreach (var obj in cprObjects)
            obj.ReleasePress();

        for (int h = 0; h < latestResult.handLandmarks.Count; h++)
        {
            var landmarks = latestResult.handLandmarks[h].landmarks;
            if (landmarks == null || landmarks.Count < 21) continue;

            var wrist = landmarks[0];
            var middleMCP = landmarks[9];

            // ตำแหน่งฝ่ามือ (world)
            Vector3 palmWorld = Camera.main.ViewportToWorldPoint(new Vector3(
                (wrist.x + middleMCP.x) / 2f,
                1f - (wrist.y + middleMCP.y) / 2f,
                handDepth
            ));

            // เอา Y ของข้อมือ
            Vector3 wristWorld = Camera.main.ViewportToWorldPoint(
                new Vector3(wrist.x, 1f - wrist.y, handDepth));

            foreach (var obj in cprObjects)
            {
                Vector3 objPos = obj.transform.position;

                float horizDist = Vector2.Distance(
                    new Vector2(palmWorld.x, palmWorld.z),
                    new Vector2(objPos.x, objPos.z)
                );

                // 🔥 ตัดเงื่อนไข Y ออก ให้ติดก่อน
                if (horizDist < detectRadius)
                {
                    obj.ApplyPress(wristWorld.y);

                    // debug เส้น
                    Debug.DrawLine(palmWorld, objPos, Color.red);
                }
            }
        }
    }
}