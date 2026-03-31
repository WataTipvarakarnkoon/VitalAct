using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class GrabController : MonoBehaviour
{
    [Header("Settings")]
    public float handDepth = 2f;
    public float grabThreshold = 0.08f;   // ระยะห่างที่ถือว่า "หยิบ"
    public float smoothSpeed = 25f;

    // เก็บ state แยกต่อมือ
    private GrabHand leftHand = new GrabHand();
    private GrabHand rightHand = new GrabHand();

    private HandLandmarkerResult latestResult;
    private bool hasResult = false;

    void Start()
    {
        HandLandmarkerRunner.OnHandLandmarkResult += OnReceiveResult;
    }

    void OnDestroy()
    {
        HandLandmarkerRunner.OnHandLandmarkResult -= OnReceiveResult;
    }

    private void OnReceiveResult(HandLandmarkerResult result)
    {
        latestResult = result;
        hasResult = true;
    }

    void Update()
    {
        if (!hasResult || latestResult.handLandmarks == null) return;

        // reset ถ้าไม่เจอมือ
        if (latestResult.handLandmarks.Count == 0)
        {
            leftHand.Release();
            rightHand.Release();
            return;
        }

        bool foundLeft = false;
        bool foundRight = false;

        for (int h = 0; h < latestResult.handLandmarks.Count; h++)
        {
            bool isLeft = false;
            if (latestResult.handedness != null && h < latestResult.handedness.Count)
            {
                var handedness = latestResult.handedness[h].categories[0].categoryName;
                isLeft = handedness == "Left";
            }

            var landmarks = latestResult.handLandmarks[h].landmarks;
            if (landmarks.Count < 21) continue;

            // ตำแหน่งนิ้วหัวแม่มือ (4) และนิ้วชี้ (8)
            Vector3 thumbPos = GetWorldPos(landmarks[4]);
            Vector3 indexPos = GetWorldPos(landmarks[8]);

            // จุดกึ่งกลางระหว่างสองนิ้ว = ตำแหน่งที่ใช้เลื่อน object
            Vector3 pinchCenter = (thumbPos + indexPos) / 2f;

            float distance = Vector3.Distance(thumbPos, indexPos);
            bool isPinching = distance < grabThreshold;

            var hand = isLeft ? leftHand : rightHand;

            if (isLeft) foundLeft = true;
            else foundRight = true;

            hand.UpdateGrab(isPinching, pinchCenter, smoothSpeed);
        }

        if (!foundLeft) leftHand.Release();
        if (!foundRight) rightHand.Release();
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

    public void UpdateGrab(bool isPinching, Vector3 pinchCenter, float smoothSpeed)
    {
        if (isPinching)
        {
            if (!wasPinching)
            {
                // เพิ่งเริ่มหยิบ → หา object ที่ใกล้ที่สุด
                TryGrab(pinchCenter);
            }

            if (grabbedObject != null)
            {
                // เลื่อน object ตาม pinch center
                Vector3 targetPos = pinchCenter + grabOffset;
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
            // ปล่อย object
            if (grabbedObject != null)
            {
                var rb = grabbedObject.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;
                grabbedObject = null;
            }
        }

        wasPinching = isPinching;
    }

    void TryGrab(Vector3 pinchCenter)
    {
        // หา Collider ที่อยู่ในรัศมี 0.15f รอบ pinch point
        Collider[] hits = Physics.OverlapSphere(pinchCenter, 0.15f);
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
            // คำนวณ offset ระหว่าง object กับ pinch center
            grabOffset = grabbedObject.transform.position - pinchCenter;

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