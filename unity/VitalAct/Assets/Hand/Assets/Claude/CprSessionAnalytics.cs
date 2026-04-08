using System.Collections.Generic;
using UnityEngine;

public class CprSessionAnalytics : MonoBehaviour
{
    [Header("References")]
    public CprHandDetector handDetector; // used only for handOnTimePercent tracking

    float _sessionStart;
    float _handOnTime;
    CprSessionData _current;

    void Awake()
    {
        if (handDetector == null && GameManager.instance != null)
            handDetector = GameManager.instance.handDetector;
        if (handDetector == null)
            handDetector = FindObjectOfType<CprHandDetector>();
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
            if (handDetector == null) handDetector = GameManager.instance?.handDetector;
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

    void OnCompression(float rate, float depth)
    {
        if (GameManager.instance == null || GameManager.instance.CurrentState != GameManager.GameState.Do) return;
        if (_current == null) return;
        if (rate <= 0f) return; // skip first compressions before rate is established

        _current.compressions.Add(new CompressionRecord
        {
            time  = Time.time - _sessionStart,
            rate  = rate,
            depth = depth
        });
    }

    void FinalizeAndInject()
    {
        var data = GameManager.instance?.LastSessionData;
        if (data == null || _current == null || _current.compressions.Count == 0) return;

        var recs = _current.compressions;

        float rateSum = 0f, depthSum = 0f, peak = 0f;
        foreach (var r in recs)
        {
            rateSum  += r.rate;
            depthSum += r.depth;
            if (r.rate > peak) peak = r.rate;
        }

        data.avgRate    = rateSum  / recs.Count;
        data.avgDepth01 = depthSum / recs.Count;
        data.peakRate   = peak;

        float variance = 0f;
        foreach (var r in recs)
        {
            float d = r.rate - data.avgRate;
            variance += d * d;
        }
        float stdDev = Mathf.Sqrt(variance / recs.Count);
        data.rateConsistency = Mathf.Clamp01(1f - stdDev / 30f) * 100f;

        float elapsed = Time.time - _sessionStart;
        data.handOnTimePercent = elapsed > 0f
            ? Mathf.Clamp01(_handOnTime / elapsed) * 100f : 0f;

        data.compressions = _current.compressions;
    }
}
