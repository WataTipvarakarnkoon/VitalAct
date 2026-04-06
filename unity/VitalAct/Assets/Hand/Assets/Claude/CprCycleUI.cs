using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD แถบล่างขนาดเล็ก — ไม่บัง 3D scene
/// แสดง: Cycle | Compression count | Progress bar | BPM
/// </summary>
public class CprCycleUI : MonoBehaviour
{
    [Header("References")]
    public CprCycleCounter cycleCounter;
    public CprHandDetector handDetector;

    [Header("Style")]
    public Font  customFont;
    public Color primaryColor  = new Color(0.12f, 0.76f, 0.55f);
    public Color dangerColor   = new Color(0.95f, 0.33f, 0.33f);
    public Color breathColor   = new Color(0.24f, 0.62f, 0.95f);

    Canvas     _canvas;
    Text       _cycleText;
    Text       _compressionText;
    Image      _compressionFill;
    Text       _rateText;
    Image      _rateDot;
    GameObject _breathPanel;
    Text       _breathTimerText;

    // HUD อยู่ด้านล่าง 14% ของหน้าจอ
    const float HUD_Y_MIN = 0f;
    const float HUD_Y_MAX = 0.14f;

    void Awake() => BuildUI();

    void BuildUI()
    {
        var go = new GameObject("CPR_HUD_Canvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 50;
        var cs = go.AddComponent<CanvasScaler>();
        cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        go.AddComponent<GraphicRaycaster>();

        var root = _canvas.transform;

        // ── HUD background ──
        Panel(root, "HudBg",
            new Vector2(0f, HUD_Y_MIN), new Vector2(1f, HUD_Y_MAX),
            new Color(0f, 0f, 0f, 0.72f));

        // ── Cycle label (ซ้าย) ──
        _cycleText = Txt(root, "Cycle",
            new Vector2(0f, HUD_Y_MIN), new Vector2(0.18f, HUD_Y_MAX),
            "Cycle 1", 32, true, Color.white);

        // ── Compression count (กลางซ้าย) ──
        _compressionText = Txt(root, "Count",
            new Vector2(0.18f, HUD_Y_MIN), new Vector2(0.42f, HUD_Y_MAX),
            "0 / 30", 38, true, Color.white);

        // ── Progress bar (กลาง) ──
        var barBg = Panel(root, "BarBg",
            new Vector2(0.42f, HUD_Y_MIN + 0.03f), new Vector2(0.72f, HUD_Y_MAX - 0.03f),
            new Color(0.25f, 0.25f, 0.25f));
        var barFill = Panel(barBg.transform, "Fill",
            Vector2.zero, Vector2.one, primaryColor);
        var fr = barFill.GetComponent<RectTransform>();
        fr.anchorMin = Vector2.zero;
        fr.anchorMax = new Vector2(0f, 1f);
        fr.offsetMin = fr.offsetMax = Vector2.zero;
        _compressionFill = barFill.GetComponent<Image>();

        // ── BPM (ขวา) ──
        // จุดสีเล็กๆ บอก good/bad
        var rateDotGo = Panel(root, "RateDot",
            new Vector2(0.72f, HUD_Y_MIN + 0.02f), new Vector2(0.75f, HUD_Y_MAX - 0.02f),
            Color.gray);
        _rateDot = rateDotGo.GetComponent<Image>();

        _rateText = Txt(root, "Rate",
            new Vector2(0.75f, HUD_Y_MIN), new Vector2(1f, HUD_Y_MAX),
            "— BPM", 32, false, Color.white);

        // ── Breathing overlay (full screen — intentional) ──
        _breathPanel = Panel(root, "BreathPanel",
            Vector2.zero, Vector2.one,
            new Color(0.10f, 0.25f, 0.55f, 0.92f));
        _breathPanel.SetActive(false);

        Txt(_breathPanel.transform, "Title",
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.82f),
            "Breathe x2", 110, true, Color.white);

        _breathTimerText = Txt(_breathPanel.transform, "Timer",
            new Vector2(0.3f, 0.3f), new Vector2(0.7f, 0.56f),
            "5", 140, true, new Color(1f, 0.9f, 0.3f));

        Txt(_breathPanel.transform, "Sub",
            new Vector2(0.1f, 0.18f), new Vector2(0.9f, 0.32f),
            "seconds", 50, false, Color.white);

        // ผูก references เข้า CprCycleCounter
        if (cycleCounter != null)
        {
            cycleCounter.compressionCountText = _compressionText;
            cycleCounter.cycleCountText       = _cycleText;
            cycleCounter.breathTimerText      = _breathTimerText;
            cycleCounter.breathPanel          = _breathPanel;
        }
    }

    void Update()
    {
        if (cycleCounter == null || handDetector == null) return;

        // cycle label
        if (_cycleText != null)
            _cycleText.text = $"Cycle {cycleCounter.completedCycles + 1}";

        // compression count
        if (_compressionText != null)
            _compressionText.text = $"{cycleCounter.currentCompression} / {cycleCounter.compressionsPerCycle}";

        // progress bar
        if (_compressionFill != null)
        {
            float p = (float)cycleCounter.currentCompression / cycleCounter.compressionsPerCycle;
            var rt = _compressionFill.GetComponent<RectTransform>();
            rt.anchorMax = new Vector2(Mathf.Clamp01(p), 1f);
            _compressionFill.color = cycleCounter.isBreathingPhase ? breathColor : primaryColor;
        }

        // BPM
        float rate = handDetector.compressionRate;
        if (_rateText != null)
            _rateText.text = rate > 0f ? $"{rate:F0} BPM" : "— BPM";

        bool good = rate >= 100f && rate <= 120f;
        if (_rateDot != null)
            _rateDot.color = rate <= 0f ? Color.gray : good ? primaryColor : dangerColor;
    }

    // ── helpers ──

    Text Txt(Transform parent, string name,
        Vector2 aMin, Vector2 aMax, string text,
        int size, bool bold, Color color,
        TextAnchor align = TextAnchor.MiddleCenter)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        Anchor(go, aMin, aMax);
        var t = go.GetComponent<Text>();
        t.text      = text;
        t.fontSize  = size;
        t.color     = color;
        t.alignment = align;
        t.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        t.font = customFont != null
            ? customFont
            : Resources.GetBuiltinResource<Font>("Arial.ttf");
        return t;
    }

    static GameObject Panel(Transform parent, string name,
        Vector2 aMin, Vector2 aMax, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Anchor(go, aMin, aMax);
        go.GetComponent<Image>().color = color;
        return go;
    }

    static void Anchor(GameObject go, Vector2 min, Vector2 max)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = min; rt.anchorMax = max;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}
