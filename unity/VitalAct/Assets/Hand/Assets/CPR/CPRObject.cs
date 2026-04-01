using UnityEngine;

public class CPRObject : MonoBehaviour
{
    [Header("CPR Settings")]
    public float pressDepth = 0.3f;
    public float springForce = 12f;
    public float damping = 6f;

    [Header("Bone Target")]
    public Transform chestBone;

    private Vector3 originalScale;
    private float currentDepth = 0f;
    private float velocity = 0f;
    private float targetDepth = 0f;

    void Start()
    {
        if (chestBone != null)
            originalScale = chestBone.localScale;
    }

    public void ApplyPress(float amount)
    {
        targetDepth = Mathf.Clamp(amount, 0f, pressDepth);
    }

    public void ReleasePress()
    {
        targetDepth = 0f;
    }

    void Update()
    {
        float spring = (targetDepth - currentDepth) * springForce;
        float damp = velocity * damping;
        velocity += (spring - damp) * Time.deltaTime;
        currentDepth += velocity * Time.deltaTime;
        currentDepth = Mathf.Clamp(currentDepth, 0f, pressDepth);

        if (chestBone != null)
        {
            float ratio = currentDepth / pressDepth;
            // ยุบแค่แกน Z (ความหนาของอก) ไม่ขยับ position
            chestBone.localScale = new Vector3(
                originalScale.x,
                originalScale.y,
                Mathf.Lerp(originalScale.z, originalScale.z * 0.7f, ratio)
            );
        }
    }

    public float GetTopY()
    {
        if (chestBone != null)
            return chestBone.position.y;
        return transform.position.y;
    }
}