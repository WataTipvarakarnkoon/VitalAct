using UnityEngine;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;

/// <summary>
/// Drop-in stabilizer for the CPR hand-tracking pipeline.
/// Attach to any active GameObject in the Hand scene.
///
/// Improvements applied:
///   1. Hand-loss buffer  — holds the last good result for N frames before
///      treating it as truly lost. Eliminates flicker from 1-3 dropped frames.
///   2. Frame-rate-independent smoothing helper — use SmoothToward() instead
///      of Vector3.Lerp(a, b, Time.deltaTime * speed) everywhere.
///   3. Adaptive speed — boosts smoothing during fast CPR motion so the
///      visualizer keeps up, backs off when idle to reduce noise.
/// </summary>
public class CprTrackingStabilizer : MonoBehaviour
{
    [Header("Hand-loss buffer")]
    [Tooltip("Frames to hold the last good result before treating the hand as lost. " +
             "5-8 is enough to bridge motion-blur gaps without adding visible lag.")]
    public int lostFrameGrace = 6;

    [Header("Smoothing")]
    [Tooltip("Half-life in seconds for joint position smoothing. " +
             "0.04 = ~2.5 frames at 60 fps — fast enough for 100 bpm CPR.")]
    public float smoothHalfLife = 0.04f;

    [Tooltip("Half-life used when the hand is moving fast (above velocityThreshold). " +
             "Lower = more responsive during active compression.")]
    public float fastHalfLife = 0.025f;

    [Tooltip("World-space velocity (units/s) above which fast smoothing activates.")]
    public float velocityThreshold = 0.4f;

    /// <summary>Default velocity threshold exposed so other scripts can reference it.</summary>
    public const float VelocityThresholdDefault = 0.4f;

    // ---- public state that SimpleHandVisualizer / ProceduralHand can read ----

    /// <summary>
    /// Latest stable result. May be the real MediaPipe output or the last
    /// cached one if we are inside the grace window.
    /// </summary>
    public static PoseLandmarkerResult StableResult { get; private set; }

    /// <summary>
    /// True only when we have a result that is still within the grace window.
    /// </summary>
    public static bool HasStableResult { get; private set; }

    // ---- internals ----

    private PoseLandmarkerResult _lastGoodResult;
    private int _missingFrames = 0;

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
        bool hasHands = result.poseLandmarks != null && result.poseLandmarks.Count > 0;

        if (hasHands)
        {
            _lastGoodResult = result;
            _missingFrames = 0;
            StableResult = result;
            HasStableResult = true;
        }
        else
        {
            _missingFrames++;
            if (_missingFrames <= lostFrameGrace)
            {
                // Hold the last good result; don't expose "no hands" yet.
                StableResult = _lastGoodResult;
                HasStableResult = true;
            }
            else
            {
                StableResult = result;     // real empty result
                HasStableResult = false;
            }
        }
    }

    // ---- Static smoothing helpers (use these in your visualizers) ----

    /// <summary>
    /// Frame-rate-independent exponential approach.
    /// Replaces: Vector3.Lerp(current, target, Time.deltaTime * speed)
    ///
    /// Usage:
    ///   hand.positions[i] = CprTrackingStabilizer.SmoothToward(
    ///       hand.positions[i], target, smoothHalfLife, Time.deltaTime);
    /// </summary>
    public static Vector3 SmoothToward(Vector3 current, Vector3 target,
                                        float halfLife, float dt)
    {
        float t = 1f - Mathf.Pow(0.5f, dt / Mathf.Max(halfLife, 0.0001f));
        return Vector3.Lerp(current, target, t);
    }

    /// <summary>
    /// Adaptive version: uses fastHalfLife when the joint is moving fast,
    /// otherwise uses normalHalfLife.
    /// </summary>
    public static Vector3 SmoothTowardAdaptive(Vector3 current, Vector3 target,
                                                float normalHalfLife, float fastHalfLife,
                                                float velocityThreshold, float dt)
    {
        float dist = Vector3.Distance(current, target);
        float speed = dist / Mathf.Max(dt, 0.0001f);
        float hl = speed > velocityThreshold ? fastHalfLife : normalHalfLife;
        return SmoothToward(current, target, hl, dt);
    }
}
