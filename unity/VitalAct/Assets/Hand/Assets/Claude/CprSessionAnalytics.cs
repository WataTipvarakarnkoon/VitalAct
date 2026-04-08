using System.Collections.Generic;
using UnityEngine;

public class CprSessionAnalytics : MonoBehaviour
{
    [Header("References")]
    public CprHandDetector handDetector;

    float _sessionStart;
    float _handOnTime;
    CprSessionData _current;

    void Awake()
    {
        if (handDetector == null) handDetector = FindObjectOfType<CprHandDetector>();
    }

    void OnEnable()
    {
        GameManager.OnStateChanged    += OnStateChanged;
        CprHandDetector.OnCompression += OnCompression;
    }

    void OnDisable()
    {
        GameManager.OnStateChanged    -= OnStateChanged;
        CprHandDetector.OnCompression -= OnCompression;
    }

    void OnStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Do)
        {
            _sessionStart = Time.time;
            _handOnTime   = 0f;
            _current      = new CprSessionData();
        }
        else if (state == GameManager.GameState.Result)
        {
            FinalizeAndInject();
        }
    }

    void Update()
    {
        if (GameManager.instance == null || GameManager.instance.CurrentState != GameManager.GameState.Do) return;
        if (handDetector != null && handDetector.handsOnMannequin)
            _handOnTime += Time.deltaTime;
    }

    void OnCompression()
    {
        if (GameManager.instance == null || GameManager.instance.CurrentState != GameManager.GameState.Do) return;
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

        float rateSum = 0f, depthSum = 0f, peak = 0f;
        foreach (var r in recs)
        {
            rateSum  += r.rate;
            depthSum += r.depth;
            if (r.rate > peak) peak = r.rate;
        }

        _current.avgRate    = rateSum  / recs.Count;
        _current.avgDepth01 = depthSum / recs.Count;
        _current.peakRate   = peak;

        float variance = 0f;
        foreach (var r in recs)
        {
            float d = r.rate - _current.avgRate;
            variance += d * d;
        }
        float stdDev = Mathf.Sqrt(variance / recs.Count);
        _current.rateConsistency = Mathf.Clamp01(1f - stdDev / 30f) * 100f;

        float elapsed = Time.time - _sessionStart;
        _current.handOnTimePercent = elapsed > 0f
            ? Mathf.Clamp01(_handOnTime / elapsed) * 100f : 0f;

        GameManager.OnSessionComplete += InjectOnce;
    }

    void InjectOnce(CprSessionData data)
    {
        GameManager.OnSessionComplete -= InjectOnce;
        if (data == null || _current == null) return;

        data.avgRate           = _current.avgRate;
        data.rateConsistency   = _current.rateConsistency;
        data.avgDepth01        = _current.avgDepth01;
        data.peakRate          = _current.peakRate;
        data.handOnTimePercent = _current.handOnTimePercent;
        data.compressions      = _current.compressions;
    }
}
