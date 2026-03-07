using UnityEngine;

public class Billboard : MonoBehaviour
{
    private float lastYaw = 0f;
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        Vector3 camForward = Camera.main.transform.forward;

        float yaw = Mathf.Atan2(camForward.x, camForward.z) * Mathf.Rad2Deg;
        float pitch = Mathf.Asin(camForward.y) * Mathf.Rad2Deg;

        // Normalize to -180 to 180
        while (yaw > 180f) yaw -= 360f;
        while (yaw < -180f) yaw += 360f;

        // Allow 170–180 range
        if ((yaw >= 170f && yaw <= 180f) || (yaw >= -180f && yaw <= -170f))
        {
            lastYaw = yaw;
        }

        Quaternion targetRotation = Quaternion.Euler(-pitch, lastYaw, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            smoothSpeed * Time.deltaTime
        );
    }
}