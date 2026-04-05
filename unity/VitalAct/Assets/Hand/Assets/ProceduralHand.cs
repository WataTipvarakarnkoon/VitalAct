using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class ProceduralHand : MonoBehaviour
{
    [Header("Settings")]
    public float handDepth = 1.5f;
    public float smoothSpeed = 15f;

    [Header("Appearance")]
    public Color skinColor = new Color(1f, 0.75f, 0.6f);
    public Color jointColor = new Color(1f, 0.75f, 0.6f);

    [Header("Thickness")]
    public float thumbWidth = 0.035f;
    public float fingerWidth = 0.025f;
    public float tipWidth = 0.018f;
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

    private HandData leftHand;
    private HandData rightHand;
    private HandLandmarkerResult latestResult;
    private bool hasResult = false;

    void Start()
    {
        leftHand = CreateHand("Left");
        rightHand = CreateHand("Right");
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

    HandData CreateHand(string side)
    {
        var data = new HandData();
        var root = new GameObject(side + "_ProceduralHand");
        root.transform.parent = transform;

        data.positions = new Vector3[21];

        var skinMat = CreateMat(skinColor);
        var jointMat = CreateMat(jointColor);

        data.joints = new GameObject[21];
        for (int i = 0; i < 21; i++)
        {
            bool isTip = (i == 4 || i == 8 || i == 12 || i == 16 || i == 20);
            bool isThumb = (i >= 1 && i <= 4);
            bool isPalm = (i == 0);

            float size;
            if (isPalm) size = palmWidth * 1.8f;
            else if (isThumb) size = thumbWidth * 1.6f;
            else if (isTip) size = tipWidth * 1.6f;
            else size = fingerWidth * 1.6f;

            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.name = side + "_Joint_" + i;
            s.transform.parent = root.transform;
            s.transform.localScale = Vector3.one * size;
            s.GetComponent<Renderer>().material = jointMat;
            s.SetActive(false);
            s.layer = LayerMask.NameToLayer("Hand");

            var rb = s.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            if (isTip)
            {
                var col = s.GetComponent<SphereCollider>();
                col.radius = 0.5f;
            }
            else
            {
                Destroy(s.GetComponent<SphereCollider>());
            }

            s.tag = "Hand";
            data.joints[i] = s;
        }

        data.capsules = new GameObject[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            var c = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            c.name = side + "_Bone_" + i;
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
        SetHandActive(leftHand, false);
        SetHandActive(rightHand, false);

        if (!hasResult || latestResult.handLandmarks == null) return;

        for (int h = 0; h < latestResult.handLandmarks.Count; h++)
        {
            bool isLeft = false;
            if (latestResult.handedness != null && h < latestResult.handedness.Count)
                isLeft = latestResult.handedness[h].categories[0].categoryName == "Left";

            var hand = isLeft ? leftHand : rightHand;
            var landmarks = latestResult.handLandmarks[h].landmarks;

            if (landmarks == null || landmarks.Count < 21) continue;

            SetHandActive(hand, true);

            for (int i = 0; i < 21; i++)
            {
                var lm = landmarks[i];
                Vector3 target = Camera.main.ViewportToWorldPoint(
                    new Vector3(lm.x, 1f - lm.y, handDepth));

                hand.positions[i] = Vector3.Lerp(
                    hand.positions[i], target, Time.deltaTime * smoothSpeed);

                var rb = hand.joints[i].GetComponent<Rigidbody>();
                if (rb != null)
                    rb.MovePosition(hand.positions[i]);
                else
                    hand.joints[i].transform.position = hand.positions[i];
            }

            for (int i = 0; i < bones.Length; i++)
            {
                int a = bones[i][0];
                int b = bones[i][1];
                if (a >= 21 || b >= 21) continue;

                float width = bones[i][2] switch
                {
                    0 => palmWidth,
                    1 => thumbWidth,
                    3 => tipWidth,
                    _ => fingerWidth
                };

                UpdateCapsule(
                    hand.capsules[i],
                    hand.positions[a],
                    hand.positions[b],
                    width);
            }
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

    void SetHandActive(HandData hand, bool active)
    {
        foreach (var j in hand.joints)
            if (j) j.SetActive(active);
        foreach (var c in hand.capsules)
            if (c) c.SetActive(active);
    }

    class HandData
    {
        public GameObject root;
        public GameObject[] joints;
        public GameObject[] capsules;
        public Vector3[] positions;
    }
}