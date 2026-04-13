using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BleedingUI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private BleedingSystem bleedingSystem;
    [SerializeField] private BleedingHandDetector handDetector;

    [Header("UI")]
    [SerializeField] private Image bleedingFillImage;
    [SerializeField] private Image holdProgressFillImage;
    [SerializeField] private Image depthFillImage;
    [SerializeField] private TMP_Text bleedingStateText;
    [SerializeField] private TMP_Text pressureText;
    [SerializeField] private TMP_Text holdText;
    [SerializeField] private TMP_Text warningText;

    [Header("Colors")]
    [SerializeField] private Color dangerColor  = new Color(0.85f, 0.15f, 0.15f);
    [SerializeField] private Color safeColor    = new Color(0.15f, 0.75f, 0.35f);
    [SerializeField] private Color neutralColor = new Color(0.9f, 0.8f, 0.25f);
    [SerializeField] private Color inactiveColor = new Color(0.7f, 0.7f, 0.7f);

    private bool _resultCalled;

    private void Update()
    {
        if (bleedingSystem == null) return;

        float bleed             = bleedingSystem.BleedingLevel;
        PressureQuality quality = bleedingSystem.CurrentPressureQuality;
        BleedingGameState state = bleedingSystem.CurrentState;

        if (state == BleedingGameState.Success && !_resultCalled)
        {
            _resultCalled = true;
            GameManager.instance?.ResultScreen();
        }

        if (bleedingFillImage != null)
        {
            bleedingFillImage.fillAmount = bleed;
            bleedingFillImage.color = Color.Lerp(safeColor, dangerColor, bleed);
        }

        if (holdProgressFillImage != null)
            holdProgressFillImage.fillAmount = Mathf.Clamp01(bleedingSystem.HoldTime / Mathf.Max(bleedingSystem.WinHoldDuration, 0.001f));

        if (depthFillImage != null)
            depthFillImage.fillAmount = bleedingSystem.CurrentPressDepth;

        if (bleedingStateText != null)
        {
            bleedingStateText.text = $"BLEEDING: {bleedingSystem.BleedingStatusLabel.ToUpperInvariant()}";
            bleedingStateText.color = bleed >= 0.7f ? dangerColor : bleed >= 0.35f ? neutralColor : safeColor;
        }

        if (pressureText != null)
        {
            pressureText.text  = GetPressureLabel(quality);
            pressureText.color = GetPressureColor(quality);
        }

        if (holdText != null)
            holdText.text = $"TIMER: {bleedingSystem.TimeToControl:0.0}s";

        if (warningText != null)
            warningText.text = BuildWarningMessage(state, quality);
    }

    private string GetPressureLabel(PressureQuality quality)
    {
        switch (quality)
        {
            case PressureQuality.TooLight: return "TOO LIGHT";
            case PressureQuality.Good:     return "GOOD PRESSURE";
            case PressureQuality.TooHard:  return "TOO HARD";
            default:                       return "NO PRESSURE";
        }
    }

    private Color GetPressureColor(PressureQuality quality)
    {
        switch (quality)
        {
            case PressureQuality.TooLight: return neutralColor;
            case PressureQuality.Good:     return safeColor;
            case PressureQuality.TooHard:  return dangerColor;
            default:                       return inactiveColor;
        }
    }

    private string BuildWarningMessage(BleedingGameState state, PressureQuality quality)
    {
        if (state == BleedingGameState.Success)
            return "Bleeding controlled. Keep pressure until help arrives.";

        if (state == BleedingGameState.Prepare)
            return "Place one hand in the lower camera zone to begin.";

        if (bleedingSystem.IsBleedSurging && bleedingSystem.CurrentPressDepth < bleedingSystem.RecommendedMinDepth)
            return "Bleeding surge. Move your hand lower and hold firmer pressure.";

        if (!bleedingSystem.IsPressing)
            return "Keep your hand inside the lower zone. Releasing makes bleeding worse.";

        if (quality == PressureQuality.TooLight)
            return "Press harder. Current pressure is not enough.";

        if (quality == PressureQuality.TooHard)
            return "Ease slightly. Too much force is less effective.";

        if (bleedingSystem.StabilityScore < 0.55f)
            return "Hold steady. Unstable pressure slows progress.";

        if (bleedingSystem.BleedingLevel <= bleedingSystem.WinBleedingThreshold)
            return "Almost there. Maintain steady pressure.";

        return "Good. Keep the pressure steady.";
    }
}
