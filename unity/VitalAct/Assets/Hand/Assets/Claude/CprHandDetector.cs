using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class CprHandDetector : MonoBehaviour
{
    [Header("Calibration")]
    public float baselineY = 0.5f;
    public float maxDepthY = 0.15f;

    [Header("Animator")]
    public Animator chestAnimator;

    [Header("Output")]
    public float compressionDepth01;
    public float compressionRate;
    public bool handsOnMannequin;

    bool _wasPressed;
    float _lastPressTime;
    List<float> _intervals = new List<float>();
    float _lastRawY;
    float _pendingRawY;
    bool _hasNewData;
    bool _handVisible;

    void OnEnable()
    {
        HandLandmarkerRunner.OnHandLandmarkResult += OnHandLandmarkResult;
    }

    void OnDisable()
    {
        HandLandmarkerRunner.OnHandLandmarkResult -= OnHandLandmarkResult;
    }

    void OnHandLandmarkResult(HandLandmarkerResult result)
    {
        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
        {
            _handVisible = false;
            return;
        }
        var lm = result.handLandmarks[0].landmarks;
        _pendingRawY = (lm[0].y + lm[5].y + lm[9].y) / 3f;
        _handVisible = true;
        _hasNewData = true;
    }

    void Update()
    {
        if (!_hasNewData) return;
        _hasNewData = false;

        if (!_handVisible)
        {
            handsOnMannequin = false;
            if (chestAnimator != null)
            {
                chestAnimator.speed = 0f;
                chestAnimator.Play("Compress", 0, 0f);
            }
            return;
        }

        _lastRawY = _pendingRawY;
        float depth = Mathf.InverseLerp(baselineY, baselineY - maxDepthY, _pendingRawY);
        compressionDepth01 = Mathf.Clamp01(depth);
        handsOnMannequin = compressionDepth01 > 0.05f;

        // เล่น animation ตาม depth โดยตรง
        if (chestAnimator != null)
        {
            chestAnimator.speed = 0f;
            chestAnimator.Play("Compress", 0, compressionDepth01);
        }

        DetectCompression(compressionDepth01);
    }

    void DetectCompression(float depth)
    {
        bool pressed = depth > 0.1f;

        if (pressed && !_wasPressed)
        {
            float now = Time.time;
            if (_lastPressTime > 0f)
            {
                _intervals.Add(now - _lastPressTime);
                if (_intervals.Count > 8) _intervals.RemoveAt(0);
            }
            _lastPressTime = now;

            if (_intervals.Count > 1)
            {
                float avg = 0f;
                foreach (var t in _intervals) avg += t;
                compressionRate = 60f / (avg / _intervals.Count);
            }
        }
        _wasPressed = pressed;
    }

    public void Calibrate()
    {
        baselineY = _lastRawY;
        Debug.Log($"[CPR] Calibrated! baselineY = {baselineY:F4}");
    }
}