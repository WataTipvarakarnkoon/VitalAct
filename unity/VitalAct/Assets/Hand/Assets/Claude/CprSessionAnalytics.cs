using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// เก็บข้อมูลละเอียดทุก compression แล้วประมวลผลตอนจบ session
/// Attach ที่ CPR_Manager GameObject เดียวกับ CprGameManager
/// </summary>
public class CprSessionAnalytics : MonoBehaviour
{
    [Header("References")]
    public CprGameManager  gameManager;
    public CprHandDetector handDetector;

    // ---- internal tracking ----
    float _sessionStart;
    float _handOnTime;        // วินาทีที่มืออยู่บนหุ่น
    bool  _wasHandOn;

    CprSessionData _current;

    // ────────────────────────────────────────

    void Awake()
    {
        if (gameManager  == null) gameManager  = FindObjectOfType<CprGameManager>();
        if (handDetector == null) handDetector = FindObjectOfType<CprHandDetector>();
    }

    void OnEnable()
    {
        CprGameManager.OnStateChanged    += OnStateChanged;
        CprHandDetector.OnCompression    += OnCompression;
    }

    void OnDisable()
    {
        CprGameManager.OnStateChanged    -= OnStateChanged;
        CprHandDetector.OnCompression    -= OnCompression;
    }

    void OnStateChanged(CprGameState state)
    {
        if (state == CprGameState.Playing)
        {
            _sessionStart = Time.time;
            _handOnTime   = 0f;
            _wasHandOn    = false;
            _current      = new CprSessionData();
        }
        else if (state == CprGameState.Results)
        {
            FinalizeAndInject();
        }
    }

    void Update()
    {
        if (gameManager == null || gameManager.State != CprGameState.Playing) return;
        if (handDetector == null) return;

        // นับเวลาที่มืออยู่บนหุ่น
        bool handOn = handDetector.handsOnMannequin;
        if (handOn) _handOnTime += Time.deltaTime;
        _wasHandOn = handOn;
    }

    void OnCompression()
    {
        if (gameManager == null || gameManager.State != CprGameState.Playing) return;
        if (_current == null || handDetector == null) return;

        _current.compressions.Add(new CompressionRecord
        {
            time  = Time.time - _sessionStart,
            rate  = handDetector.compressionRate,
            depth = handDetector.compressionDepth01
        });
    }

    void FinalizeAndInject()
    {
        if (_current == null || _current.compressions.Count == 0) return;

        var recs = _current.compressions;

        // ---- avg rate ----
        float rateSum = 0f;
        float depthSum = 0f;
        float peak = 0f;

        foreach (var r in recs)
        {
            rateSum  += r.rate;
            depthSum += r.depth;
            if (r.rate > peak) peak = r.rate;
        }

        _current.avgRate    = rateSum  / recs.Count;
        _current.avgDepth01 = depthSum / recs.Count;
        _current.peakRate   = peak;

        // ---- rate consistency (ยิ่ง std dev น้อย ยิ่งสม่ำเสมอ) ----
        float variance = 0f;
        foreach (var r in recs)
        {
            float d = r.rate - _current.avgRate;
            variance += d * d;
        }
        float stdDev = Mathf.Sqrt(variance / recs.Count);
        // แปลง std dev → consistency 0-100 (stdDev=0 → 100, stdDev=30 → 0)
        _current.rateConsistency = Mathf.Clamp01(1f - stdDev / 30f) * 100f;

        // ---- hand on time ----
        float elapsed = Time.time - _sessionStart;
        _current.handOnTimePercent = elapsed > 0f
            ? Mathf.Clamp01(_handOnTime / elapsed) * 100f : 0f;

        // inject ลงใน session data ที่ CprGameManager สร้าง
        // (ค้นหา CprSessionData จากที่ส่ง event ล่าสุดไม่ได้ → subscribe เพิ่มแทน)
        CprGameManager.OnSessionComplete += InjectOnce;
    }

    void InjectOnce(CprSessionData data)
    {
        CprGameManager.OnSessionComplete -= InjectOnce;
        if (data == null || _current == null) return;

        data.avgRate            = _current.avgRate;
        data.rateConsistency    = _current.rateConsistency;
        data.avgDepth01         = _current.avgDepth01;
        data.peakRate           = _current.peakRate;
        data.handOnTimePercent  = _current.handOnTimePercent;
        data.compressions       = _current.compressions;

        Debug.Log($"[Analytics] Avg:{data.avgRate:F1} BPM | Consistency:{data.rateConsistency:F0}% | AvgDepth:{data.avgDepth01:F2} | HandOn:{data.handOnTimePercent:F0}%");
    }
}
