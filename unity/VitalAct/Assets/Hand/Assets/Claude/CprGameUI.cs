using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CprGameUI : MonoBehaviour
{
    public CprTimerFill timerFill;
    
    [Header("Results Panel")]
    public GameObject  resultsPanel;
    public CanvasGroup canvasGroup;
    public Button      retryButton;
    public float       fadeDuration = 0.5f;

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
        if (retryButton != null)
            retryButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));

        if (canvasGroup != null)
        {
            canvasGroup.alpha          = 0f;
            canvasGroup.interactable   = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    bool _shown;

    void Update()
    {
        if (_shown) return;
        if (timerFill == null || timerFill.timer > 0f) return;

        _shown = true;
        StartCoroutine(ShowResults());
    }

    IEnumerator ShowResults()
    {
        yield return null; // wait one frame so EndSession() and analytics have run
        FillResults(GameManager.instance.LastSessionData);
        if (canvasGroup != null) StartCoroutine(FadeIn());
    }

    void FillResults(CprSessionData d)
    {
        if (d == null) return;

        if (gradeText         != null) gradeText        .text = d.Grade;
        if (overallText       != null) overallText       .text = $"Overall {d.OverallScore:F0}%";
        if (rateAccuracyText  != null) rateAccuracyText  .text = $"{d.RateAccuracy:F0}%";
        if (depthAccuracyText != null) depthAccuracyText .text = $"{d.DepthAccuracy:F0}%";
        if (rhythmConsistText != null) rhythmConsistText .text = $"{d.rateConsistency:F0}%";
        if (avgRateText       != null) avgRateText       .text = $"Average Rate: {d.avgRate:F0} BPM";
        if (peakRateText      != null) peakRateText      .text = $"Peak: {d.peakRate:F0} BPM";
        if (handOnText        != null) handOnText        .text = $"Hand On: {d.handOnTimePercent:F0}%";
        if (avgDepthText      != null) avgDepthText      .text = $"Average Dept: {d.avgDepth01 * 100f:F0}%";
        if (compressionText   != null) compressionText   .text = $"Compression: {d.totalCompressions}";
        if (cyclesText        != null) cyclesText        .text = $"Cycles: {d.completedCycles}";
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha          = 1f;
        canvasGroup.interactable   = true;
        canvasGroup.blocksRaycasts = true;
    }
}
