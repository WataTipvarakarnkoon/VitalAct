using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// นับรอบ CPR แบบ 30:2 (30 กด → 2 เป่าปาก → ซ้ำ)
/// ช่วง Breathing ใช้ countdown timer เพราะตรวจการเป่าปากจากกล้องไม่ได้
/// </summary>
public class CprCycleCounter : MonoBehaviour
{
    [Header("Settings")]
    public int compressionsPerCycle = 30;
    [Tooltip("เวลา (วินาที) ที่ให้เป่าปาก ก่อนกลับมากดรอบถัดไป")]
    public float breathPhaseDuration = 5f;

    [Header("UI")]
    public Text compressionCountText;   // แสดง "12 / 30"
    public Text cycleCountText;         // แสดง "รอบที่ 2"
    public Text phaseText;              // แสดง "กด CPR" หรือ "เป่าปาก!"
    public Text breathTimerText;        // แสดงเวลาถอยหลังช่วงเป่าปาก
    public GameObject breathPanel;      // panel ที่โชว์ตอน breathing phase

    [Header("Output - Read Only")]
    public int currentCompression;      // นับในรอบปัจจุบัน
    public int completedCycles;         // รอบที่เสร็จแล้ว
    public bool isBreathingPhase;

    float _breathTimer;

    void OnEnable()  => CprHandDetector.OnCompression += OnCompression;
    void OnDisable() => CprHandDetector.OnCompression -= OnCompression;

    void Start() => UpdateUI();

    void OnCompression()
    {
        if (isBreathingPhase) return;  // ไม่นับตอนอยู่ช่วงเป่าปาก

        currentCompression++;
        UpdateUI();

        if (currentCompression >= compressionsPerCycle)
            EnterBreathingPhase();
    }

    void EnterBreathingPhase()
    {
        isBreathingPhase = true;
        _breathTimer = breathPhaseDuration;
        if (breathPanel != null) breathPanel.SetActive(true);
        UpdateUI();
    }

    void Update()
    {
        if (!isBreathingPhase) return;

        _breathTimer -= Time.deltaTime;

        if (breathTimerText != null)
            breathTimerText.text = Mathf.CeilToInt(_breathTimer).ToString();

        if (_breathTimer <= 0f)
            EnterCompressionPhase();
    }

    void EnterCompressionPhase()
    {
        isBreathingPhase = false;
        currentCompression = 0;
        completedCycles++;
        if (breathPanel != null) breathPanel.SetActive(false);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (compressionCountText != null)
            compressionCountText.text = $"{currentCompression} / {compressionsPerCycle}";

        if (cycleCountText != null)
            cycleCountText.text = $"รอบที่ {completedCycles + 1}";

        if (phaseText != null)
            phaseText.text = isBreathingPhase ? "เป่าปาก!" : "กด CPR";
    }

    public void ResetAll()
    {
        isBreathingPhase = false;
        currentCompression = 0;
        completedCycles = 0;
        _breathTimer = 0f;
        if (breathPanel != null) breathPanel.SetActive(false);
        UpdateUI();
    }
}
