using UnityEngine;

public class MannequinAnimSync : MonoBehaviour
{
    public CprHandDetector detector;
    public Transform chestBone;      // bone หน้าอกหุ่น
    public float maxPressDown = 0.05f; // เมตร

    Vector3 _chestOrigin;

    void Start() => _chestOrigin = chestBone.localPosition;

    void Update()
    {
        if (!detector.handsOnMannequin) return;
        float d = detector.compressionDepth01;
        chestBone.localPosition = _chestOrigin + Vector3.down * (d * maxPressDown);
    }
}