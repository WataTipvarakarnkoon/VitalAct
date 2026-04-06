using UnityEngine;

/// <summary>
/// เชื่อม CprGameManager กับ TimerBar ที่มีอยู่ในเกม
/// - ดึง duration จาก TimerBar มาใช้เป็น sessionDuration
/// - Drive timer ของ TimerBar จาก CprGameManager
/// - จบ session เมื่อ TimerBar หมดเวลา
/// </summary>
public class CprTimerBridge : MonoBehaviour
{
    [Header("References (auto-find ถ้าว่าง)")]
    public CprGameManager gameManager;
    public TimerBar       timerBar;

    [Header("Bar Colors")]
    public Color colorFull = Color.green;
    public Color colorMid  = Color.yellow;
    public Color colorLow  = Color.red;

    void Awake()
    {
        if (gameManager == null) gameManager = FindObjectOfType<CprGameManager>();
        if (timerBar    == null) timerBar    = FindObjectOfType<TimerBar>();

        // sync duration: ถ้า TimerBar มี duration ตั้งไว้แล้ว ใช้ค่านั้น
        if (gameManager != null && timerBar != null && timerBar.duration > 0f)
        {
            gameManager.sessionDuration = timerBar.duration;
            Debug.Log($"[CprTimerBridge] sessionDuration = {timerBar.duration}s (from TimerBar)");
        }
    }

    void OnEnable()  => CprGameManager.OnStateChanged += OnStateChanged;
    void OnDisable() => CprGameManager.OnStateChanged -= OnStateChanged;

    void OnStateChanged(CprGameState state)
    {
        if (timerBar == null) return;

        if (state == CprGameState.Playing)
        {
            // reset TimerBar ให้เริ่มนับใหม่
            timerBar.timer = 0f;
            timerBar.enabled = true;
        }
        else
        {
            // หยุด TimerBar เมื่อออกจาก Playing
            timerBar.enabled = false;
        }
    }

    void Update()
    {
        if (gameManager == null || timerBar == null) return;
        if (gameManager.State != CprGameState.Playing) return;

        // Drive fillAmount จาก SessionTimeLeft โดยตรง
        // TimerBar ถูก disabled แล้ว ดังนั้น Update() ของมันไม่รัน
        float progress = gameManager.SessionTimeLeft / gameManager.sessionDuration;
        if (timerBar.fillImage != null)
            timerBar.fillImage.fillAmount = Mathf.Clamp01(progress);

        // เปลี่ยนสีตามเวลาที่เหลือ
        Color barColor = progress > 0.5f
            ? Color.Lerp(colorMid, colorFull, (progress - 0.5f) * 2f)
            : Color.Lerp(colorLow, colorMid,   progress * 2f);
        if (timerBar.fillImage != null)
            timerBar.fillImage.color = barColor;
    }
}
