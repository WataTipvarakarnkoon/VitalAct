using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BleedingUI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private BleedingSystem bleedingSystem;
    [SerializeField] private BleedingHandDetector handDetector;

    [Header("Primary UI")]
    [SerializeField] private Slider bleedingSlider;
    [SerializeField] private Image bleedingFill;
    [SerializeField] private Image bleedingFillImage;
    [SerializeField] private Image holdProgressFillImage;
    [SerializeField] private Image depthFillImage;
    [SerializeField] private TMP_Text bleedingStateText;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text pressureText;
    [SerializeField] private TMP_Text holdText;
    [SerializeField] private TMP_Text stabilityText;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private TMP_Text cameraGuideText;

    [Header("Optional Feedback")]
    [SerializeField] private Image stressOverlay;
    [SerializeField] private CanvasGroup warningGroup;
    [SerializeField] private Color dangerColor = new Color(0.85f, 0.15f, 0.15f);
    [SerializeField] private Color safeColor = new Color(0.15f, 0.75f, 0.35f);
    [SerializeField] private Color neutralColor = new Color(0.9f, 0.8f, 0.25f);
    [SerializeField] private Color inactiveColor = new Color(0.7f, 0.7f, 0.7f);

    public void ConfigureSimpleHud(
        BleedingSystem system,
        BleedingHandDetector detector,
        Image bleedingFillImageRef,
        Image holdProgressFillImageRef,
        Image depthFillImageRef,
        TMP_Text objectiveTextRef,
        TMP_Text bleedingStateTextRef,
        TMP_Text pressureTextRef,
        TMP_Text holdTextRef,
        TMP_Text warningTextRef,
        TMP_Text stabilityTextRef,
        TMP_Text cameraGuideTextRef,
        Image stressOverlayRef)
    {
        bleedingSystem = system;
        handDetector = detector;
        bleedingFillImage = bleedingFillImageRef;
        holdProgressFillImage = holdProgressFillImageRef;
        depthFillImage = depthFillImageRef;
        objectiveText = objectiveTextRef;
        bleedingStateText = bleedingStateTextRef;
        pressureText = pressureTextRef;
        holdText = holdTextRef;
        warningText = warningTextRef;
        stabilityText = stabilityTextRef;
        cameraGuideText = cameraGuideTextRef;
        stressOverlay = stressOverlayRef;
    }

    private void Awake()
    {
        if (bleedingSlider != null)
        {
            bleedingSlider.minValue = 0f;
            bleedingSlider.maxValue = 1f;
        }
    }

    private void Update()
    {
        if (bleedingSystem == null)
        {
            return;
        }

        float bleed = bleedingSystem.BleedingLevel;
        PressureQuality quality = bleedingSystem.CurrentPressureQuality;
        BleedingGameState state = bleedingSystem.CurrentState;

        if (bleedingSlider != null)
        {
            bleedingSlider.value = bleed;
        }

        if (bleedingFillImage != null)
        {
            bleedingFillImage.fillAmount = bleed;
        }

        if (bleedingFill != null)
        {
            bleedingFill.color = Color.Lerp(safeColor, dangerColor, bleed);
        }

        if (bleedingFillImage != null)
        {
            bleedingFillImage.color = Color.Lerp(safeColor, dangerColor, bleed);
        }

        if (stateText != null)
        {
            stateText.text = $"State: {GetStateLabel(state)}";
        }

        if (objectiveText != null)
        {
            objectiveText.text = state == BleedingGameState.Success
                ? "Bleeding controlled."
                : bleedingSystem.IsBleedSurging
                    ? "Bleeding surge. Increase pressure now."
                    : "Maintain firm pressure on the wound.";
        }

        if (bleedingStateText != null)
        {
            bleedingStateText.text = $"BLEEDING: {bleedingSystem.BleedingStatusLabel.ToUpperInvariant()}";
            bleedingStateText.color = bleed >= 0.7f ? dangerColor : bleed >= 0.35f ? neutralColor : safeColor;
        }

        if (pressureText != null)
        {
            pressureText.text = GetPressureLabel(quality).ToUpperInvariant();
            pressureText.color = GetPressureColor(quality);
        }

        if (holdText != null)
        {
            holdText.text = $"TIMER: {bleedingSystem.TimeToControl:0.0}s";
        }

        if (stabilityText != null)
        {
            stabilityText.text = $"Stability {bleedingSystem.StabilityScore * 100f:0}%   Depth {bleedingSystem.CurrentPressDepth * 100f:0}%   Min {bleedingSystem.RecommendedMinDepth * 100f:0}%";
        }

        if (holdProgressFillImage != null)
        {
            holdProgressFillImage.fillAmount = Mathf.Clamp01(bleedingSystem.HoldTime / Mathf.Max(bleedingSystem.WinHoldDuration, 0.001f));
        }

        if (depthFillImage != null)
        {
            depthFillImage.fillAmount = bleedingSystem.CurrentPressDepth;
        }

        if (warningText != null)
        {
            warningText.text = BuildWarningMessage(state, quality);
            warningText.color = bleedingSystem.IsBleedSurging || bleedingSystem.IsCritical ? dangerColor : Color.white;
        }

        if (cameraGuideText != null)
        {
            if (handDetector == null)
            {
                cameraGuideText.text = "Place hand in zone";
            }
            else
            {
                cameraGuideText.text = handDetector.HandInZone
                    ? "Move lower to press harder"
                    : "Place hand in lower zone";
            }
        }

        if (warningGroup != null)
        {
            float pulse = 0.65f + 0.35f * Mathf.Abs(Mathf.Sin(Time.time * 4f));
            warningGroup.alpha = bleedingSystem.IsCritical ? pulse : 1f;
        }

        if (stressOverlay != null)
        {
            float targetAlpha = bleed * 0.14f;
            if (!bleedingSystem.IsPressing && state == BleedingGameState.Active)
            {
                targetAlpha += 0.06f;
            }

            Color color = stressOverlay.color;
            color.a = Mathf.Lerp(color.a, Mathf.Clamp01(targetAlpha), Time.deltaTime * 6f);
            stressOverlay.color = color;
        }
    }

    private string GetStateLabel(BleedingGameState state)
    {
        switch (state)
        {
            case BleedingGameState.Prepare:
                return "Prepare";
            case BleedingGameState.Active:
                return "Active";
            case BleedingGameState.Success:
                return "Controlled";
            default:
                return state.ToString();
        }
    }

    private string GetPressureLabel(PressureQuality quality)
    {
        switch (quality)
        {
            case PressureQuality.TooLight:
                return "Too Light";
            case PressureQuality.Good:
                return "Good Pressure";
            case PressureQuality.TooHard:
                return "Too Hard";
            default:
                return "No Pressure";
        }
    }

    private Color GetPressureColor(PressureQuality quality)
    {
        switch (quality)
        {
            case PressureQuality.TooLight:
                return neutralColor;
            case PressureQuality.Good:
                return safeColor;
            case PressureQuality.TooHard:
                return dangerColor;
            default:
                return inactiveColor;
        }
    }

    private string BuildWarningMessage(BleedingGameState state, PressureQuality quality)
    {
        if (state == BleedingGameState.Success)
        {
            return "Bleeding controlled. Keep pressure until help arrives.";
        }

        if (state == BleedingGameState.Prepare)
        {
            return "Place one hand in the lower camera zone to begin.";
        }

        if (bleedingSystem.IsBleedSurging && bleedingSystem.CurrentPressDepth < bleedingSystem.RecommendedMinDepth)
        {
            return "Bleeding surge. Move your hand lower and hold firmer pressure.";
        }

        if (!bleedingSystem.IsPressing)
        {
            return "Keep your hand inside the lower zone. Releasing makes bleeding worse.";
        }

        if (quality == PressureQuality.TooLight)
        {
            return "Press harder. Current pressure is not enough.";
        }

        if (quality == PressureQuality.TooHard)
        {
            return "Ease slightly. Too much force is less effective.";
        }

        if (bleedingSystem.StabilityScore < 0.55f)
        {
            return "Hold steady. Unstable pressure slows progress.";
        }

        if (bleedingSystem.BleedingLevel <= bleedingSystem.WinBleedingThreshold)
        {
            return "Almost there. Maintain steady pressure.";
        }

        return "Good. Keep the pressure steady.";
    }
}
