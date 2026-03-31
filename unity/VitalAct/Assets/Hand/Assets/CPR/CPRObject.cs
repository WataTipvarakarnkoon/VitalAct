using UnityEngine;

public class CPRObject : MonoBehaviour
{
    [Header("CPR Settings")]
    public float pressDepth = 0.3f;
    public float springForce = 8f;
    public float damping = 4f;

    [Header("Feedback")]
    public Color normalColor = Color.white;
    public Color pressColor = new Color(1f, 0.3f, 0.3f);

    private Vector3 originalScale;
    private Vector3 originalPos;
    private float currentDepth = 0f;
    private float velocity = 0f;
    private float targetDepth = 0f;
    private Renderer rend;

    void Start()
    {
        originalScale = transform.localScale;
        originalPos = transform.position;
        rend = GetComponent<Renderer>();

        if (rend) rend.material.color = normalColor;
    }

    // 🔥 ใช้ตำแหน่งตรง ไม่ใช้ delta แล้ว
    public void ApplyPress(float handY)
    {
        float depth = originalPos.y - handY;
        targetDepth = Mathf.Clamp(depth, 0f, pressDepth);
    }

    public void ReleasePress()
    {
        targetDepth = 0f;
    }

    void Update()
    {
        float springAcc = (targetDepth - currentDepth) * springForce
                          - velocity * damping;

        velocity += springAcc * Time.deltaTime;
        currentDepth += velocity * Time.deltaTime;
        currentDepth = Mathf.Clamp(currentDepth, 0f, pressDepth);

        float ratio = currentDepth / pressDepth;

        // scale
        float scaleY = Mathf.Lerp(originalScale.y, originalScale.y * 0.7f, ratio);
        transform.localScale = new Vector3(originalScale.x, scaleY, originalScale.z);

        // position
        transform.position = new Vector3(
            originalPos.x,
            originalPos.y - currentDepth * 0.5f,
            originalPos.z
        );

        // color
        if (rend)
            rend.material.color = Color.Lerp(normalColor, pressColor, ratio);
    }
}