using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class CprHandDetector : MonoBehaviour
{
    // ──────────────────────────────────────────────────────
    //  ZONE  —  ปรับกรอบให้ครอบหน้าอกหุ่นในภาพกล้อง
    // ──────────────────────────────────────────────────────
    [Header("Mannequin Zone (0-1, top-left origin)")]
    [Tooltip("ปรับกรอบให้ครอบหน้าอกหุ่น CPR ในภาพกล้อง\nต้องครอบทั้งตำแหน่งวาง AND ตำแหน่งกดลงสุด")]
    public Rect mannequinZone = new Rect(0.15f, 0.35f, 0.70f, 0.65f);

    // ──────────────────────────────────────────────────────
    //  SMOOTHING
    // ──────────────────────────────────────────────────────
    [Header("Smoothing")]
    [Range(0.05f, 0.8f)]
    [Tooltip("ค่าน้อย = smooth มาก (ตอบสนองช้า), ค่ามาก = responsive (noise มาก)\nแนะนำ: 0.45-0.60 สำหรับ CPR ที่กดเร็ว")]
    public float smoothAlpha = 0.50f;

    // ──────────────────────────────────────────────────────
    //  COMPRESSION DETECTION
    // ──────────────────────────────────────────────────────
    [Header("Compression Detection")]
    [Tooltip("ระยะ Y (normalized) ที่ถือว่ากดสุด\nค่าน้อย = sensitive มากขึ้น  |  แนะนำ: 0.04-0.08\nดู Depth bar ขณะกด ถ้าไม่เต็มให้ลดค่านี้ลง")]
    public float maxDepthY = 0.05f;

    [Range(0.1f, 0.7f)]
    [Tooltip("depth ที่ถือว่า 'กำลังกด' (ไม่ควรสูงเกิน 0.5)\nแนะนำ: 0.25-0.35")]
    public float pressThreshold = 0.30f;

    [Range(0.05f, 0.5f)]
    [Tooltip("depth ที่ถือว่า 'ปล่อย' — ต้องต่ำกว่า pressThreshold เสมอ\nแนะนำ: 0.10-0.20")]
    public float releaseThreshold = 0.12f;

    [Tooltip("ระยะเวลาขั้นต่ำระหว่างการกด 2 ครั้ง (วินาที) — กันการนับซ้ำ\nแนะนำ: 0.25-0.35")]
    public float minCompressionInterval = 0.28f;

    [Tooltip("เวลาที่มือออกจาก zone ก่อน baseline จะ reset (วินาที)\nเพิ่มให้สูงถ้า baseline reset บ่อยเกินไป")]
    public float baselineResetDelay = 1.5f;

    // ──────────────────────────────────────────────────────
    //  REFERENCES
    // ──────────────────────────────────────────────────────
    [Header("Animator")]
    public Animator chestAnimator;

    // ──────────────────────────────────────────────────────
    //  OUTPUT (read-only)
    // ──────────────────────────────────────────────────────
    [Header("Output – Read Only")]
    public float compressionDepth01;
    public float compressionRate;
    public bool  handsOnMannequin;

    // ──────────────────────────────────────────────────────
    //  DEBUG
    // ──────────────────────────────────────────────────────
    [Header("Debug Zone")]
    public UnityEngine.UI.RawImage cameraPreview;
    public bool showDebugZone = true;
    [Tooltip("แสดง depth bar และค่าตัวเลขบนหน้าจอ")]
    public bool showDepthHUD  = true;

    // ──────────────────────────────────────────────────────
    //  EVENT
    // ──────────────────────────────────────────────────────
    public static event System.Action OnCompression;

    // ──────────────────────────────────────────────────────
    //  INTERNAL
    // ──────────────────────────────────────────────────────
    // thread-safe (MediaPipe → main)
    volatile float _pendingRawX;
    volatile float _pendingRawY;
    volatile bool  _hasNewData;
    volatile bool  _handVisible;

    // main thread only
    float _smoothY;
    bool  _smoothInit;

    float _baselineY;
    bool  _baselineSet;

    bool  _wasOnMannequin;
    float _leaveTime = -1f;

    // peak detection state
    float _prevDepth;
    float _peakDepth;
    bool  _goingDown;

    float _lastPressTime;
    readonly List<float> _intervals = new List<float>();

    // ──────────────────────────────────────────────────────

    void OnEnable()  => HandLandmarkerRunner.OnHandLandmarkResult += OnHandLandmarkResult;
    void OnDisable() => HandLandmarkerRunner.OnHandLandmarkResult -= OnHandLandmarkResult;

    // MediaPipe thread
    void OnHandLandmarkResult(HandLandmarkerResult result)
    {
        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
        {
            _handVisible = false;
            _hasNewData  = true;
            return;
        }

        var lm = result.handLandmarks[0].landmarks;
        // ใช้ wrist(0), index-MCP(5), middle-MCP(9), ring-MCP(13), pinky-MCP(17)
        // เพิ่มจุดอ้างอิง → stable กว่าใช้แค่ 3 จุด
        _pendingRawX = (lm[0].x + lm[5].x + lm[9].x + lm[13].x + lm[17].x) / 5f;
        _pendingRawY = (lm[0].y + lm[5].y + lm[9].y + lm[13].y + lm[17].y) / 5f;
        _handVisible = true;
        _hasNewData  = true;
    }

    void Update()
    {
        if (!_hasNewData) return;
        _hasNewData = false;

        if (!_handVisible)
        {
            OnHandAbsent();
            return;
        }

        float rawX = _pendingRawX;
        float rawY = _pendingRawY;

        // ── Exponential Moving Average smoothing ──
        if (!_smoothInit) { _smoothY = rawY; _smoothInit = true; }
        _smoothY = _smoothY + smoothAlpha * (rawY - _smoothY);

        // flip X (MediaPipe mirror)
        float zoneX = 1f - rawX;
        float zoneY = _smoothY;

        bool inZone = mannequinZone.Contains(new Vector2(zoneX, zoneY));

        if (!inZone)
        {
            if (_wasOnMannequin && _leaveTime < 0f)
                _leaveTime = Time.time;   // เริ่มจับเวลาที่ออก

            // baseline reset เฉพาะเมื่ออยู่นอก zone นานพอ
            if (_leaveTime > 0f && Time.time - _leaveTime > baselineResetDelay)
                ClearBaseline();

            handsOnMannequin      = false;
            compressionDepth01    = Mathf.MoveTowards(compressionDepth01, 0f, Time.deltaTime * 4f);
            return;
        }

        // ── มืออยู่ใน zone ──
        _leaveTime = -1f;  // ยกเลิก countdown reset

        // set baseline ตอนเข้า zone ใหม่
        if (!_wasOnMannequin || !_baselineSet)
        {
            _baselineY   = _smoothY;
            _baselineSet = true;
            Debug.Log($"[CPR] Baseline set: Y={_baselineY:F4}");
        }

        _wasOnMannequin  = true;
        handsOnMannequin = true;

        // depth (Y เพิ่ม = กดลง)
        float depth = Mathf.InverseLerp(_baselineY, _baselineY + maxDepthY, _smoothY);
        compressionDepth01 = Mathf.Clamp01(depth);

        if (chestAnimator != null)
        {
            chestAnimator.speed = 0f;
            chestAnimator.Play("Compress", 0, compressionDepth01);
        }

        DetectCompression(compressionDepth01);
    }

    // ── Peak detection: ตรวจจุดกดสูงสุดตอนมือเริ่มขึ้น ──
    // วิธีนี้จับการกดเร็วได้ดีกว่า threshold crossing
    void DetectCompression(float depth)
    {
        float delta = depth - _prevDepth;
        _prevDepth = depth;

        const float DOWN_DELTA  =  0.008f;  // เริ่มกดลง
        const float UP_DELTA    = -0.008f;  // เริ่มขึ้น

        if (!_goingDown && delta > DOWN_DELTA)
        {
            _goingDown = true;
            _peakDepth = depth;
        }
        else if (_goingDown)
        {
            if (depth > _peakDepth) _peakDepth = depth;  // track peak

            if (delta < UP_DELTA)   // มือเริ่มขึ้น → compression เสร็จ
            {
                _goingDown = false;

                if (_peakDepth >= pressThreshold)
                {
                    float now = Time.time;
                    if (now - _lastPressTime >= minCompressionInterval)
                    {
                        if (_lastPressTime > 0f)
                        {
                            float interval = now - _lastPressTime;
                            if (interval < 3f)
                            {
                                _intervals.Add(interval);
                                if (_intervals.Count > 10) _intervals.RemoveAt(0);
                            }
                        }
                        _lastPressTime = now;

                        if (_intervals.Count >= 2)
                        {
                            float sum = 0f;
                            foreach (var t in _intervals) sum += t;
                            compressionRate = 60f / (sum / _intervals.Count);
                        }

                        OnCompression?.Invoke();
                    }
                }
                _peakDepth = 0f;
            }
        }
    }

    void OnHandAbsent()
    {
        compressionDepth01    = Mathf.MoveTowards(compressionDepth01, 0f, Time.deltaTime * 6f);
        handsOnMannequin      = false;
        _goingDown            = false;

        if (_wasOnMannequin && _leaveTime < 0f)
            _leaveTime = Time.time;

        if (_leaveTime > 0f && Time.time - _leaveTime > baselineResetDelay)
            ClearBaseline();
    }

    void ClearBaseline()
    {
        _baselineSet    = false;
        _wasOnMannequin = false;
        _leaveTime      = -1f;
        _intervals.Clear();
        compressionRate = 0f;
        _goingDown      = false;
        _peakDepth      = 0f;
        _prevDepth      = 0f;
        _smoothInit     = false;
    }

    // ────────────────────────────────────────────────────
    //  OnGUI – debug overlay
    // ────────────────────────────────────────────────────
    void OnGUI()
    {
        DrawZone();
        if (showDepthHUD) DrawDepthHUD();
    }

    void DrawZone()
    {
        if (!showDebugZone) return;

        Rect cam = GetCameraScreenRect();
        if (cam.width <= 0) return;

        Rect zone = new Rect(
            cam.x + mannequinZone.x * cam.width,
            cam.y + mannequinZone.y * cam.height,
            mannequinZone.width  * cam.width,
            mannequinZone.height * cam.height
        );

        bool hit    = handsOnMannequin;
        Color fill  = hit ? new Color(0f, 1f, 0f, 0.25f) : new Color(1f, 1f, 0f, 0.12f);
        Color border= hit ? Color.green : Color.yellow;

        GUI.color = fill;
        GUI.DrawTexture(zone, Texture2D.whiteTexture);
        GUI.color = Color.white;
        DrawBorder(zone, 3f, border);
        DrawBorder(cam,  2f, new Color(0.6f, 0.6f, 0.6f, 0.6f));

        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize  = Mathf.RoundToInt(cam.height * 0.07f),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter
        };
        style.normal.textColor = border;
        float lh = style.fontSize + 4;
        GUI.Label(new Rect(zone.x, zone.y - lh, zone.width, lh),
                  hit ? "Hand Detected!" : "Place hands here", style);
    }

    void DrawDepthHUD()
    {
        float sw = Screen.width, sh = Screen.height;
        float bw = sw * 0.22f, bh = sh * 0.026f;

        // ซ้ายล่าง เหนือ CPR HUD strip (14%) — ไม่ซ้อนกับ timer, camera, หรือ BPM
        float bx = sw * 0.01f;
        float by = sh * 0.72f;

        // พื้นหลัง
        GUI.color = new Color(0f, 0f, 0f, 0.72f);
        GUI.DrawTexture(new Rect(bx - 4, by - 4, bw + 8, bh * 3.6f + 8), Texture2D.whiteTexture);
        GUI.color = Color.white;

        var lbl = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(bh * 0.85f) };
        lbl.normal.textColor = Color.white;

        // row 1: ตัวเลข
        GUI.Label(new Rect(bx, by, bw, bh),
            $"Depth: {compressionDepth01:F2}   BPM: {compressionRate:F0}", lbl);

        // row 2: bar track
        float barY = by + bh * 1.15f;
        GUI.color = new Color(0.25f, 0.25f, 0.25f);
        GUI.DrawTexture(new Rect(bx, barY, bw, bh * 0.6f), Texture2D.whiteTexture);

        // bar fill
        float d = Mathf.Clamp01(compressionDepth01);
        Color bc = d >= pressThreshold ? Color.green
                 : d >= releaseThreshold ? Color.yellow : Color.gray;
        GUI.color = bc;
        GUI.DrawTexture(new Rect(bx, barY, bw * d, bh * 0.6f), Texture2D.whiteTexture);

        // threshold markers
        GUI.color = Color.cyan;
        GUI.DrawTexture(new Rect(bx + bw * pressThreshold - 1, barY, 2, bh * 0.6f), Texture2D.whiteTexture);
        GUI.color = Color.yellow;
        GUI.DrawTexture(new Rect(bx + bw * releaseThreshold - 1, barY, 2, bh * 0.6f), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // row 3: legend
        var sm = new GUIStyle(lbl) { fontSize = Mathf.RoundToInt(bh * 0.62f) };
        sm.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        GUI.Label(new Rect(bx, barY + bh * 0.7f, bw, bh),
            $"| cyan=press({pressThreshold:F2})  ylw=release({releaseThreshold:F2})", sm);
    }

    Rect GetCameraScreenRect()
    {
        if (cameraPreview != null)
        {
            Vector3[] corners = new Vector3[4];
            cameraPreview.rectTransform.GetWorldCorners(corners);
            float x = corners[0].x;
            float y = Screen.height - corners[2].y;
            float w = corners[2].x - corners[0].x;
            float h = corners[2].y - corners[0].y;
            return new Rect(x, y, w, h);
        }
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
