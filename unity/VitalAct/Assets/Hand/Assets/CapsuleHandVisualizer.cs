using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class CapsuleHandVisualizer : MonoBehaviour
{
    public float handDepth = 2f;
    public float smoothSpeed = 25f;
    public float fingerRadius = 0.012f;  // ความหนานิ้ว
    public float palmRadius = 0.018f;    // ความหนาฝ่ามือ

    // เส้นเชื่อมระหว่าง landmark (bone connections)
    private static readonly int[][] bones = {
        // ฝ่ามือ
        new int[]{0, 1}, new int[]{0, 5}, new int[]{0, 17},
        new int[]{5, 9}, new int[]{9, 13}, new int[]{13, 17},
        // นิ้วหัวแม่มือ
        new int[]{1, 2}, new int[]{2, 3}, new int[]{3, 4},
        // นิ้วชี้
        new int[]{5, 6}, new int[]{6, 7}, new int[]{7, 8},
        // นิ้วกลาง
        new int[]{9, 10}, new int[]{10, 11}, new int[]{11, 12},
        // นิ้วนาง
        new int[]{13, 14}, new int[]{14, 15}, new int[]{15, 16},
        // นิ้วก้อย
        new int[]{17, 18}, new int[]{18, 19}, new int[]{19, 20}
    };

    private List<CapsuleBone> leftBones = new List<CapsuleBone>();
    private List<CapsuleBone> rightBones = new List<CapsuleBone>();
    private List<Vector3> leftPositions = new List<Vector3>();
    private List<Vector3> rightPositions = new List<Vector3>();

    private HandLandmarkerResult latestResult;
    private bool hasResult = false;

    void Start()
    {
        // เตรียม positions 21 จุดต่อมือ
        for (int i = 0; i < 21; i++)
        {
            leftPositions.Add(Vector3.zero);
            rightPositions.Add(Vector3.zero);
        }

        leftBones = CreateHandBones("Left", new Color(0.9f, 0.75f, 0.65f));
        rightBones = CreateHandBones("Right", new Color(0.9f, 0.75f, 0.65f));

        SetHandActive(leftBones, false);
        SetHandActive(rightBones, false);

        HandLandmarkerRunner.OnHandLandmarkResult += OnReceiveResult;
    }

    List<CapsuleBone> CreateHandBones(string handName, Color color)
    {
        var boneList = new List<CapsuleBone>();
        var parent = new GameObject(handName + "_Hand");
        parent.transform.parent = this.transform;

        var mat = new Material(Shader.Find("Standard"));
        mat.color = color;

        for (int i = 0; i < bones.Length; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = handName + "_Bone_" + i;
            go.transform.parent = parent.transform;
            go.GetComponent<Renderer>().material = mat;
            Destroy(go.GetComponent<CapsuleCollider>());

            // palm bone หนากว่า finger bone
            bool isPalm = i < 6;
            float radius = isPalm ? palmRadius : fingerRadius;
            go.transform.localScale = new Vector3(radius * 2, 0.05f, radius * 2);

            boneList.Add(new CapsuleBone { gameObject = go, radius = radius });
        }

        return boneList;
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
        SetHandActive(leftBones, false);
        SetHandActive(rightBones, false);

        if (!hasResult || latestResult.handLandmarks == null) return;

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

            var positions = isLeft ? leftPositions : rightPositions;
            var boneList = isLeft ? leftBones : rightBones;

            if (isLeft) foundLeft = true;
            else foundRight = true;

            // อัพเดท positions ทุก landmark
            for (int i = 0; i < 21; i++)
            {
                var lm = landmarks[i];
                Vector3 target = Camera.main.ViewportToWorldPoint(
                    new Vector3(lm.x, 1f - lm.y, handDepth)
                );
                positions[i] = Vector3.Lerp(positions[i], target, Time.deltaTime * smoothSpeed);
            }

            // อัพเดท capsule แต่ละ bone
            SetHandActive(boneList, true);
            for (int i = 0; i < bones.Length; i++)
            {
                Vector3 start = positions[bones[i][0]];
                Vector3 end = positions[bones[i][1]];
                UpdateCapsule(boneList[i], start, end);
            }
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

    void SetHandActive(List<CapsuleBone> boneList, bool active)
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