using UnityEngine;
using UnityEngine.UI;

public class CprGameUI : MonoBehaviour
{
    [Header("References")]
    public CprGameManager gameManager;

    [Header("Font (ต้องใส่ font ที่รองรับภาษาไทย)")]
    public Font thaiFont;

    [Header("Style")]
    public Color accentGreen = new Color(0.12f, 0.76f, 0.55f);
    public Color accentRed   = new Color(0.95f, 0.33f, 0.33f);

    Canvas     _canvas;
    GameObject _countdownPanel;
    Text       _countdownText;
    GameObject _hudPanel;
    Text       _timerText;
    GameObject _resultsPanel;
    Text       _resultsBody;

    bool           _resultsFilled;
    CprGameState   _prevState = CprGameState.Idle;

    // ────────────────────────────────────────

    void Awake()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<CprGameManager>();

        CreateCanvas();
        CreateCountdownPanel();
        CreateHudPanel();
        CreateResultsPanel();

        _countdownPanel.SetActive(false);
        _hudPanel      .SetActive(false);
        _resultsPanel  .SetActive(false);
    }

    void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<CprGameManager>();
            return;
        }

        var state = gameManager.State;

        // ── sync panels ทุก frame (ไม่พึ่ง event) ──
        if (_countdownPanel != null) _countdownPanel.SetActive(state == CprGameState.Countdown);
        if (_hudPanel       != null) _hudPanel      .SetActive(state == CprGameState.Playing);
        if (_resultsPanel   != null) _resultsPanel  .SetActive(state == CprGameState.Results);

        // ── reset ตอนเริ่ม session ใหม่ ──
        if (state == CprGameState.Playing && _prevState != CprGameState.Playing)
            _resultsFilled = false;

        // ── fill results ทันทีที่เข้า Results state ──
        if (state == CprGameState.Results && !_resultsFilled)
        {
            var data = gameManager.LastSessionData;
            if (data != null)
            {
                FillResults(data);
                _resultsFilled = true;
            }
        }

        // ── countdown number ──
        if (state == CprGameState.Countdown && _countdownText != null)
        {
            int v = gameManager.CountdownValue;
            _countdownText.text = v > 0 ? v.ToString() : "GO!";
        }

        // ── HUD timer ──
        if (state == CprGameState.Playing && _timerText != null)
        {
            float t = Mathf.Max(0f, gameManager.SessionTimeLeft);
            int m = (int)(t / 60f), s = (int)(t % 60f);
            _timerText.text  = $"{m}:{s:D2}";
            _timerText.color = t <= 30f ? accentRed : Color.white;
        }

        _prevState = state;
    }

    // ────────── fill results ──────────

    void FillResults(CprSessionData d)
    {
        if (d == null || _resultsBody == null) return;

        int min = (int)(d.sessionDuration / 60f);
        int sec = (int)(d.sessionDuration % 60f);

        _resultsBody.text =
            $"Grade: {d.Grade}     Overall: {d.OverallScore:F0}%\n" +
            $"─────────────────────────────\n" +
            $"Rate Accuracy:    {d.RateAccuracy:F0}%   {Bar(d.RateAccuracy)}\n" +
            $"Depth Accuracy:   {d.DepthAccuracy:F0}%   {Bar(d.DepthAccuracy)}\n" +
            $"Rhythm Consist.:  {d.rateConsistency:F0}%   {Bar(d.rateConsistency)}\n" +
            $"─────────────────────────────\n" +
            $"Avg Rate:   {d.avgRate:F0} BPM   Peak: {d.peakRate:F0} BPM\n" +
            $"Avg Depth:  {d.avgDepth01 * 100f:F0}%\n" +
            $"Hand On:    {d.handOnTimePercent:F0}% of session\n" +
            $"Compressions: {d.totalCompressions}   Cycles: {d.completedCycles}\n" +
            $"Time: {min}:{sec:D2}\n" +
            $"─────────────────────────────\n" +
            GetTip(d);

        _resultsBody.color = Color.white;
    }

    static string Bar(float pct)
    {
        int n = Mathf.RoundToInt(pct / 10f);
        return new string('|', n) + new string('.', 10 - n);
    }

    static string GetTip(CprSessionData d)
    {
        if (d.RateAccuracy  < 60f) return "Tip: Aim for 100-120 compressions/min";
        if (d.DepthAccuracy < 60f) return "Tip: Press deeper (at least 5 cm)";
        if (d.OverallScore >= 90f) return "Excellent work!";
        return "Keep practicing!";
    }

    Color GradeColor(string g)
    {
        if (g == "A") return accentGreen;
        if (g == "B") return new Color(0.4f, 0.9f, 0.4f);
        if (g == "C") return new Color(1f, 0.85f, 0.2f);
        if (g == "D") return new Color(1f, 0.55f, 0.1f);
        return accentRed;
    }

    // ────────── builders ──────────

    void CreateCanvas()
    {
        var go = new GameObject("GameUI_Canvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 999;
        var cs = go.AddComponent<CanvasScaler>();
        cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        go.AddComponent<GraphicRaycaster>();
    }

    void CreateCountdownPanel()
    {
        _countdownPanel = Panel(_canvas.transform, "Countdown",
            new Vector2(0.3f, 0.25f), new Vector2(0.7f, 0.75f),
            new Color(0f, 0f, 0f, 0.85f));

        Txt(_countdownPanel.transform, "Ready",
            new Vector2(0f, 0.65f), Vector2.one,
            "Get Ready!", 56, true, Color.white);

        _countdownText = Txt(_countdownPanel.transform, "Num",
            new Vector2(0f, 0.1f), new Vector2(1f, 0.7f),
            "3", 180, true, accentGreen);
    }

    void CreateHudPanel()
    {
        _hudPanel = Panel(_canvas.transform, "HUD",
            new Vector2(0f, 0.92f), new Vector2(0.13f, 1f),
            new Color(0f, 0f, 0f, 0.6f));

        _timerText = Txt(_hudPanel.transform, "Timer",
            Vector2.zero, Vector2.one,
            "2:00", 44, true, Color.white);
    }

    void CreateResultsPanel()
    {
        // พื้นหลังทึบเต็มจอ
        _resultsPanel = Panel(_canvas.transform, "Results",
            Vector2.zero, Vector2.one,
            new Color(0.05f, 0.05f, 0.10f, 1f));   // alpha=1 ทึบสนิท

        // header
        Txt(_resultsPanel.transform, "Header",
            new Vector2(0f, 0.88f), Vector2.one,
            "CPR Result", 70, true, Color.white);

        // body text เดียว แสดงข้อมูลทั้งหมด
        _resultsBody = Txt(_resultsPanel.transform, "Body",
            new Vector2(0.15f, 0.28f), new Vector2(0.85f, 0.87f),
            "—", 38, false, Color.white, TextAnchor.UpperLeft);

        if (thaiFont != null) _resultsBody.font = thaiFont;

        // retry button
        var btn = Panel(_resultsPanel.transform, "Retry",
            new Vector2(0.35f, 0.05f), new Vector2(0.65f, 0.14f),
            accentGreen);
        btn.AddComponent<Button>().onClick.AddListener(() => gameManager?.ReturnToIdle());
        Txt(btn.transform, "lbl", Vector2.zero, Vector2.one,
            "Try Again", 48, true, Color.white);
    }

    // ────────── helpers ──────────

    Text Txt(Transform parent, string name,
        Vector2 aMin, Vector2 aMax, string text,
        int size, bool bold, Color color,
        TextAnchor align = TextAnchor.MiddleCenter)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var t = go.GetComponent<Text>();
        t.text      = text;
        t.fontSize  = size;
        t.color     = color;
        t.alignment = align;
        t.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        // ต้อง assign font เสมอ ไม่งั้น dynamic Text จะไม่ render
        t.font = thaiFont != null
            ? thaiFont
            : Resources.GetBuiltinResource<Font>("Arial.ttf");
        return t;
    }

    static GameObject Panel(Transform parent, string name,
        Vector2 aMin, Vector2 aMax, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        go.GetComponent<Image>().color = color;
        return go;
    }
}
