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
    public CprSessionData LastSessionData => _data;

    // ---- OnGUI results ----
    GUIStyle _boxStyle;
    GUIStyle _titleStyle;
    GUIStyle _bodyStyle;
    GUIStyle _btnStyle;
    bool     _stylesBuilt;

    // ────────────────────────────────────────

    void OnEnable()  => CprHandDetector.OnCompression += OnCompression;
    void OnDisable() => CprHandDetector.OnCompression -= OnCompression;

    void Start()
    {
        // auto-add analytics & timer bridge ถ้ายังไม่มี
        if (FindObjectOfType<CprSessionAnalytics>() == null) gameObject.AddComponent<CprSessionAnalytics>();
        if (FindObjectOfType<CprTimerBridge>()      == null) gameObject.AddComponent<CprTimerBridge>();

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

    // ── built-in Results overlay (ไม่พึ่ง Canvas/Component ใด) ──
    void OnGUI()
    {
        if (State == CprGameState.Countdown)
        {
            BuildStyles();
            float sw = Screen.width, sh = Screen.height;
            GUI.Box(new Rect(sw*0.3f, sh*0.25f, sw*0.4f, sh*0.5f), "", _boxStyle);
            GUI.Label(new Rect(sw*0.3f, sh*0.28f, sw*0.4f, sh*0.12f), "Get Ready!", _titleStyle);
            string cd = CountdownValue > 0 ? CountdownValue.ToString() : "GO!";
            GUI.Label(new Rect(sw*0.3f, sh*0.38f, sw*0.4f, sh*0.28f), cd, _titleStyle);
            return;
        }

        if (State != CprGameState.Results) return;
        BuildStyles();

        float W = Screen.width, H = Screen.height;

        // พื้นหลังทึบ
        GUI.Box(new Rect(0, 0, W, H), "", _boxStyle);

        // header
        GUI.Label(new Rect(0, H*0.04f, W, H*0.1f), "CPR Result", _titleStyle);

        if (_data != null)
        {
            int min = (int)(_data.sessionDuration / 60f);
            int sec = (int)(_data.sessionDuration % 60f);

            string body =
                $"Grade: {_data.Grade}          Overall: {_data.OverallScore:F0}%\n" +
                $"----------------------------------------------\n" +
                $"Rate Accuracy:    {_data.RateAccuracy:F0}%\n" +
                $"Depth Accuracy:   {_data.DepthAccuracy:F0}%\n" +
                $"Rhythm Consist.:  {_data.rateConsistency:F0}%\n" +
                $"----------------------------------------------\n" +
                $"Avg Rate:   {_data.avgRate:F0} BPM      Peak: {_data.peakRate:F0} BPM\n" +
                $"Avg Depth:  {_data.avgDepth01 * 100f:F0}%\n" +
                $"Hand On:    {_data.handOnTimePercent:F0}% of session\n" +
                $"Compressions: {_data.totalCompressions}      Cycles: {_data.completedCycles}\n" +
                $"Time: {min}:{sec:D2}";

            GUI.Label(new Rect(W*0.15f, H*0.16f, W*0.7f, H*0.7f), body, _bodyStyle);
        }

        // Try Again button
        if (GUI.Button(new Rect(W*0.35f, H*0.84f, W*0.3f, H*0.09f), "Try Again", _btnStyle))
            ReturnToIdle();
    }

    void BuildStyles()
    {
        if (_stylesBuilt) return;
        _stylesBuilt = true;

        _boxStyle = new GUIStyle(GUI.skin.box);
        _boxStyle.normal.background = MakeTex(1, 1, new Color(0.04f, 0.04f, 0.10f, 0.97f));

        _titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = Mathf.RoundToInt(Screen.height * 0.07f),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
        };
        _titleStyle.normal.textColor = Color.white;

        _bodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = Mathf.RoundToInt(Screen.height * 0.033f),
            fontStyle = FontStyle.Normal,
            wordWrap  = true,
        };
        _bodyStyle.normal.textColor = Color.white;

        _btnStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize  = Mathf.RoundToInt(Screen.height * 0.045f),
            fontStyle = FontStyle.Bold,
        };
        _btnStyle.normal.textColor = Color.white;
        _btnStyle.normal.background = MakeTex(1, 1, new Color(0.12f, 0.76f, 0.55f, 1f));
    }

    static Texture2D MakeTex(int w, int h, Color col)
    {
        var tex = new Texture2D(w, h);
        var pix = new Color[w * h];
        for (int i = 0; i < pix.Length; i++) pix[i] = col;
        tex.SetPixels(pix);
        tex.Apply();
        return tex;
    }
}
