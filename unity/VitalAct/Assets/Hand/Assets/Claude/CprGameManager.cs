using UnityEngine;

public enum CprGameState { Idle, Playing, Results }

public class CprGameManager : MonoBehaviour
{
    [Header("References")]
    public CprHandDetector handDetector;
    public CprCycleCounter cycleCounter;

    [Header("Settings")]
    public float sessionDuration = 120f;

    public static event System.Action<CprGameState>   OnStateChanged;
    public static event System.Action<CprSessionData> OnSessionComplete;

    public CprGameState    State           { get; private set; } = CprGameState.Idle;
    public CprSessionData  LastSessionData => _data;

    CprSessionData _data;

    void OnEnable()  => CprHandDetector.OnCompression += OnCompression;
    void OnDisable() => CprHandDetector.OnCompression -= OnCompression;

    void Start()
    {
        if (FindObjectOfType<CprSessionAnalytics>() == null) gameObject.AddComponent<CprSessionAnalytics>();

        BeginPlaying();
    }

    void BeginPlaying()
    {
        _data = new CprSessionData();
        cycleCounter?.ResetAll();
        SetState(CprGameState.Playing);
    }

    void OnCompression()
    {
        if (State != CprGameState.Playing || _data == null) return;

        _data.totalCompressions++;

        float rate  = handDetector != null ? handDetector.compressionRate    : 0f;
        float depth = handDetector != null ? handDetector.compressionDepth01 : 0f;

        if (rate >= 100f && rate <= 120f) _data.goodRateCompressions++;
        if (depth >= 0.5f)               _data.goodDepthCompressions++;
    }

    public void EndSession()
    {
        if (State != CprGameState.Playing || _data == null) return;
        _data.completedCycles = cycleCounter != null ? cycleCounter.completedCycles : 0;
        _data.sessionDuration = sessionDuration;
        SetState(CprGameState.Results);
        OnSessionComplete?.Invoke(_data);
    }

    public void ReturnToIdle()
    {
        cycleCounter?.ResetAll();
        SetState(CprGameState.Idle);
    }

    void SetState(CprGameState next)
    {
        State = next;
        OnStateChanged?.Invoke(next);
    }
}
