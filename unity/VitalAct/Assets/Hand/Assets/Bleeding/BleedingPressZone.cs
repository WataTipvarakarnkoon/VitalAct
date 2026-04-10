using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BleedingPressZone : MonoBehaviour
{
    [SerializeField] private BleedingHandInput inputBridge;
    [Range(0f, 1f)] [SerializeField] private float defaultZoneDepth = 0.6f;

    [Header("Optional Mouse Drag Depth")]
    [SerializeField] private bool useVerticalMouseDepth = true;
    [Range(0f, 1f)] [SerializeField] private float minDragDepth = 0.2f;
    [Range(0f, 1f)] [SerializeField] private float maxDragDepth = 0.95f;

    private void Reset()
    {
        inputBridge = FindObjectOfType<BleedingHandInput>();
    }

    private void OnMouseDown()
    {
        if (inputBridge == null)
        {
            return;
        }

        inputBridge.BeginZonePress(defaultZoneDepth);
    }

    private void OnMouseDrag()
    {
        if (inputBridge == null || !useVerticalMouseDepth)
        {
            return;
        }

        float normalizedY = Mathf.Clamp01(Input.mousePosition.y / Mathf.Max(Screen.height, 1f));
        float depth = Mathf.Lerp(minDragDepth, maxDragDepth, normalizedY);
        inputBridge.UpdateZoneDepth(depth);
    }

    private void OnMouseUp()
    {
        inputBridge?.EndZonePress();
    }

    private void OnMouseExit()
    {
        if (Input.GetMouseButton(0))
        {
            inputBridge?.EndZonePress();
        }
    }
}
