using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;
using UnityEngine;
using UnityEngine.UI;

public class BleedingHandDetector : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private BleedingHandInput inputBridge;

    [Header("Control Zone (0-1, top-left origin)")]
    [SerializeField] private Rect controlZone = new Rect(0f, 0.5f, 1f, 0.5f);

    [Header("Depth Mapping")]
    [SerializeField] private float topDeadZone = 0.08f;
    [SerializeField] private float bottomDeadZone = 0.08f;
    [SerializeField] private float depthCurve = 1.1f;

    [Header("Smoothing")]
    [Range(0.05f, 0.8f)] [SerializeField] private float smoothAlpha = 0.45f;

    [Header("Debug")]
    [SerializeField] private RawImage cameraPreview;
    [SerializeField] private bool showDebugZone = true;
    [SerializeField] private bool showDepthHud = true;

    [Header("Output")]
    [SerializeField] private bool handInZone;
    [Range(0f, 1f)] [SerializeField] private float zoneDepth01;
    [SerializeField] private Vector2 handPoint01;

    public bool HandInZone => handInZone;
    public float ZoneDepth01 => zoneDepth01;
    public Vector2 HandPoint01 => handPoint01;

    private volatile float _pendingRawX;
    private volatile float _pendingRawY;
    private volatile bool _handVisible;
    private volatile bool _hasNewData;

    private float _smoothX;
    private float _smoothY;
    private bool _smoothInit;

    private void OnEnable()
    {
        PoseLandmarkerRunner.OnPoseLandmarkResult += OnPoseLandmarkResult;
    }

    private void OnDisable()
    {
        PoseLandmarkerRunner.OnPoseLandmarkResult -= OnPoseLandmarkResult;
    }

    private void OnPoseLandmarkResult(PoseLandmarkerResult result)
    {
        if (result.poseLandmarks == null || result.poseLandmarks.Count == 0)
        {
            _handVisible = false;
            _hasNewData = true;
            return;
        }

        var landmarks = result.poseLandmarks[0].landmarks;
        // left_wrist(15) + right_wrist(16)
        _pendingRawX = (landmarks[15].x + landmarks[16].x) / 2f;
        _pendingRawY = (landmarks[15].y + landmarks[16].y) / 2f;
        _handVisible = true;
        _hasNewData = true;
    }

    private void Update()
    {
        if (!_hasNewData)
        {
            return;
        }

        _hasNewData = false;

        if (!_handVisible)
        {
            ClearInput();
            return;
        }

        if (!_smoothInit)
        {
            _smoothX = _pendingRawX;
            _smoothY = _pendingRawY;
            _smoothInit = true;
        }

        _smoothX += smoothAlpha * (_pendingRawX - _smoothX);
        _smoothY += smoothAlpha * (_pendingRawY - _smoothY);

        float zoneX = 1f - _smoothX;
        float zoneY = _smoothY;
        handPoint01 = new Vector2(zoneX, zoneY);

        if (!controlZone.Contains(handPoint01))
        {
            ClearInput();
            return;
        }

        handInZone = true;

        float top = controlZone.yMin + controlZone.height * topDeadZone;
        float bottom = controlZone.yMax - controlZone.height * bottomDeadZone;
        float rawDepth = Mathf.InverseLerp(top, bottom, zoneY);
        zoneDepth01 = Mathf.Clamp01(Mathf.Pow(rawDepth, depthCurve));

        if (inputBridge != null)
        {
            inputBridge.SetExternalInput(true, zoneDepth01);
        }
    }

    private void ClearInput()
    {
        handInZone = false;
        zoneDepth01 = Mathf.MoveTowards(zoneDepth01, 0f, Time.deltaTime * 6f);

        if (inputBridge != null)
        {
            inputBridge.SetExternalInput(false, 0f);
        }
    }

    private void OnGUI()
    {
        if (showDebugZone)
        {
            DrawZone();
        }

        if (showDepthHud)
        {
            DrawDepthHud();
        }
    }

    private void DrawZone()
    {
        Rect cameraRect = GetCameraScreenRect();
        if (cameraRect.width <= 0f || cameraRect.height <= 0f)
        {
            return;
        }

        Rect zoneRect = new Rect(
            cameraRect.x + controlZone.x * cameraRect.width,
            cameraRect.y + controlZone.y * cameraRect.height,
            controlZone.width * cameraRect.width,
            controlZone.height * cameraRect.height
        );

        Color fill = handInZone ? new Color(0f, 1f, 0f, 0.18f) : new Color(1f, 0.85f, 0f, 0.12f);
        Color border = handInZone ? Color.green : new Color(1f, 0.85f, 0f);

        GUI.color = fill;
        GUI.DrawTexture(zoneRect, Texture2D.whiteTexture);
        GUI.color = Color.white;
        DrawBorder(zoneRect, 3f, border);

        if (_smoothInit)
        {
            float handX = cameraRect.x + handPoint01.x * cameraRect.width;
            float handY = cameraRect.y + handPoint01.y * cameraRect.height;
            Rect marker = new Rect(handX - 8f, handY - 8f, 16f, 16f);
            GUI.color = handInZone ? Color.green : Color.red;
            GUI.DrawTexture(marker, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(cameraRect.height * 0.055f),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter
        };
        style.normal.textColor = border;

        GUI.Label(
            new Rect(zoneRect.x, zoneRect.y - style.fontSize - 6f, zoneRect.width, style.fontSize + 10f),
            handInZone ? "Move lower to press harder" : "Place hand in lower zone",
            style);
    }

    private void DrawDepthHud()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float barWidth = screenWidth * 0.22f;
        float barHeight = screenHeight * 0.028f;
        float barX = screenWidth * 0.02f;
        float barY = screenHeight * 0.78f;

        GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.85f);
        GUI.DrawTexture(new Rect(barX, barY, barWidth, barHeight), Texture2D.whiteTexture);

        Color fill = zoneDepth01 < 0.25f ? Color.yellow
            : zoneDepth01 > 0.8f ? Color.red
            : Color.green;

        GUI.color = fill;
        GUI.DrawTexture(new Rect(barX, barY, barWidth * zoneDepth01, barHeight), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private Rect GetCameraScreenRect()
    {
        if (cameraPreview != null)
        {
            Vector3[] corners = new Vector3[4];
            cameraPreview.rectTransform.GetWorldCorners(corners);
            float x = corners[0].x;
            float y = Screen.height - corners[2].y;
            float width = corners[2].x - corners[0].x;
            float height = corners[2].y - corners[0].y;
            return new Rect(x, y, width, height);
        }

        return new Rect(0f, 0f, Screen.width, Screen.height);
    }

    private static void DrawBorder(Rect rect, float thickness, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
}
