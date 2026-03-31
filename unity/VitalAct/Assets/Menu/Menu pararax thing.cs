using UnityEngine;

public class ParallaxCamera3D : MonoBehaviour
{
    public float rotationMulti = 10f;
    public float smoothTime = 0.3f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
{
    Vector2 offset = Vector2.zero;

    #if UNITY_EDITOR || UNITY_STANDALONE
    if (Camera.main != null)
        offset = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition) - new Vector2(0.5f, 0.5f);

    #else
    if (SystemInfo.supportsGyroscope)
    {
        Input.gyro.enabled = true;
        offset = new Vector2(Input.gyro.rotationRateUnbiased.y, Input.gyro.rotationRateUnbiased.x) * 0.1f;
    }
    else if (Input.touchCount > 0)
{
    Touch touch = Input.GetTouch(0);

    if (UnityEngine.EventSystems.EventSystem.current != null &&
        UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        return;

    offset = (touch.position / new Vector2(Screen.width, Screen.height)) - new Vector2(0.5f, 0.5f);
}
    #endif

    if (float.IsNaN(offset.x) || float.IsNaN(offset.y) ||
        float.IsInfinity(offset.x) || float.IsInfinity(offset.y))
        return;

    offset.x = Mathf.Clamp(offset.x, -1f, 1f);
    offset.y = Mathf.Clamp(offset.y, -1f, 1f);

    Quaternion targetRotation = startRotation * Quaternion.Euler(-offset.y * rotationMulti, offset.x * rotationMulti, 0f);
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime);
}
}