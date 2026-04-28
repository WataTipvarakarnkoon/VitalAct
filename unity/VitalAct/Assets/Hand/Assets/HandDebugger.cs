using UnityEngine;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;

public class HandDebugger : MonoBehaviour
{
    void OnEnable()
    {
        PoseLandmarkerRunner.OnPoseLandmarkResult += OnResult;
    }

    void OnDisable()
    {
        PoseLandmarkerRunner.OnPoseLandmarkResult -= OnResult;
    }

    private void OnResult(PoseLandmarkerResult result)
    {
        if (result.poseLandmarks == null || result.poseLandmarks.Count == 0) return;

        var lm = result.poseLandmarks[0].landmarks;
        var lw = lm[15]; // left_wrist
        var rw = lm[16]; // right_wrist
        Debug.Log($"L-Wrist X:{lw.x:F2} Y:{lw.y:F2}  R-Wrist X:{rw.x:F2} Y:{rw.y:F2}");
    }
}