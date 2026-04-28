using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;

public class GrabController : MonoBehaviour
{
    [Header("Settings")]
    public float handDepth = 2f;
    public float grabThreshold = 0.15f;   // ระยะห่างข้อมือกับ object ที่ถือว่า "หยิบ"
    public float smoothSpeed = 25f;

    // เก็บ state แยกต่อมือ (Pose: 15=left_wrist, 16=right_wrist)
    private GrabHand leftHand = new GrabHand();
    private GrabHand rightHand = new GrabHand();

    private PoseLandmarkerResult latestResult;
    private bool hasResult = false;

    void Start()
    {
        PoseLandmarkerRunner.OnPoseLandmarkResult += OnReceiveResult;
    }

    void OnDestroy()
    {
        PoseLandmarkerRunner.OnPoseLandmarkResult -= OnReceiveResult;
    }

    private void OnReceiveResult(PoseLandmarkerResult result)
    {
        latestResult = result;
        hasResult = true;
    }

    void Update()
    {
        if (!hasResult || latestResult.poseLandmarks == null) return;

        if (latestResult.poseLandmarks.Count == 0)
        {
            leftHand.Release();
            rightHand.Release();
            return;
        }

        var lm = latestResult.poseLandmarks[0].landmarks;
        if (lm.Count < 17)
        {
            leftHand.Release();
            rightHand.Release();
            return;
        }

        // left_wrist=15, right_wrist=16
        Vector3 leftWrist  = GetWorldPos(lm[15]);
        Vector3 rightWrist = GetWorldPos(lm[16]);

        // Grab โดยใช้ wrist proximity (ไม่มี pinch ใน Pose)
        leftHand.UpdateGrab(true, leftWrist, smoothSpeed, grabThreshold);
        rightHand.UpdateGrab(true, rightWrist, smoothSpeed, grabThreshold);
    }

    Vector3 GetWorldPos(Mediapipe.Tasks.Components.Containers.NormalizedLandmark lm)
    {
        return Camera.main.ViewportToWorldPoint(
            new Vector3(lm.x, 1f - lm.y, handDepth)
        );
    }
}

// class เก็บ state ของแต่ละมือ
[System.Serializable]
public class GrabHand
{
    public GameObject grabbedObject = null;
    private Vector3 grabOffset;
    private bool wasPinching = false;

    public void UpdateGrab(bool wristVisible, Vector3 wristPos, float smoothSpeed, float grabThreshold)
    {
        if (wristVisible)
        {
            if (!wasPinching)
            {
                // ลอง grab object ที่ใกล้ wrist
                TryGrab(wristPos, grabThreshold);
            }

            if (grabbedObject != null)
            {
                Vector3 targetPos = wristPos + grabOffset;
                var rb = grabbedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.MovePosition(Vector3.Lerp(
                        grabbedObject.transform.position,
                        targetPos,
                        Time.deltaTime * smoothSpeed
                    ));
                }
                else
                {
                    grabbedObject.transform.position = Vector3.Lerp(
                        grabbedObject.transform.position,
                        targetPos,
                        Time.deltaTime * smoothSpeed
                    );
                }
            }
        }
        else
        {
            if (grabbedObject != null)
            {
                var rb = grabbedObject.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;
                grabbedObject = null;
            }
        }

        wasPinching = wristVisible && grabbedObject != null;
    }

    void TryGrab(Vector3 wristPos, float grabThreshold)
    {
        Collider[] hits = Physics.OverlapSphere(wristPos, grabThreshold);
        float closest = float.MaxValue;

        foreach (var hit in hits)
        {
            // ข้าม object ที่เป็นมือ (sphere joints)
            if (hit.GetComponent<Rigidbody>() == null) continue;
            if (hit.GetComponent<Rigidbody>().isKinematic) continue;

            float dist = Vector3.Distance(hit.transform.position, pinchCenter);
            if (dist < closest)
            {
                closest = dist;
                grabbedObject = hit.gameObject;
            }
        }

        if (grabbedObject != null)
        {
            grabOffset = grabbedObject.transform.position - wristPos;

            // หยุด physics ชั่วคราวขณะถือ
            var rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }
        }
    }

    public void Release()
    {
        if (grabbedObject != null)
        {
            var rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
            grabbedObject = null;
        }
        wasPinching = false;
    }
}