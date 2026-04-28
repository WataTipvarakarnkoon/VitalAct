using UnityEngine;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;

// แสดง arm skeleton: shoulder→elbow→wrist สำหรับแต่ละข้าง
// Pose indices: L shoulder=11, L elbow=13, L wrist=15 | R shoulder=12, R elbow=14, R wrist=16
public class ProceduralHand : MonoBehaviour
{
    [Header("Settings")]
    public float handDepth = 1.5f;
    public float smoothHalfLife = 0.04f;
    public float fastHalfLife = 0.025f;

    [Header("Appearance")]
    public Color skinColor = new Color(1f, 0.75f, 0.6f);
    public Color jointColor = new Color(1f, 0.75f, 0.6f);

    [Header("Thickness")]
    public float armWidth = 0.045f;
    public float wristWidth = 0.035f;

    // Pose landmark indices สำหรับ arm
    // [0]=left arm, [1]=right arm; แต่ละ arm: [shoulder, elbow, wrist]
    private static readonly int[][] armIndices = {
        new int[]{ 11, 13, 15 },  // left
        new int[]{ 12, 14, 16 },  // right
    };
    // bones ต่อ arm: shoulder→elbow, elbow→wrist
    private static readonly int[][] armBones = {
        new int[]{ 0, 1 },
        new int[]{ 1, 2 },
    };

    private ArmData leftArm;
    private ArmData rightArm;
    private PoseLandmarkerResult latestResult;
    private bool hasResult = false;

    void Start()
    {
        leftArm  = CreateArm("Left");
        rightArm = CreateArm("Right");
        PoseLandmarkerRunner.OnPoseLandmarkResult += OnReceiveResult;
    }

    void OnDestroy()
    {
        PoseLandmarkerRunner.OnPoseLandmarkResult -= OnReceiveResult;
    }

    void OnReceiveResult(PoseLandmarkerResult result)
    {
        latestResult = CprTrackingStabilizer.HasStableResult
            ? CprTrackingStabilizer.StableResult
            : result;
        hasResult = CprTrackingStabilizer.HasStableResult
            || (result.poseLandmarks != null && result.poseLandmarks.Count > 0);
    }

    ArmData CreateArm(string side)
    {
        var data = new ArmData();
        var root = new GameObject(side + "_ProceduralArm");
        root.transform.parent = transform;

        data.positions = new Vector3[3]; // shoulder, elbow, wrist

        var skinMat = CreateMat(skinColor);
        var jointMat = CreateMat(jointColor);

        data.joints = new GameObject[3];
        string[] jointNames = { "Shoulder", "Elbow", "Wrist" };
        float[] sizes = { armWidth * 1.8f, armWidth * 1.6f, wristWidth * 1.8f };

        for (int i = 0; i < 3; i++)
        {
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.name = side + "_" + jointNames[i];
            s.transform.parent = root.transform;
            s.transform.localScale = Vector3.one * sizes[i];
            s.GetComponent<Renderer>().material = jointMat;
            s.SetActive(false);
            s.layer = LayerMask.NameToLayer("Hand");

            var rb = s.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            if (i == 2) // wrist = collider สำหรับ interaction
            {
                s.GetComponent<SphereCollider>().radius = 0.5f;
                s.tag = "Hand";
            }
            else
            {
                Destroy(s.GetComponent<SphereCollider>());
            }

            data.joints[i] = s;
        }

        data.capsules = new GameObject[armBones.Length];
        for (int i = 0; i < armBones.Length; i++)
        {
            var c = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            c.name = side + "_ArmBone_" + i;
            c.transform.parent = root.transform;
            c.GetComponent<Renderer>().material = skinMat;
            c.layer = LayerMask.NameToLayer("Hand");
            Destroy(c.GetComponent<CapsuleCollider>());
            c.SetActive(false);
            data.capsules[i] = c;
        }

        data.root = root;
        return data;
    }

    Material CreateMat(Color color)
    {
        var shader = Shader.Find("Shader Graphs/ToonShade");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
        var mat = new Material(shader);
        mat.color = color;
        return mat;
    }

    void Update()
    {
        SetArmActive(leftArm, false);
        SetArmActive(rightArm, false);

        if (!hasResult || latestResult.poseLandmarks == null) return;
        if (latestResult.poseLandmarks.Count == 0) return;

        var landmarks = latestResult.poseLandmarks[0].landmarks;
        if (landmarks.Count < 17) return;

        UpdateArm(leftArm,  armIndices[0], landmarks);
        UpdateArm(rightArm, armIndices[1], landmarks);
    }

    void UpdateArm(ArmData arm, int[] poseIdx, System.Collections.Generic.IList<Mediapipe.Tasks.Components.Containers.NormalizedLandmark> landmarks)
    {
        SetArmActive(arm, true);

        for (int i = 0; i < 3; i++)
        {
            var lm = landmarks[poseIdx[i]];
            Vector3 target = Camera.main.ViewportToWorldPoint(
                new Vector3(lm.x, 1f - lm.y, handDepth));

            arm.positions[i] = CprTrackingStabilizer.SmoothTowardAdaptive(
                arm.positions[i], target,
                smoothHalfLife, fastHalfLife,
                CprTrackingStabilizer.VelocityThresholdDefault,
                Time.deltaTime);

            var rb = arm.joints[i].GetComponent<Rigidbody>();
            if (rb != null) rb.MovePosition(arm.positions[i]);
            else arm.joints[i].transform.position = arm.positions[i];
        }

        for (int i = 0; i < armBones.Length; i++)
        {
            UpdateCapsule(arm.capsules[i], arm.positions[armBones[i][0]], arm.positions[armBones[i][1]], armWidth);
        }
    }

    void UpdateCapsule(GameObject cap, Vector3 a, Vector3 b, float width)
    {
        cap.transform.position = (a + b) * 0.5f;
        Vector3 dir = b - a;
        float len = dir.magnitude;
        if (len > 0.0001f)
            cap.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        cap.transform.localScale = new Vector3(width * 2f, len * 0.5f, width * 2f);
    }

    void SetArmActive(ArmData arm, bool active)
    {
        foreach (var j in arm.joints)
            if (j) j.SetActive(active);
        foreach (var c in arm.capsules)
            if (c) c.SetActive(active);
    }

    class ArmData
    {
        public GameObject root;
        public GameObject[] joints;   // [0]=shoulder, [1]=elbow, [2]=wrist
        public GameObject[] capsules; // [0]=shoulder→elbow, [1]=elbow→wrist
        public Vector3[] positions;
    }
}