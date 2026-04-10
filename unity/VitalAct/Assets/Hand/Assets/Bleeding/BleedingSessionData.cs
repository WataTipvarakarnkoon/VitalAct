using UnityEngine;

public class BleedingSessionData
{
    public float assessmentDuration;
    public float totalDuration;
    public float totalPressTime;
    public float goodPressureTime;
    public float tooLightTime;
    public float tooHardTime;
    public float stableTime;
    public int survivedSurgeCount;
    public int checklistSubmissionCount;
    public bool checklistCompleted;
    public bool sceneSafeChecked;
    public bool severeBleedingChecked;
    public bool emergencyCalledChecked;
    public bool readyForPressureChecked;
    public int initialChoiceAttempts;
    public bool initialChoiceCorrect;
    public int reassessChoiceAttempts;
    public bool reassessChoiceCorrect;
    public int incorrectChoices;
    public float averageDepth;
    public float maxDepth;
    public float finalBleedingLevel;

    public float PressureAccuracy =>
        totalPressTime > 0.001f ? goodPressureTime / totalPressTime * 100f : 0f;

    public float StabilityPercent =>
        totalPressTime > 0.001f ? stableTime / totalPressTime * 100f : 0f;

    public float ControlScore
    {
        get
        {
            float bleedingControl = (1f - finalBleedingLevel) * 100f;
            float decisionBonus =
                (checklistCompleted ? 10f : 0f) +
                (initialChoiceCorrect ? 8f : 0f) +
                (reassessChoiceCorrect ? 7f : 0f) -
                incorrectChoices * 4f;
            return Mathf.Clamp(
                bleedingControl * 0.45f +
                PressureAccuracy * 0.3f +
                StabilityPercent * 0.2f +
                Mathf.Clamp01(averageDepth) * 100f * 0.05f +
                decisionBonus,
                0f,
                100f);
        }
    }

    public string Grade
    {
        get
        {
            float score = ControlScore;
            if (score >= 90f) return "A";
            if (score >= 75f) return "B";
            if (score >= 60f) return "C";
            return "D";
        }
    }
}
