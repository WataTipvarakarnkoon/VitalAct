using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CprCycleUI : MonoBehaviour
{
    [Header("References")]
    public CprCycleCounter cycleCounter;
    public CprHandDetector handDetector;

    [Header("Cycle & Compression")]
    public TextMeshProUGUI cycleText;
    public TextMeshProUGUI compressionText;
    public Image           compressionFill;

    [Header("BPM")]
    public TextMeshProUGUI rateText;
    public Image           rateDot;

    [Header("Depth")]
    public Image depthFill;

    [Header("Breath Overlay")]
    public GameObject      breathPanel;
    public TextMeshProUGUI breathTimerText;

    [Header("Style")]
    public Color primaryColor = new Color(0.12f, 0.76f, 0.55f);
    public Color dangerColor  = new Color(0.95f, 0.33f, 0.33f);
    public Color breathColor  = new Color(0.24f, 0.62f, 0.95f);

    void Awake()
    {
        if (cycleCounter == null) cycleCounter = FindObjectOfType<CprCycleCounter>(true);
        if (handDetector == null) handDetector = FindObjectOfType<CprHandDetector>(true);

        if (cycleCounter != null)
        {
            cycleCounter.compressionCountText = null;
            cycleCounter.cycleCountText       = null;
            cycleCounter.breathTimerText      = breathTimerText;
            cycleCounter.breathPanel          = breathPanel;
        }

        if (breathPanel != null) breathPanel.SetActive(false);
    }

    void Update()
    {
        if (cycleCounter != null)
        {
            if (cycleText != null)
                cycleText.text = $"Cycle {cycleCounter.completedCycles + 1}";

            if (compressionText != null)
                compressionText.text = $"{cycleCounter.currentCompression} / {cycleCounter.compressionsPerCycle}";

            if (compressionFill != null)
            {
                float p = cycleCounter.compressionsPerCycle > 0
                    ? (float)cycleCounter.currentCompression / cycleCounter.compressionsPerCycle
                    : 0f;
                compressionFill.fillAmount = Mathf.Clamp01(p);
                compressionFill.color = cycleCounter.isBreathingPhase ? breathColor : primaryColor;
            }
        }

        if (handDetector != null)
        {
            float rate = handDetector.compressionRate;
            if (rateText != null)
                rateText.text = rate > 0f ? $"{rate:F0} BPM" : "— BPM";

            bool good = rate >= 100f && rate <= 120f;
            if (rateDot != null)
                rateDot.color = rate <= 0f ? Color.gray : good ? primaryColor : dangerColor;

            if (depthFill != null)
            {
                float d = handDetector.compressionDepth01;
                depthFill.fillAmount = Mathf.Clamp01(d);
                depthFill.color = d >= handDetector.pressThreshold ? primaryColor
                                : d >= handDetector.releaseThreshold ? Color.yellow
                                : Color.gray;
            }
        }
    }
}
