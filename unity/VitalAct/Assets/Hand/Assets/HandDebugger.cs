using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class HandDebugger : MonoBehaviour
{
    void OnEnable()
    {
        HandLandmarkerRunner.OnHandLandmarkResult += OnResult;
    }

    void OnDisable()
    {
        HandLandmarkerRunner.OnHandLandmarkResult -= OnResult;
    }

    private void OnResult(HandLandmarkerResult result)
    {
        if (result.handLandmarks == null || result.handLandmarks.Count == 0) return;

        var tip = result.handLandmarks[0].landmarks[8];
        Debug.Log($"Index Tip X:{tip.x:F2} Y:{tip.y:F2} Z:{tip.z:F2}");
    }
}