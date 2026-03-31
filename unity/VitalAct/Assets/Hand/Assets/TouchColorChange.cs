using UnityEngine;

public class TouchColorChange : MonoBehaviour
{
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color touchColor = Color.red;

    [Header("Settings")]
    public float colorSpeed = 5f;
    public float cooldown = 0.5f; // กันกดรัว (วินาที)

    private Renderer rend;
    private bool isOn = false;
    private float lastTouchTime = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = normalColor;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Hand") && Time.time - lastTouchTime > cooldown)
        {
            isOn = !isOn; // สลับสถานะ
            lastTouchTime = Time.time;
        }
    }

    void Update()
    {
        Color target = isOn ? touchColor : normalColor;

        rend.material.color = Color.Lerp(
            rend.material.color,
            target,
            Time.deltaTime * colorSpeed
        );
    }
}