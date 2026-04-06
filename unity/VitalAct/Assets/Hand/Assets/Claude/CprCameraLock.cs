using UnityEngine;

/// <summary>
/// ล็อค camera ไว้ที่ตำแหน่ง/มุมที่กำหนดตอน GameState.Do (CPR stage)
///
/// วิธีตั้งค่า:
///   1. Attach script นี้กับ Player GameObject
///   2. กด Play → เดินกล้องไปมุมที่ต้องการ
///   3. กด Pause → คลิกขวาที่ script ใน Inspector → "Capture Current Camera Pose"
///   4. กด Play ใหม่ → กล้องจะ lock อัตโนมัติตอนเข้า CPR stage
/// </summary>
public class CprCameraLock : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Main Camera ของ Player")]
    public Camera playerCamera;
    [Tooltip("PlayerMovement script ที่จะ disable ตอน CPR")]
    public PlayerMovement playerMovement;

    [Header("CPR Camera Pose")]
    [Tooltip("ตำแหน่ง Player (world space) ตอน CPR\nกด Capture Current Camera Pose เพื่อบันทึก")]
    public Vector3 lockedPosition = new Vector3(0f, 1.6f, 0f);
    [Tooltip("มุม Player (Euler Y) ตอน CPR")]
    public float   lockedYaw      = 0f;
    [Tooltip("มุม Camera (Euler X, pitch up/down) ตอน CPR")]
    public float   lockedPitch    = 15f;

    [Header("Transition")]
    [Tooltip("ความเร็ว smooth ไปยัง CPR position (สูง = เร็ว)")]
    public float transitionSpeed = 4f;

    // ---- internal ----
    bool _locked;
    Vector3 _originPos;
    float   _originYaw;
    float   _originPitch;

    // current smoothed values
    Vector3 _curPos;
    float   _curYaw;
    float   _curPitch;

    void Awake()
    {
        if (playerCamera  == null) playerCamera  = Camera.main;
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        _curPos   = transform.position;
        _curYaw   = transform.eulerAngles.y;
        _curPitch = playerCamera != null ? playerCamera.transform.localEulerAngles.x : 0f;
    }

    void Update()
    {
        bool shouldLock = GameManager.instance != null &&
                          GameManager.instance.CurrentState == GameManager.GameState.Do;

        if (shouldLock && !_locked)  EnterLock();
        if (!shouldLock && _locked)  ExitLock();

        if (!_locked) return;

        // smooth ไปยังตำแหน่ง CPR
        float t = Time.deltaTime * transitionSpeed;
        _curPos   = Vector3.Lerp(_curPos,   lockedPosition, t);
        _curYaw   = Mathf.LerpAngle(_curYaw,   lockedYaw,   t);
        _curPitch = Mathf.LerpAngle(_curPitch, lockedPitch,  t);

        transform.position = _curPos;
        transform.rotation = Quaternion.Euler(0f, _curYaw, 0f);
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(_curPitch, 0f, 0f);
    }

    void EnterLock()
    {
        _locked = true;
        // บันทึกตำแหน่งเดิม
        _originPos   = transform.position;
        _originYaw   = transform.eulerAngles.y;
        _originPitch = playerCamera != null
            ? WrapAngle(playerCamera.transform.localEulerAngles.x) : 0f;

        // เริ่ม smooth จากตำแหน่งปัจจุบัน
        _curPos   = _originPos;
        _curYaw   = _originYaw;
        _curPitch = _originPitch;

        if (playerMovement != null) playerMovement.enabled = false;

        // unlock cursor สำหรับ CPR interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    void ExitLock()
    {
        _locked = false;
        transform.position = _originPos;
        transform.rotation = Quaternion.Euler(0f, _originYaw, 0f);
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(_originPitch, 0f, 0f);

        if (playerMovement != null) playerMovement.enabled = true;
    }

    // ── บันทึกตำแหน่งกล้องปัจจุบันเป็น CPR pose ──
    [ContextMenu("Capture Current Camera Pose")]
    void CapturePose()
    {
        lockedPosition = transform.position;
        lockedYaw      = transform.eulerAngles.y;
        lockedPitch    = playerCamera != null
            ? WrapAngle(playerCamera.transform.localEulerAngles.x) : 0f;

        Debug.Log($"[CprCameraLock] Captured — Pos:{lockedPosition}  Yaw:{lockedYaw:F1}  Pitch:{lockedPitch:F1}");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    // แปลง 270° → -90° ฯลฯ
    static float WrapAngle(float a) => a > 180f ? a - 360f : a;
}
