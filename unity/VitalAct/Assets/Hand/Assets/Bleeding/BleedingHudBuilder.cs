using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class BleedingHudBuilder : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private BleedingSystem bleedingSystem;
    [SerializeField] private BleedingUI bleedingUI;
    [SerializeField] private BleedingHandDetector handDetector;
    [SerializeField] private RawImage cameraPreview;

    [Header("Build")]
    [SerializeField] private bool rebuildOnStart;
    [SerializeField] private string rootName = "BleedingHUD_Auto";

    private void Reset()
    {
        targetCanvas = GetComponent<Canvas>();
        bleedingUI = GetComponent<BleedingUI>();
    }

    private void Start()
    {
        if (Application.isPlaying && rebuildOnStart)
        {
            BuildHud();
        }
    }

    [ContextMenu("Build Bleeding HUD")]
    public void BuildHud()
    {
        if (targetCanvas == null)
        {
            targetCanvas = GetComponent<Canvas>();
        }

        if (targetCanvas == null)
        {
            Debug.LogWarning("BleedingHudBuilder requires a Canvas.");
            return;
        }

        if (bleedingUI == null)
        {
            bleedingUI = GetComponent<BleedingUI>();
        }

        if (bleedingUI == null)
        {
            bleedingUI = GetComponentInChildren<BleedingUI>(true);
        }

        if (bleedingUI == null)
        {
            bleedingUI = targetCanvas.GetComponent<BleedingUI>();
        }

        if (bleedingUI == null)
        {
            bleedingUI = targetCanvas.GetComponentInChildren<BleedingUI>(true);
        }

        if (bleedingUI == null)
        {
            bleedingUI = targetCanvas.gameObject.AddComponent<BleedingUI>();
        }

        Transform oldRoot = targetCanvas.transform.Find(rootName);
        if (oldRoot != null)
        {
            if (Application.isPlaying)
            {
                Destroy(oldRoot.gameObject);
            }
            else
            {
                DestroyImmediate(oldRoot.gameObject);
            }
        }

        Sprite sprite = null;
        TMP_FontAsset font = TMP_Settings.defaultFontAsset;

        RectTransform root = CreatePanel(rootName, targetCanvas.transform, sprite, new Color(0f, 0f, 0f, 0f));
        Stretch(root);

        Image stressOverlay = null;
        Image topFill = null;

        TMP_Text objective = null;

        Image depthTrack = CreateRoundedBar("DepthTrack", root, sprite, new Vector2(0.03f, 0.16f), new Vector2(0.05f, 0.38f), new Color(1f, 1f, 1f, 0.95f));
        Image depthFill = CreateRoundedBar("DepthFill", depthTrack.rectTransform, sprite, new Vector2(0.14f, 0.02f), new Vector2(0.86f, 0.98f), new Color(0.12f, 0.72f, 1f, 1f));
        depthFill.type = Image.Type.Filled;
        depthFill.fillMethod = Image.FillMethod.Vertical;
        depthFill.fillOrigin = (int)Image.OriginVertical.Bottom;

        TMP_Text depthLabel = CreateText("DepthLabel", root, font, 24, FontStyles.Bold, Color.white, "DEPT");
        SetRect(depthLabel.rectTransform, new Vector2(0.02f, 0.14f), new Vector2(0.13f, 0.2f), Vector2.zero, Vector2.zero);
        depthLabel.alignment = TextAlignmentOptions.Left;
        AddTextOutline(depthLabel, new Color(0f, 0.5f, 0.85f), 0.22f);

        TMP_Text timerText = CreateText("TimerText", root, font, 26, FontStyles.Bold, Color.white, "TIMER: 10s");
        SetRect(timerText.rectTransform, new Vector2(0.03f, 0.06f), new Vector2(0.22f, 0.12f), Vector2.zero, Vector2.zero);
        timerText.alignment = TextAlignmentOptions.Left;
        AddTextOutline(timerText, new Color(0.15f, 0.9f, 0.65f), 0.22f);

        TMP_Text bleedingStateText = CreateText("BleedingStateText", root, font, 24, FontStyles.Bold, Color.white, "BLEEDING: HEAVY");
        SetRect(bleedingStateText.rectTransform, new Vector2(0.03f, 0.12f), new Vector2(0.28f, 0.18f), Vector2.zero, Vector2.zero);
        bleedingStateText.alignment = TextAlignmentOptions.Left;
        AddTextOutline(bleedingStateText, Color.black, 0.18f);

        Image holdFill = null;

        TMP_Text pressureText = CreateText("PressureText", root, font, 30, FontStyles.Bold, new Color(1f, 0.65f, 0.05f), "TOO LIGHT");
        SetRect(pressureText.rectTransform, new Vector2(0.54f, 0.06f), new Vector2(0.72f, 0.13f), Vector2.zero, Vector2.zero);
        pressureText.alignment = TextAlignmentOptions.Left;
        AddTextOutline(pressureText, Color.white, 0.2f);

        TMP_Text warningText = CreateText("WarningText", root, font, 22, FontStyles.Bold, Color.white, "Maintain firm pressure on the wound.");
        SetRect(warningText.rectTransform, new Vector2(0.26f, 0.12f), new Vector2(0.78f, 0.19f), Vector2.zero, Vector2.zero);
        warningText.alignment = TextAlignmentOptions.Center;
        AddTextOutline(warningText, Color.black, 0.18f);

        TMP_Text stabilityText = CreateText("StabilityText", root, font, 18, FontStyles.Normal, Color.white, "Stability 100%   Depth 0%   Min 25%");
        SetRect(stabilityText.rectTransform, new Vector2(0.28f, 0.03f), new Vector2(0.68f, 0.08f), Vector2.zero, Vector2.zero);
        stabilityText.alignment = TextAlignmentOptions.Center;
        AddTextOutline(stabilityText, Color.black, 0.14f);

        TMP_Text cameraGuide = null;

        ApplyReferences(topFill, holdFill, depthFill, objective, bleedingStateText, timerText, pressureText, warningText, stabilityText, cameraGuide, stressOverlay);
    }

    private void ApplyReferences(Image bleedingFill, Image holdFill, Image depthFill, TMP_Text objective, TMP_Text bleedingStateText, TMP_Text timerText, TMP_Text pressureText, TMP_Text warningText, TMP_Text stabilityText, TMP_Text cameraGuide, Image stressOverlay)
    {
        if (bleedingUI != null)
        {
            bleedingUI.ConfigureSimpleHud(
                bleedingSystem,
                handDetector,
                bleedingFill,
                holdFill,
                depthFill,
                objective,
                bleedingStateText,
                pressureText,
                timerText,
                warningText,
                stabilityText,
                cameraGuide,
                stressOverlay);
        }

        if (handDetector != null && cameraPreview != null)
        {
            handDetector.GetType().GetField("cameraPreview", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(handDetector, cameraPreview);
        }
        
        Debug.Log($"Bleeding HUD built on '{targetCanvas.name}'. Timer assigned: {timerText != null}, UI target: {(bleedingUI != null ? bleedingUI.name : "None")}");
    }

    private static RectTransform CreatePanel(string name, Transform parent, Sprite sprite, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = color;
        return go.GetComponent<RectTransform>();
    }

    private static Image CreateImage(string name, Transform parent, Sprite sprite, Color color)
    {
        return CreatePanel(name, parent, sprite, color).GetComponent<Image>();
    }

    private static Image CreateRoundedBar(string name, Transform parent, Sprite sprite, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        RectTransform rect = CreatePanel(name, parent, sprite, color);
        SetRect(rect, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
        return rect.GetComponent<Image>();
    }

    private static TMP_Text CreateText(string name, Transform parent, TMP_FontAsset font, float fontSize, FontStyles styles, Color color, string text)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        TMP_Text tmp = go.GetComponent<TMP_Text>();
        tmp.font = font;
        tmp.fontSize = fontSize;
        tmp.fontStyle = styles;
        tmp.color = color;
        tmp.text = text;
        return tmp;
    }

    private static void AddTextOutline(TMP_Text text, Color color, float thickness)
    {
        text.fontMaterial = new Material(text.fontSharedMaterial);
        text.enableWordWrapping = false;
        text.outlineColor = color;
        text.outlineWidth = thickness;
    }

    private static void Stretch(RectTransform rect)
    {
        SetRect(rect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }
}
