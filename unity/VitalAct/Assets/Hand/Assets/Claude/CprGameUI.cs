using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CprGameUI : MonoBehaviour
{
    [Header("References")]
    public CprGameManager gameManager;

    [Header("Results Panel")]
    public GameObject resultsPanel;
    public Button     retryButton;

    [Header("Results - Header")]
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI overallText;

    [Header("Results - Accuracy Row")]
    public TextMeshProUGUI rateAccuracyText;
    public TextMeshProUGUI depthAccuracyText;
    public TextMeshProUGUI rhythmConsistText;

    [Header("Results - Stats")]
    public TextMeshProUGUI avgRateText;
    public TextMeshProUGUI peakRateText;
    public TextMeshProUGUI handOnText;
    public TextMeshProUGUI avgDepthText;
    public TextMeshProUGUI compressionText;
    public TextMeshProUGUI cyclesText;

    void Awake()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<CprGameManager>();

        if (retryButton != null)
            retryButton.onClick.AddListener(() => gameManager?.ReturnToIdle());

        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    void OnEnable()  => CprGameManager.OnSessionComplete += OnSessionComplete;
    void OnDisable() => CprGameManager.OnSessionComplete -= OnSessionComplete;

    void OnSessionComplete(CprSessionData data)
    {
        if (resultsPanel != null) resultsPanel.SetActive(true);
        FillResults(data);
    }

    void FillResults(CprSessionData d)
    {
        if (d == null) return;

        if (gradeText   != null) gradeText  .text = d.Grade;
        if (overallText != null) overallText.text = $"Overall {d.OverallScore:F0}%";

        if (rateAccuracyText  != null) rateAccuracyText .text = $"{d.RateAccuracy:F0}%";
        if (depthAccuracyText != null) depthAccuracyText.text = $"{d.DepthAccuracy:F0}%";
        if (rhythmConsistText != null) rhythmConsistText.text = $"{d.rateConsistency:F0}%";

        if (avgRateText     != null) avgRateText    .text = $"{d.avgRate:F0} BPM";
        if (peakRateText    != null) peakRateText   .text = $"{d.peakRate:F0} BPM";
        if (handOnText      != null) handOnText     .text = $"{d.handOnTimePercent:F0}%";
        if (avgDepthText    != null) avgDepthText   .text = $"{d.avgDepth01 * 100f:F0}%";
        if (compressionText != null) compressionText.text = d.totalCompressions.ToString();
        if (cyclesText      != null) cyclesText     .text = d.completedCycles.ToString();
    }
}
