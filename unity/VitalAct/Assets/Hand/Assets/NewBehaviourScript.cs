using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PalmMesh : MonoBehaviour
{
    public Color skinColor = new Color(0.9f, 0.72f, 0.62f);
    private Mesh mesh;
    private Vector3[] vertices = new Vector3[7];

    // triangle ของฝ่ามือ (index เทียบกับ vertices array)
    // vertices: 0=wrist, 1=thumb_cmc(1), 2=index_mcp(5),
    //           3=middle_mcp(9), 4=ring_mcp(13),
    //           5=pinky_mcp(17), 6=pinky_base_low
    private static readonly int[] triangles = {
        0, 2, 1,   // wrist → index_base → thumb
        0, 3, 2,   // wrist → middle_base → index_base
        0, 4, 3,   // wrist → ring_base → middle_base
        0, 5, 4,   // wrist → pinky_base → ring_base
    };

    void Awake()
    {
        mesh = new Mesh();
        mesh.name = "PalmMesh";
        GetComponent<MeshFilter>().mesh = mesh;

        var mat = new Material(Shader.Find("Standard"));
        mat.color = skinColor;
        mat.SetFloat("_Glossiness", 0.3f);
        mat.SetFloat("_Metallic", 0.1f);
        GetComponent<MeshRenderer>().material = mat;
    }

    // เรียก method นี้ทุก frame จาก ProceduralHand
    public void UpdatePalm(Vector3 wrist, Vector3 thumb,
                           Vector3 index, Vector3 middle,
                           Vector3 ring, Vector3 pinky)
    {
        vertices[0] = transform.InverseTransformPoint(wrist);
        vertices[1] = transform.InverseTransformPoint(thumb);
        vertices[2] = transform.InverseTransformPoint(index);
        vertices[3] = transform.InverseTransformPoint(middle);
        vertices[4] = transform.InverseTransformPoint(ring);
        vertices[5] = transform.InverseTransformPoint(pinky);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}