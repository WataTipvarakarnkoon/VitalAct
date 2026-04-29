using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;

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
    [Tooltip("ค่าน้อย = smooth มาก (ตอบสนองช้า), ค่ามาก = responsive (noise มาก)\nแนะนำ: 0.60-0.70 สำหรับ CPR ที่กดเร็ว")]
    public float smoothAlpha = 0.65f;

    // ──────────────────────────────────────────────────────
    //  COMPRESSION DETECTION
    // ──────────────────────────────────────────────────────
    [Header("Compression Detection")]
    [Tooltip("ระยะ Y relative (wrist-shoulder) ที่ถือว่ากดสุด\nแนะนำ: 0.10-0.20 สำหรับ Pose wrist")]
    public float maxDepthY = 0.15f;

    [Range(0.1f, 0.9f)]
    [Tooltip("กดลงถึงค่านี้ = นับ 1 ครั้ง  |  แนะนำ: 0.45-0.60")]
    public float pressThreshold = 0.50f;

    [Range(0.05f, 0.5f)]
    [Tooltip("ต้องขึ้นมาถึงค่านี้ก่อน ถึงจะกดรอบถัดไปได้  |  ต้องต่ำกว่า pressThreshold  |  แนะนำ: 0.20-0.30")]
    public float releaseThreshold = 0.25f;

    [Tooltip("ระยะเวลาขั้นต่ำระหว่างการกด 2 ครั้ง (วินาที) — ลดให้ต่ำ = กดรัวได้\nแนะนำ: 0.18-0.22")]
    public float minCompressionInterval = 0.20f;

    [Tooltip("เวลาที่มือออกจาก zone ก่อน baseline จะ reset (วินาที)")]
    public float baselineResetDelay = 2.0f;

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
    [Header("Zone UI (ใส่บน Camera Preview)")]
    public UnityEngine.UI.RawImage cameraPreview;
    [Tooltip("Image ที่เป็น child ของ cameraPreview — จะ resize ให้ตรงกับ zone อัตโนมัติ")]
    public RectTransform zoneUI;
    [Tooltip("สีกรอบตอนมือยังไม่อยู่ใน zone")]
    public UnityEngine.UI.Graphic zoneGraphic;
    public Color zoneNormalColor  = new Color(1f, 1f, 0f, 0.35f);
    public Color zoneDetectedColor = new Color(0f, 1f, 0.3f, 0.45f);

    [Header("Debug Overlay (fallback ถ้าไม่มี zoneUI)")]
    public bool showDebugZone = true;
    [Tooltip("แสดง depth bar และค่าตัวเลขบนหน้าจอ")]
    public bool showDepthHUD  = true;

    // ──────────────────────────────────────────────────────
    //  EVENT
    // ──────────────────────────────────────────────────────
    public static event System.Action<float, float> OnCompression; // rate (BPM), peakDepth01

    /// <summary>เรียกตอนเริ่ม cycle ใหม่เพื่อ reset peak detection state</summary>
    public void ResetDetection()
    {
        _waitingForRelease = false;
    }

    // ──────────────────────────────────────────────────────
    //  INTERNAL
    // ──────────────────────────────────────────────────────
    // thread-safe (MediaPipe → main)
    volatile float _pendingRawX;
    volatile float _pendingRawY;   // wrist Y relative to shoulder (increases when pressing down)
    volatile float _pendingWristY; // absolute wrist Y for zone check
    volatile bool  _hasNewData;
    volatile bool  _handVisible;

    // main thread only
    float _smoothY;
    bool  _smoothInit;

    float _baselineY;
    bool  _baselineSet;

    bool  _wasOnMannequin;
    float _leaveTime = -1f;

    // compression state machine
    bool _waitingForRelease = false;   // true = กดแล้ว รอขึ้น | false = ขึ้นแล้ว รอกด

    float _lastPressTime;
    readonly List<float> _intervals = new List<float>();

    // ──────────────────────────────────────────────────────

    void OnEnable()  => PoseLandmarkerRunner.OnPoseLandmarkResult += OnPoseLandmarkResult;
    void OnDisable() => PoseLandmarkerRunner.OnPoseLandmarkResult -= OnPoseLandmarkResult;

    // MediaPipe thread
    void OnPoseLandmarkResult(PoseLandmarkerResult result)
    {
        if (result.poseLandmarks == null || result.poseLandmarks.Count == 0)
        {
            _handVisible = false;
            _hasNewData  = true;
            return;
        }

        var lm = result.poseLandmarks[0].landmarks;
        // Pick wrist with higher visibility
        bool useRight = lm[16].visibility > lm[15].visibility;
        var wrist    = useRight ? lm[16] : lm[15];
        var shoulder = useRight ? lm[12] : lm[11];

        _pendingRawX   = wrist.x;
        _pendingWristY = wrist.y;
        // Depth = wrist Y minus shoulder Y (in-frame signal: increases when pressing down)
        _pendingRawY   = wrist.y - shoulder.y;
        _handVisible   = true;
        _hasNewData    = true;
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

        float rawX      = _pendingRawX;
        float rawY      = _pendingRawY;      // relative depth signal (wrist - shoulder Y)
        float wristAbsY = _pendingWristY;    // absolute wrist Y for zone check

        // ── Exponential Moving Average smoothing on relative depth ──
        if (!_smoothInit) { _smoothY = rawY; _smoothInit = true; }
        _smoothY = _smoothY + smoothAlpha * (rawY - _smoothY);

        // flip X (MediaPipe mirror), zone uses absolute wrist Y
        float zoneX = 1f - rawX;
        // Allow wrist below zone bottom (Y > 1) — common during CPR lean
        bool inZone = zoneX >= mannequinZone.xMin && zoneX <= mannequinZone.xMax &&
                      wristAbsY >= mannequinZone.yMin;

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
        UpdateZoneUI();
    }

    void UpdateZoneUI()
    {
        if (zoneUI == null) return;

        // ตั้ง anchor ให้ครอบตาม mannequinZone
        // MediaPipe Y = top-down → RectTransform Y = bottom-up
        zoneUI.anchorMin = new Vector2(mannequinZone.x,    1f - mannequinZone.yMax);
        zoneUI.anchorMax = new Vector2(mannequinZone.xMax, 1f - mannequinZone.y);
        zoneUI.offsetMin = Vector2.zero;
        zoneUI.offsetMax = Vector2.zero;

        if (zoneGraphic != null)
            zoneGraphic.color = handsOnMannequin ? zoneDetectedColor : zoneNormalColor;
    }

    // ── Peak detection: ตรวจจุดกดสูงสุดตอนมือเริ่มขึ้น ──
    // วิธีนี้จับการกดเร็วได้ดีกว่า threshold crossing
    // ── State machine แบบง่าย: กด → รอขึ้น → กด → รอขึ้น ──
    // ไม่มี delta ไม่มี peak tracking — ไม่เบิ้ล ไม่หาย
    void DetectCompression(float depth)
    {
        if (!_waitingForRelease)
        {
            // รอกด: depth ข้าม pressThreshold → นับทันที
            if (depth >= pressThreshold)
            {
                _waitingForRelease = true;

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

                    if (_intervals.Count >= 1)
                    {
                        float sum = 0f;
                        foreach (var t in _intervals) sum += t;
                        compressionRate = 60f / (sum / _intervals.Count);
                    }

                    OnCompression?.Invoke(compressionRate, depth);
                }
            }
        }
        else
        {
            // รอขึ้น: depth ต่ำกว่า releaseThreshold → พร้อมกดรอบถัดไป
            if (depth < releaseThreshold)
                _waitingForRelease = false;
        }
    }

    void OnHandAbsent()
    {
        compressionDepth01    = Mathf.MoveTowards(compressionDepth01, 0f, Time.deltaTime * 6f);
        handsOnMannequin      = false;
        _waitingForRelease    = false;

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
        _waitingForRelease = false;
        _smoothInit        = false;
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
        // ถ้ามี zoneUI แล้วไม่ต้องวาด OnGUI
        if (!showDebugZone || zoneUI != null) return;

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

        float bx = sw * 0.01f;
        float by = sh * 0.72f;

        // bar track
        GUI.color = new Color(0.25f, 0.25f, 0.25f);
        GUI.DrawTexture(new Rect(bx, by, bw, bh * 0.6f), Texture2D.whiteTexture);

        // bar fill
        float d = Mathf.Clamp01(compressionDepth01);
        Color bc = d >= pressThreshold ? Color.green
                 : d >= releaseThreshold ? Color.yellow : Color.gray;
        GUI.color = bc;
        GUI.DrawTexture(new Rect(bx, by, bw * d, bh * 0.6f), Texture2D.whiteTexture);

        GUI.color = Color.white;
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
