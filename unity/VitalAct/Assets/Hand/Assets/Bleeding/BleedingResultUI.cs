using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BleedingResultUI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private BleedingSystem bleedingSystem;
    [SerializeField] private Canvas targetCanvas;

    [Header("Results Panel")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button retryButton;
    [SerializeField] private float fadeDuration = 0.35f;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI overallText;
    [SerializeField] private TextMeshProUGUI pressureAccuracyText;
    [SerializeField] private TextMeshProUGUI stabilityText;
    [SerializeField] private TextMeshProUGUI averageDepthText;
    [SerializeField] private TextMeshProUGUI maxDepthText;
    [SerializeField] private TextMeshProUGUI totalTimeText;
    [SerializeField] private TextMeshProUGUI pressTimeText;
    [SerializeField] private TextMeshProUGUI surgesText;
    [SerializeField] private TextMeshProUGUI decisionsText;

    private bool _shown;

    private void Awake()
    {
        if (targetCanvas == null)
        {
            targetCanvas = GetComponentInParent<Canvas>();
        }

        EnsurePanel();

        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (resultsPanel != null)
        {
            resultsPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (bleedingSystem != null)
        {
            bleedingSystem.SessionCompleted += OnSessionCompleted;
        }
    }

    private void OnDisable()
    {
        if (bleedingSystem != null)
        {
            bleedingSystem.SessionCompleted -= OnSessionCompleted;
        }
    }

    private void OnSessionCompleted(BleedingSessionData data)
    {
        if (_shown || data == null)
        {
            return;
        }

        _shown = true;
        FillResults(data);
        StartCoroutine(ShowResults());
    }

    private IEnumerator ShowResults()
    {
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
        }

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            }
            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void FillResults(BleedingSessionData data)
    {
        if (gradeText != null) gradeText.text = data.Grade;
        if (overallText != null) overallText.text = $"Overall {data.ControlScore:F0}%";
        if (pressureAccuracyText != null) pressureAccuracyText.text = $"Pressure Accuracy: {data.PressureAccuracy:F0}%";
        if (stabilityText != null) stabilityText.text = $"Stability: {data.StabilityPercent:F0}%";
        if (averageDepthText != null) averageDepthText.text = $"Average Depth: {data.averageDepth * 100f:F0}%";
        if (maxDepthText != null) maxDepthText.text = $"Peak Depth: {data.maxDepth * 100f:F0}%";
        if (totalTimeText != null) totalTimeText.text = $"Total Time: {data.totalDuration:F1}s";
        if (pressTimeText != null) pressTimeText.text = $"Pressure Time: {data.totalPressTime:F1}s";
        if (surgesText != null) surgesText.text = $"Surges Managed: {data.survivedSurgeCount}";
        if (decisionsText != null) decisionsText.text = $"Checklist: {(data.checklistCompleted ? "Passed" : "Missed")}   Initial Choice: {(data.initialChoiceCorrect ? "Correct" : "Incorrect")}   Reassess: {(data.reassessChoiceCorrect ? "Correct" : "Incorrect")}";
    }

    private void EnsurePanel()
    {
        if (resultsPanel != null)
        {
            return;
        }

        if (targetCanvas == null)
        {
            return;
        }

        Sprite sprite = null;
        var font = TMP_Settings.defaultFontAsset;

        GameObject panel = new GameObject("BleedingResultsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
        panel.transform.SetParent(targetCanvas.transform, false);
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.18f, 0.14f);
        panelRect.anchorMax = new Vector2(0.82f, 0.86f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        var panelImage = panel.GetComponent<Image>();
        panelImage.sprite = sprite;
        panelImage.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        panelImage.color = new Color(0.09f, 0.11f, 0.19f, 0.96f);

        canvasGroup = panel.GetComponent<CanvasGroup>();
        resultsPanel = panel;

        gradeText = CreateText("GradeText", panel.transform, font, 52, FontStyles.Bold, Color.white, "A");
        SetRect(gradeText.rectTransform, new Vector2(0.08f, 0.76f), new Vector2(0.28f, 0.92f));
        gradeText.alignment = TextAlignmentOptions.Center;

        overallText = CreateText("OverallText", panel.transform, font, 28, FontStyles.Bold, new Color(1f, 0.9f, 0.2f), "Overall 95%");
        SetRect(overallText.rectTransform, new Vector2(0.28f, 0.8f), new Vector2(0.92f, 0.9f));

        pressureAccuracyText = CreateText("PressureAccuracyText", panel.transform, font, 24, FontStyles.Normal, Color.white, "");
        SetRect(pressureAccuracyText.rectTransform, new Vector2(0.1f, 0.58f), new Vector2(0.9f, 0.68f));

        stabilityText = CreateText("StabilityText", panel.transform, font, 24, FontStyles.Normal, Color.white, "");
        SetRect(stabilityText.rectTransform, new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.58f));

        averageDepthText = CreateText("AverageDepthText", panel.transform, font, 24, FontStyles.Normal, Color.white, "");
        SetRect(averageDepthText.rectTransform, new Vector2(0.1f, 0.38f), new Vector2(0.9f, 0.48f));

        maxDepthText = CreateText("MaxDepthText", panel.transform, font, 24, FontStyles.Normal, Color.white, "");
        SetRect(maxDepthText.rectTransform, new Vector2(0.1f, 0.32f), new Vector2(0.9f, 0.42f));

        surgesText = CreateText("SurgesText", panel.transform, font, 24, FontStyles.Normal, Color.white, "");
        SetRect(surgesText.rectTransform, new Vector2(0.1f, 0.22f), new Vector2(0.9f, 0.32f));

        totalTimeText = CreateText("TotalTimeText", panel.transform, font, 24, FontStyles.Normal, Color.white, "");
        SetRect(totalTimeText.rectTransform, new Vector2(0.1f, 0.14f), new Vector2(0.9f, 0.24f));

        pressTimeText = CreateText("PressTimeText", panel.transform, font, 24, FontStyles.Normal, Color.white, "");
        SetRect(pressTimeText.rectTransform, new Vector2(0.1f, 0.06f), new Vector2(0.9f, 0.16f));

        decisionsText = CreateText("DecisionsText", panel.transform, font, 18, FontStyles.Normal, new Color(0.85f, 0.9f, 1f), "");
        SetRect(decisionsText.rectTransform, new Vector2(0.08f, 0f), new Vector2(0.94f, 0.08f));

        GameObject buttonObject = new GameObject("RetryButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(panel.transform, false);
        var buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.65f, 0.03f);
        buttonRect.anchorMax = new Vector2(0.9f, 0.12f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;

        var buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.sprite = sprite;
        buttonImage.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        buttonImage.color = new Color(0.82f, 0.18f, 0.2f, 1f);
        retryButton = buttonObject.GetComponent<Button>();

        var retryLabel = CreateText("RetryLabel", buttonObject.transform, font, 24, FontStyles.Bold, Color.white, "Retry");
        SetRect(retryLabel.rectTransform, Vector2.zero, Vector2.one);
        retryLabel.alignment = TextAlignmentOptions.Center;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, TMP_FontAsset font, float size, FontStyles style, Color color, string value)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var text = go.GetComponent<TextMeshProUGUI>();
        text.font = font;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.text = value;
        return text;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
