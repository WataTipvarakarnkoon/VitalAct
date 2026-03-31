using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class SimpleHandVisualizer : MonoBehaviour
{
    public float handDepth = 2f;
    public float smoothSpeed = 25f;

    private List<GameObject> leftJoints = new List<GameObject>();
    private List<GameObject> rightJoints = new List<GameObject>();

    // index ของ joint ที่ต้องการ collider
    private int[] colliderJoints = { 0, 4, 8, 12, 16, 20 };

    private HandLandmarkerResult latestResult;
    private bool hasResult = false;

    private string[] landmarkNames = {
        "Wrist",
        "Thumb_CMC", "Thumb_MCP", "Thumb_IP", "Thumb_Tip",
        "Index_MCP", "Index_PIP", "Index_DIP", "Index_Tip",
        "Middle_MCP", "Middle_PIP", "Middle_DIP", "Middle_Tip",
        "Ring_MCP", "Ring_PIP", "Ring_DIP", "Ring_Tip",
        "Pinky_MCP", "Pinky_PIP", "Pinky_DIP", "Pinky_Tip"
    };

    void Start()
    {
        leftJoints = CreateHandJoints("Left", Color.red);
        rightJoints = CreateHandJoints("Right", Color.blue);
        HandLandmarkerRunner.OnHandLandmarkResult += OnReceiveResult;
    }

    List<GameObject> CreateHandJoints(string handName, Color color)
    {
        var joints = new List<GameObject>();
        var parent = new GameObject(handName + "_Hand");
        parent.transform.parent = this.transform;

        for (int i = 0; i < 21; i++)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = handName + "_" + landmarkNames[i];
            sphere.transform.parent = parent.transform;
            sphere.SetActive(false);

            bool isColliderJoint = System.Array.IndexOf(colliderJoints, i) >= 0;

            if (isColliderJoint)
            {
                // ปลายนิ้ว: ใหญ่กว่า + มี physics
                sphere.transform.localScale = Vector3.one * 0.05f;

                var col = sphere.GetComponent<SphereCollider>();
                col.radius = 0.5f;

                var rb = sphere.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
            else
            {
                // joint ทั่วไป: เล็กกว่า ไม่มี collider
                sphere.transform.localScale = Vector3.one * 0.02f;
                Destroy(sphere.GetComponent<SphereCollider>());
            }

            var mat = new Material(Shader.Find("Standard"));
            mat.color = isColliderJoint ? Color.white : color;
            sphere.GetComponent<Renderer>().material = mat;

            joints.Add(sphere);
        }
        return joints;
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
        foreach (var obj in leftJoints) obj.SetActive(false);
        foreach (var obj in rightJoints) obj.SetActive(false);

        if (!hasResult || latestResult.handLandmarks == null) return;

        int handCount = latestResult.handLandmarks.Count;

        for (int h = 0; h < handCount; h++)
        {
            bool isLeft = false;
            if (latestResult.handedness != null && h < latestResult.handedness.Count)
            {
                var handedness = latestResult.handedness[h].categories[0].categoryName;
                isLeft = handedness == "Left";
            }

            var joints = isLeft ? leftJoints : rightJoints;
            var landmarks = latestResult.handLandmarks[h].landmarks;

            for (int i = 0; i < Mathf.Min(21, landmarks.Count); i++)
            {
                joints[i].SetActive(true);

                var lm = landmarks[i];
                Vector3 worldPos = Camera.main.ViewportToWorldPoint(
                    new Vector3(lm.x, 1f - lm.y, handDepth)
                );

                var rb = joints[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // ใช้ MovePosition สำหรับ joint ที่มี physics
                    rb.MovePosition(Vector3.Lerp(
                        joints[i].transform.position,
                        worldPos,
                        Time.deltaTime * smoothSpeed
                    ));
                }
                else
                {
                    joints[i].transform.position = Vector3.Lerp(
                        joints[i].transform.position,
                        worldPos,
                        Time.deltaTime * smoothSpeed
                    );
                }
            }
        }
    }
}
