using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using System.Collections.Generic;

public class ProceduralHand : MonoBehaviour
{
    [Header("Settings")]
    public float handDepth = 1.5f;
    public float smoothSpeed = 15f;

    [Header("Appearance")]
    public Color skinColor = new Color(1f, 0.75f, 0.6f);
    public Color jointColor = new Color(0.9f, 0.4f, 0.4f);

    [Header("Thickness")]
    public float thumbWidth = 0.035f;
    public float fingerWidth = 0.025f;
    public float tipWidth = 0.02f;
    public float palmWidth = 0.045f;

    private static readonly int[][] bones = {
        new int[]{0,1,0}, new int[]{0,5,0}, new int[]{0,17,0},
        new int[]{5,9,0}, new int[]{9,13,0}, new int[]{13,17,0},
        new int[]{1,2,1}, new int[]{2,3,1}, new int[]{3,4,3},
        new int[]{5,6,2}, new int[]{6,7,2}, new int[]{7,8,3},
        new int[]{9,10,2}, new int[]{10,11,2}, new int[]{11,12,3},
        new int[]{13,14,2}, new int[]{14,15,2}, new int[]{15,16,3},
        new int[]{17,18,2}, new int[]{18,19,2}, new int[]{19,20,3},
    };

    private HandData hand;
    private HandLandmarkerResult latestResult;
    private bool hasResult = false;

    void Start()
    {
        hand = CreateHand();
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

    HandData CreateHand()
    {
        var data = new HandData();
        var root = new GameObject("ProceduralHand");
        root.transform.parent = transform;

        data.positions = new Vector3[21];

        var skinMat = CreateMat(skinColor);
        var jointMat = CreateMat(jointColor);

        data.joints = new GameObject[21];

        for (int i = 0; i < 21; i++)
        {
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.parent = root.transform;
            s.transform.localScale = Vector3.one * 0.04f;
            s.GetComponent<Renderer>().material = jointMat;

            var rb = s.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            s.tag = "Hand";
            data.joints[i] = s;
        }

        data.capsules = new GameObject[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            var c = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            c.transform.parent = root.transform;
            c.GetComponent<Renderer>().material = skinMat;
            Destroy(c.GetComponent<CapsuleCollider>());
            data.capsules[i] = c;
        }

        data.root = root;
        return data;
    }

    Material CreateMat(Color color)
    {
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = color;
        mat.SetFloat("_Smoothness", 0.4f);
        mat.SetFloat("_Metallic", 0f);
        return mat;
    }

    void Update()
    {
        if (!hasResult) return;
        if (latestResult.handLandmarks == null) return;
        if (latestResult.handLandmarks.Count == 0) return;

        var landmarks = latestResult.handLandmarks[0].landmarks;

        // 🔥 กัน error index out of range
        if (landmarks == null || landmarks.Count < 21) return;

        for (int i = 0; i < 21; i++)
        {
            var lm = landmarks[i];

            Vector3 target = Camera.main.ViewportToWorldPoint(
                new Vector3(lm.x, 1f - lm.y, handDepth));

            hand.positions[i] = Vector3.Lerp(
                hand.positions[i],
                target,
                Time.deltaTime * smoothSpeed);

            hand.joints[i].transform.position = hand.positions[i];
        }

        for (int i = 0; i < bones.Length; i++)
        {
            int a = bones[i][0];
            int b = bones[i][1];

            if (a >= 21 || b >= 21) continue;

            UpdateCapsule(
                hand.capsules[i],
                hand.positions[a],
                hand.positions[b],
                fingerWidth);
        }
    }

    void UpdateCapsule(GameObject cap, Vector3 a, Vector3 b, float width)
    {
        cap.transform.position = (a + b) * 0.5f;

        Vector3 dir = b - a;
        float len = dir.magnitude;

        if (len > 0.0001f)
            cap.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        cap.transform.localScale = new Vector3(width, len * 0.5f, width);
    }

    class HandData
    {
        public GameObject root;
        public GameObject[] joints;
        public GameObject[] capsules;
        public Vector3[] positions;
    }
}