using UnityEngine;

public class BleedingHandInput : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private BleedingSystem bleedingSystem;

    [Header("External Input")]
    [SerializeField] private bool preferExternalInput = true;
    [SerializeField] private float externalInputTimeout = 0.2f;

    [Header("Manual Test Input")]
    [SerializeField] private bool enableManualTestInput = true;
    [SerializeField] private bool useMouseButton = true;
    [SerializeField] private int mouseButton = 0;
    [SerializeField] private KeyCode pressKey = KeyCode.Space;
    [SerializeField] private KeyCode depthUpKey = KeyCode.E;
    [SerializeField] private KeyCode depthDownKey = KeyCode.Q;
    [SerializeField] private float defaultPressDepth = 0.6f;
    [SerializeField] private float depthAdjustSpeed = 1.2f;
    [SerializeField] private float depthReturnSpeed = 2.5f;
    [SerializeField] private float scrollWheelSensitivity = 0.08f;

    [Header("Runtime")]
    [Range(0f, 1f)] [SerializeField] private float manualDepth = 0.6f;

    private bool _zonePressing;
    private float _zoneDepth = 0.6f;

    private bool _externalPressing;
    private float _externalDepth;
    private float _lastExternalUpdate = -100f;

    public void SetExternalInput(bool isPressing, float pressDepth)
    {
        _externalPressing = isPressing;
        _externalDepth = Mathf.Clamp01(pressDepth);
        _lastExternalUpdate = Time.time;
    }

    public void SetExternalPressing(bool isPressing)
    {
        SetExternalInput(isPressing, _externalDepth);
    }

    public void SetExternalDepth(float pressDepth)
    {
        SetExternalInput(_externalPressing, pressDepth);
    }

    public void BeginZonePress(float pressDepth)
    {
        _zonePressing = true;
        _zoneDepth = Mathf.Clamp01(pressDepth);
        manualDepth = Mathf.Clamp01(Mathf.Max(manualDepth, _zoneDepth));
    }

    public void UpdateZoneDepth(float pressDepth)
    {
        _zoneDepth = Mathf.Clamp01(pressDepth);
    }

    public void EndZonePress()
    {
        _zonePressing = false;
    }

    public void SetManualDepth(float pressDepth)
    {
        manualDepth = Mathf.Clamp01(pressDepth);
    }

    private void Reset()
    {
        bleedingSystem = GetComponent<BleedingSystem>();
    }

    private void Update()
    {
        if (bleedingSystem == null)
        {
            return;
        }

        bool useExternal = preferExternalInput && Time.time - _lastExternalUpdate <= externalInputTimeout;
        bool isPressing;
        float pressDepth;

        if (useExternal)
        {
            isPressing = _externalPressing;
            pressDepth = isPressing ? _externalDepth : 0f;
        }
        else
        {
            isPressing = GetManualPressing();
            pressDepth = UpdateManualDepth(isPressing);
        }

        if (isPressing && bleedingSystem.CurrentState == BleedingGameState.Prepare)
        {
            bleedingSystem.BeginScenario();
        }

        bleedingSystem.SetInput(isPressing, pressDepth);
    }

    private bool GetManualPressing()
    {
        if (!enableManualTestInput)
        {
            return _zonePressing;
        }

        bool keyPress = Input.GetKey(pressKey);
        bool mousePress = useMouseButton && Input.GetMouseButton(mouseButton);
        return keyPress || mousePress || _zonePressing;
    }

    private float UpdateManualDepth(bool isPressing)
    {
        if (!enableManualTestInput && !_zonePressing)
        {
            manualDepth = Mathf.MoveTowards(manualDepth, 0f, Time.deltaTime * depthReturnSpeed);
            return 0f;
        }

        if (Input.GetKey(depthUpKey))
        {
            manualDepth += depthAdjustSpeed * Time.deltaTime;
        }

        if (Input.GetKey(depthDownKey))
        {
            manualDepth -= depthAdjustSpeed * Time.deltaTime;
        }

        float wheel = Input.mouseScrollDelta.y;
        if (Mathf.Abs(wheel) > 0.001f)
        {
            manualDepth += wheel * scrollWheelSensitivity;
        }

        if (_zonePressing)
        {
            manualDepth = Mathf.MoveTowards(manualDepth, _zoneDepth, Time.deltaTime * depthAdjustSpeed * 2f);
        }
        else if (isPressing)
        {
            manualDepth = Mathf.MoveTowards(manualDepth, defaultPressDepth, Time.deltaTime * depthReturnSpeed);
        }
        else
        {
            manualDepth = Mathf.MoveTowards(manualDepth, 0f, Time.deltaTime * depthReturnSpeed);
        }

        manualDepth = Mathf.Clamp01(manualDepth);
        return isPressing ? manualDepth : 0f;
    }
}
