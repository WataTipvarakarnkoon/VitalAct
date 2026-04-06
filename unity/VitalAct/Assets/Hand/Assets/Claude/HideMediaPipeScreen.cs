using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ซ่อน RawImage ขนาดใหญ่ของ MediaPipe processing screen
/// Attach ที่ GameObject ใดก็ได้ในฉาก
/// </summary>
public class HideMediaPipeScreen : MonoBehaviour
{
    [Tooltip("ชื่อ GameObject ที่มี RawImage ที่อยากซ่อน (ถ้าว่างจะซ่อนอันที่ใหญ่ที่สุด)")]
    public string targetObjectName = "";

    void Start()
    {
        if (!string.IsNullOrEmpty(targetObjectName))
        {
            // ซ่อนตาม ชื่อ
            var go = GameObject.Find(targetObjectName);
            if (go != null)
            {
                go.SetActive(false);
                Debug.Log($"[HideScreen] Hid '{targetObjectName}'");
                return;
            }
            Debug.LogWarning($"[HideScreen] Not found: '{targetObjectName}'");
        }

        // fallback: ซ่อน RawImage ที่ใหญ่ที่สุดในฉาก
        RawImage biggest  = null;
        float    maxArea  = 0f;

        foreach (var img in FindObjectsOfType<RawImage>())
        {
            var rt   = img.rectTransform;
            // ต้องการพื้นที่จริงบนหน้าจอ
            var corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            float w    = Vector3.Distance(corners[0], corners[3]);
            float h    = Vector3.Distance(corners[0], corners[1]);
            float area = w * h;

            if (area > maxArea)
            {
                maxArea  = area;
                biggest  = img;
            }
        }

        if (biggest != null)
        {
            biggest.enabled = false;   // ซ่อน visual แต่ยัง process ได้
            Debug.Log($"[HideScreen] Hid RawImage on '{biggest.gameObject.name}' (largest, area={maxArea:F0})");
        }
    }
}
