using UnityEngine;

public enum CprGameState { Idle, Countdown, Playing, Results }

/// <summary>
/// State machine หลักของเกม CPR
/// Idle → Countdown → Playing → Results → Idle
/// </summary>
public class CprGameManager : MonoBehaviour
{
    [Header("References")]
    public CprHandDetector  handDetector;
    public CprCycleCounter  cycleCounter;

    [Header("Settings")]
    public float sessionDuration  = 120f;  // วินาที (2 นาที)
    public int   countdownSeconds = 3;
    [Tooltip("เริ่ม countdown ทันทีตอน scene โหลด")]
    public bool  autoStartOnLoad = true;

    // ---- events ----
    public static event System.Action<CprGameState> OnStateChanged;
    public static event System.Action<CprSessionData> OnSessionComplete;

    // ---- state ----
    public CprGameState State { get; private set; } = CprGameState.Idle;

    // ---- timers ----
    public float SessionTimeLeft  { get; private set; }
    public int   CountdownValue   { get; private set; }

    // ---- stats ----
    CprSessionData _data;

    // ────────────────────────────────────────

    void OnEnable()  => CprHandDetector.OnCompression += OnCompression;
    void OnDisable() => CprHandDetector.OnCompression -= OnCompression;

    void Start()
    {
        if (autoStartOnLoad) StartGame();
    }

    public void StartGame()
    {
        if (State != CprGameState.Idle) return;
        SetState(CprGameState.Countdown);
        CountdownValue = countdownSeconds;
        InvokeRepeating(nameof(TickCountdown), 0f, 1f);
    }

    void TickCountdown()
    {
        CountdownValue--;
        if (CountdownValue <= 0)
        {
            CancelInvoke(nameof(TickCountdown));
            BeginPlaying();
        }
    }

    void BeginPlaying()
    {
        _data = new CprSessionData();
        SessionTimeLeft = sessionDuration;
        cycleCounter?.ResetAll();
        SetState(CprGameState.Playing);
    }

    void Update()
    {
        if (State != CprGameState.Playing) return;

        SessionTimeLeft -= Time.deltaTime;
        if (SessionTimeLeft <= 0f) EndSession();
    }

    void OnCompression()
    {
        if (State != CprGameState.Playing || _data == null) return;

        _data.totalCompressions++;

        float rate  = handDetector != null ? handDetector.compressionRate  : 0f;
        float depth = handDetector != null ? handDetector.compressionDepth01 : 0f;

        if (rate >= 100f && rate <= 120f) _data.goodRateCompressions++;
        if (depth >= 0.5f)               _data.goodDepthCompressions++;
    }

    void EndSession()
    {
        if (_data == null) return;
        _data.completedCycles   = cycleCounter != null ? cycleCounter.completedCycles : 0;
        _data.sessionDuration   = sessionDuration - SessionTimeLeft;
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
