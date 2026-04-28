using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;

// แสดง arm skeleton capsule: shoulder→elbow→wrist จาก Pose landmark
// Left:  shoulder=11, elbow=13, wrist=15
// Right: shoulder=12, elbow=14, wrist=16
public class CapsuleHandVisualizer : MonoBehaviour
{
    public float handDepth = 2f;
    public float smoothSpeed = 25f;
    public float armRadius = 0.018f;

    // [arm][bone]: pose landmark index คู่ a→b
    private static readonly int[][][] armBones = {
        new int[][]{ new int[]{11,13}, new int[]{13,15} }, // left
        new int[][]{ new int[]{12,14}, new int[]{14,16} }, // right
    };

    private List<CapsuleBone> leftBones  = new List<CapsuleBone>();
    private List<CapsuleBone> rightBones = new List<CapsuleBone>();

    // positions: [left/right][shoulder=0, elbow=1, wrist=2]
    private Vector3[][] positions = { new Vector3[3], new Vector3[3] };

    private PoseLandmarkerResult latestResult;
    private bool hasResult = false;

    void Start()
    {
        leftBones  = CreateArmBones("Left",  new Color(0.9f, 0.75f, 0.65f));
        rightBones = CreateArmBones("Right", new Color(0.9f, 0.75f, 0.65f));

        SetArmActive(leftBones,  false);
        SetArmActive(rightBones, false);

        PoseLandmarkerRunner.OnPoseLandmarkResult += OnReceiveResult;
    }

    List<CapsuleBone> CreateArmBones(string side, Color color)
    {
        var boneList = new List<CapsuleBone>();
        var parent = new GameObject(side + "_Arm");
        parent.transform.parent = this.transform;

        var mat = new Material(Shader.Find("Standard"));
        mat.color = color;

        for (int i = 0; i < 2; i++) // shoulder→elbow, elbow→wrist
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = side + "_ArmBone_" + i;
            go.transform.parent = parent.transform;
            go.GetComponent<Renderer>().material = mat;
            Destroy(go.GetComponent<CapsuleCollider>());
            go.transform.localScale = new Vector3(armRadius * 2, 0.05f, armRadius * 2);

            boneList.Add(new CapsuleBone { gameObject = go, radius = armRadius });
        }

        return boneList;
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
        SetArmActive(leftBones,  false);
        SetArmActive(rightBones, false);

        if (!hasResult || latestResult.poseLandmarks == null) return;
        if (latestResult.poseLandmarks.Count == 0) return;

        var landmarks = latestResult.poseLandmarks[0].landmarks;
        if (landmarks.Count < 17) return;

        // Pose indices: left shoulder=11,elbow=13,wrist=15 | right shoulder=12,elbow=14,wrist=16
        int[][] poseSides = {
            new int[]{ 11, 13, 15 }, // left
            new int[]{ 12, 14, 16 }, // right
        };
        List<CapsuleBone>[] boneSides = { leftBones, rightBones };

        for (int s = 0; s < 2; s++)
        {
            // อัพเดท 3 positions: shoulder, elbow, wrist
            for (int j = 0; j < 3; j++)
            {
                var lm = landmarks[poseSides[s][j]];
                Vector3 target = Camera.main.ViewportToWorldPoint(
                    new Vector3(lm.x, 1f - lm.y, handDepth));
                positions[s][j] = Vector3.Lerp(positions[s][j], target, Time.deltaTime * smoothSpeed);
            }

            SetArmActive(boneSides[s], true);
            for (int i = 0; i < 2; i++) // bone 0: shoulder→elbow, bone 1: elbow→wrist
                UpdateCapsule(boneSides[s][i], positions[s][i], positions[s][i + 1]);
        }
    }

    void UpdateCapsule(CapsuleBone bone, Vector3 start, Vector3 end)
    {
        Vector3 mid = (start + end) / 2f;
        float length = Vector3.Distance(start, end);

        bone.gameObject.transform.position = mid;

        // หมุน capsule ให้ชี้จาก start → end
        Vector3 dir = (end - start).normalized;
        if (dir != Vector3.zero)
        {
            bone.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        }

        // ปรับความยาว
        float diameter = bone.radius * 2;
        bone.gameObject.transform.localScale = new Vector3(
            diameter,
            length / 2f,  // Unity capsule height = scale.y * 2
            diameter
        );
    }

    void SetArmActive(List<CapsuleBone> boneList, bool active)
    {
        foreach (var b in boneList)
            if (b.gameObject != null)
                b.gameObject.SetActive(active);
    }
}

[System.Serializable]
public class CapsuleBone
{
    public GameObject gameObject;
    public float radius;
}