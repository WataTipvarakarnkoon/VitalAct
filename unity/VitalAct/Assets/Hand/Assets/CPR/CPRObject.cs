using UnityEngine;

public class CPRObject : MonoBehaviour
{
    public Transform chestBone;

    [Header("Settings")]
    public float pressDepth = 0.05f;
    public float speed = 15f;

    private Vector3 originalPos;
    private float current = 0f;
    private float target = 0f;

    void Start()
    {
        if (chestBone != null)
            originalPos = chestBone.localPosition;
    }

    public void ApplyPress(float value)
    {
        target = Mathf.Clamp01(value);
    }

    public void ReleasePress()
    {
        target = 0f;
    }

    void Update()
    {
        current = Mathf.Lerp(current, target, Time.deltaTime * speed);
        if (chestBone != null)
            chestBone.localPosition = originalPos + new Vector3(0, -current * pressDepth, 0);
    }
}