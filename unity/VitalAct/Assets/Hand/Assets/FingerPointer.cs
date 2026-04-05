using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class FingerPointer : MonoBehaviour
{
    [Header("Settings")]
    public float handDepth = 1.5f;
    public float hoverTime = 1.5f;
    public float rayLength = 50f;

    private HandLandmarkerResult latestResult;
    private bool hasResult = false;
    private float hoverTimer = 0f;
    private Collider lastHovered = null;

    void Start()
    {
        HandLandmarkerRunner.OnHandLandmarkResult += OnReceiveResult;
    }

    void OnDestroy()
    {
        HandLandmarkerRunner.OnHandLandmarkResult -= OnReceiveResult;
    }

    void OnReceiveResult(HandLandmarkerResult result)
    {
        latestResult = result;
        hasResult = true;
    }

    void Update()
    {
        if (!hasResult || latestResult.handLandmarks == null) return;
        if (latestResult.handLandmarks.Count == 0) return;

        var landmarks = latestResult.handLandmarks[0].landmarks;
        if (landmarks == null || landmarks.Count < 9) return;

        // ปลายนิ้วชี้ = landmark 8
        var tip = landmarks[8];
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