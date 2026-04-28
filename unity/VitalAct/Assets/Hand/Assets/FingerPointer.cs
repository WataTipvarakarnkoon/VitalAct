using UnityEngine;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;

public class FingerPointer : MonoBehaviour
{
    [Header("Settings")]
    public float handDepth = 1.5f;
    public float hoverTime = 1.5f;
    public float rayLength = 50f;

    private PoseLandmarkerResult latestResult;
    private bool hasResult = false;
    private float hoverTimer = 0f;
    private Collider lastHovered = null;

    void Start()
    {
        PoseLandmarkerRunner.OnPoseLandmarkResult += OnReceiveResult;
    }

    void OnDestroy()
    {
        PoseLandmarkerRunner.OnPoseLandmarkResult -= OnReceiveResult;
    }

    void OnReceiveResult(PoseLandmarkerResult result)
    {
        latestResult = result;
        hasResult = true;
    }

    void Update()
    {
        if (!hasResult || latestResult.poseLandmarks == null) return;
        if (latestResult.poseLandmarks.Count == 0) return;

        var landmarks = latestResult.poseLandmarks[0].landmarks;
        if (landmarks == null || landmarks.Count < 17) return;

        // ใช้ left_wrist(15) เป็นตัวชี้
        var tip = landmarks[15];
        Vector3 tipWorld = Camera.main.ViewportToWorldPoint(
            new Vector3(1f - tip.x, 1f - tip.y, handDepth));

        // ยิง Ray จากกล้องผ่านปลายนิ้ว
        Ray ray = new Ray(Camera.main.transform.position,
            (tipWorld - Camera.main.transform.position).normalized);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayLength))
        {
            var col = hit.collider;
            Debug.Log("Pointing at: " + col.name);

            if (col == lastHovered)
            {
                hoverTimer += Time.deltaTime;
                Debug.Log($"Hover: {hoverTimer:F1}/{hoverTime}");

                if (hoverTimer >= hoverTime)
                {
                    // คลิก!
                    var btn = col.GetComponentInParent<UnityEngine.UI.Button>();
                    if (btn != null)
                    {
                        btn.onClick.Invoke();
                        Debug.Log("Clicked: " + col.name);
                    }
                    else
                    {
                        // ถ้าไม่มี Button ลอง SendMessage
                        col.SendMessageUpwards("OnPointerClick",
                            SendMessageOptions.DontRequireReceiver);
                    }

                    hoverTimer = 0f;
                    lastHovered = null;
                }
            }
            else
            {
                lastHovered = col;
                hoverTimer = 0f;
            }
        }
        else
        {
            lastHovered = null;
            hoverTimer = 0f;
        }
    }
}