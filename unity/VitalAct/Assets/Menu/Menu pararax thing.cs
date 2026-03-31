using UnityEngine;

public class ParallaxCamera3D : MonoBehaviour
{
    public float rotationMulti = 10f;
    public float smoothTime = 0.3f;

    private Quaternion startRotation;
    private Vector3 velocity;
    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {

        Vector2 offset = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition) - new Vector2(0.5f, 0.5f);

        Quaternion targetRotation = startRotation * Quaternion.Euler(-offset.y * rotationMulti, offset.x * rotationMulti, 0f);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime);
    }
}