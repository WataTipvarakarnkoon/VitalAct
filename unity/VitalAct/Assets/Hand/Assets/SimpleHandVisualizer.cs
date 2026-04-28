using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;

// แสดง sphere ที่ข้อมือซ้าย(15) และขวา(16) จาก Pose landmark
public class SimpleHandVisualizer : MonoBehaviour
{
    public float handDepth = 2f;
    public float smoothSpeed = 25f;

    // Pose: left_wrist=15, right_wrist=16
    private static readonly int[] poseWristIndices = { 15, 16 };
    private static readonly string[] wristNames = { "Left_Wrist", "Right_Wrist" };
    private static readonly Color[] wristColors = { Color.red, Color.blue };

    private List<GameObject> wristSpheres = new List<GameObject>();

    private PoseLandmarkerResult latestResult;
    private bool hasResult = false;

    void Start()
    {
        var parent = new GameObject("Wrist_Visualizer");
        parent.transform.parent = this.transform;

        for (int i = 0; i < 2; i++)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = wristNames[i];
            sphere.transform.parent = parent.transform;
            sphere.transform.localScale = Vector3.one * 0.05f;
            sphere.SetActive(false);

            var col = sphere.GetComponent<SphereCollider>();
            col.radius = 0.5f;

            var rb = sphere.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            var mat = new Material(Shader.Find("Standard"));
            mat.color = wristColors[i];
            sphere.GetComponent<Renderer>().material = mat;

            wristSpheres.Add(sphere);
        }

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
        foreach (var obj in wristSpheres) obj.SetActive(false);

        if (!hasResult || latestResult.poseLandmarks == null) return;
        if (latestResult.poseLandmarks.Count == 0) return;

        var landmarks = latestResult.poseLandmarks[0].landmarks;
        if (landmarks.Count < 17) return;

        for (int i = 0; i < 2; i++)
        {
            var lm = landmarks[poseWristIndices[i]];
            wristSpheres[i].SetActive(true);

            Vector3 worldPos = Camera.main.ViewportToWorldPoint(
                new Vector3(lm.x, 1f - lm.y, handDepth)
            );

            var rb = wristSpheres[i].GetComponent<Rigidbody>();
            if (rb != null)
                rb.MovePosition(Vector3.Lerp(wristSpheres[i].transform.position, worldPos, Time.deltaTime * smoothSpeed));
            else
                wristSpheres[i].transform.position = Vector3.Lerp(wristSpheres[i].transform.position, worldPos, Time.deltaTime * smoothSpeed);
        }
    }
}
