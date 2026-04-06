using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class CprHandDetector : MonoBehaviour
{
    [Header("Mannequin Zone (Normalized 0-1, origin = top-left)")]
    [Tooltip("พื้นที่บนภาพกล้องที่หน้าอกหุ่น CPR อยู่ — ปรับจนกรอบสีเหลืองครอบหน้าอกหุ่นพอดี")]
    public Rect mannequinZone = new Rect(0.25f, 0.55f, 0.5f, 0.35f);

    [Header("Compression Depth")]
    [Tooltip("ระยะ Y (MediaPipe) ที่ถือว่ากดสุด ค่าน้อย = sensitive มากขึ้น")]
    public float maxDepthY = 0.10f;

    [Header("Animator")]
    public Animator chestAnimator;

    [Header("Output - Read Only")]
    public float compressionDepth01;
    public float compressionRate;
    public bool handsOnMannequin;

    // ---- MediaPipe thread → main thread ----
    volatile float _pendingRawX;
    volatile float _pendingRawY;
    volatile bool _hasNewData;
    volatile bool _handVisible;

    // ---- main thread only ----
    float _lastRawY;
    float _baselineY;
    bool _baselineSet;
    bool _wasOnMannequin;
    bool _wasPressed;
    float _lastPressTime;
    readonly List<float> _intervals = new List<float>();

    void OnEnable()  => HandLandmarkerRunner.OnHandLandmarkResult += OnHandLandmarkResult;
    void OnDisable() => HandLandmarkerRunner.OnHandLandmarkResult -= OnHandLandmarkResult;

    void OnHandLandmarkResult(HandLandmarkerResult result)
    {
        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
        {
            _handVisible = false;
            _hasNewData = true;
            return;
        }

        var lm = result.handLandmarks[0].landmarks;
        // ฝ่ามือ = เฉลี่ย wrist(0), index-base(5), middle-base(9)
        _pendingRawX = (lm[0].x + lm[5].x + lm[9].x) / 3f;
        _pendingRawY = (lm[0].y + lm[5].y + lm[9].y) / 3f;
        _handVisible = true;
        _hasNewData = true;
    }

    void Update()
    {
        if (!_hasNewData) return;
        _hasNewData = false;

        if (!_handVisible) { ResetState(); return; }

        float rawX = _pendingRawX;
        float rawY = _pendingRawY;
        _lastRawY = rawY;

        // MediaPipe X มาจากกล้องแบบ mirror → flip X ก่อนตรวจ zone
        float zoneX = 1f - rawX;
        float zoneY = rawY;  // Y=0 บน เหมือนกัน

        bool inZone = mannequinZone.Contains(new Vector2(zoneX, zoneY));

        if (!inZone)
        {
            if (_wasOnMannequin) OnLeaveZone();
            return;
        }

        // --- มืออยู่ในโซนหน้าอกหุ่น ---
        handsOnMannequin = true;

        // Auto-calibrate ทุกครั้งที่มือเข้าโซนใหม่ (จำ Y ตอนวางมือเป็น baseline)
        if (!_wasOnMannequin || !_baselineSet)
        {
            _baselineY = rawY;
            _baselineSet = true;
            _wasPressed = false;
            _lastPressTime = 0f;
            _intervals.Clear();
            compressionRate = 0f;
            Debug.Log($"[CPR] Auto-calibrated! baselineY={_baselineY:F4}");
        }
        _wasOnMannequin = true;

        // depth: Y เพิ่ม = กดลง (MediaPipe Y=0 บน → Y=1 ล่าง)
        float depth = Mathf.InverseLerp(_baselineY, _baselineY + maxDepthY, rawY);
        compressionDepth01 = Mathf.Clamp01(depth);

        if (chestAnimator != null)
        {
            chestAnimator.speed = 0f;
            chestAnimator.Play("Compress", 0, compressionDepth01);
        }

        DetectCompression(compressionDepth01);
    }

    void OnLeaveZone()
    {
        handsOnMannequin = false;
        compressionDepth01 = 0f;
        _baselineSet = false;
        _wasOnMannequin = false;

        if (chestAnimator != null)
        {
            chestAnimator.speed = 0f;
            chestAnimator.Play("Compress", 0, 0f);
        }
    }

    // ยิงทุกครั้งที่กดลง 1 ครั้ง
    public static event System.Action OnCompression;

    void DetectCompression(float depth)
    {
        bool pressed = depth > 0.15f;

        if (pressed && !_wasPressed)
        {
            float now = Time.time;
            if (_lastPressTime > 0f)
            {
                float interval = now - _lastPressTime;
                if (interval < 3f)
                {
                    _intervals.Add(interval);
                    if (_intervals.Count > 8) _intervals.RemoveAt(0);
                }
            }
            _lastPressTime = now;

            if (_intervals.Count >= 1)
            {
                float avg = 0f;
                foreach (var t in _intervals) avg += t;
                compressionRate = 60f / (avg / _intervals.Count);
            }

            OnCompression?.Invoke();
        }
        _wasPressed = pressed;
    }

    void ResetState()
    {
        handsOnMannequin = false;
        compressionDepth01 = 0f;
        compressionRate = 0f;
        _wasPressed = false;
        _lastPressTime = 0f;
        _intervals.Clear();
        _baselineSet = false;
        _wasOnMannequin = false;

        if (chestAnimator != null)
        {
            chestAnimator.speed = 0f;
            chestAnimator.Play("Compress", 0, 0f);
        }
    }

    [Header("Debug Zone")]
    [Tooltip("ลาก RawImage ของ camera feed มาใส่ — zone จะวาดทับบน camera โดยตรง")]
    public UnityEngine.UI.RawImage cameraPreview;
    [Tooltip("เปิดเพื่อดูกรอบ zone บน camera feed")]
    public bool showDebugZone = true;

    void OnGUI()
    {
        if (!showDebugZone) return;

        Rect camRect = GetCameraScreenRect();
        if (camRect.width <= 0) return;

        // แปลง mannequinZone → screen pixels บน camera feed
        Rect zoneRect = new Rect(
            camRect.x + mannequinZone.x * camRect.width,
            camRect.y + mannequinZone.y * camRect.height,
            mannequinZone.width  * camRect.width,
            mannequinZone.height * camRect.height
        );

        bool hit    = handsOnMannequin;
        Color fill  = hit ? new Color(0f, 1f, 0f, 0.30f) : new Color(1f, 1f, 0f, 0.15f);
        Color border= hit ? Color.green : Color.yellow;
        float thick = 3f;

        // เส้นกรอบ
        GUI.color = fill;
        GUI.DrawTexture(zoneRect, Texture2D.whiteTexture);
        GUI.color = Color.white;
        DrawBorder(zoneRect, thick, border);

        // label บอกว่าต้องวางมือที่นี่
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize  = Mathf.RoundToInt(camRect.height * 0.08f),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter
        };
        style.normal.textColor = border;

        string label = hit ? "Hand Detected!" : "Place hands here";
        float labelH = style.fontSize + 4;
        GUI.Label(new Rect(zoneRect.x, zoneRect.y - labelH, zoneRect.width, labelH),
                  label, style);

        // วาดกรอบรอบ camera feed ด้วย เพื่อให้รู้ขอบเขต
        DrawBorder(camRect, 2f, new Color(0.5f, 0.5f, 0.5f, 0.8f));
    }

    Rect GetCameraScreenRect()
    {
        if (cameraPreview != null)
        {
            Vector3[] corners = new Vector3[4];
            cameraPreview.rectTransform.GetWorldCorners(corners);
            float x = corners[0].x;
            float y = Screen.height - corners[2].y;  // flip Y (GUI: Y=0 บน)
            float w = corners[2].x - corners[0].x;
            float h = corners[2].y - corners[0].y;
            return new Rect(x, y, w, h);
        }

        // fallback: ใช้ทั้งหน้าจอ (จะทำให้ zone ใหญ่มาก)
        return new Rect(0, 0, Screen.width, Screen.height);
    }

    static void DrawBorder(Rect r, float t, Color c)
    {
        GUI.color = c;
        GUI.DrawTexture(new Rect(r.x,        r.y,        r.width, t), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(r.x,        r.yMax - t, r.width, t), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(r.x,        r.y,        t, r.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(r.xMax - t, r.y,        t, r.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
}
