using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoCameraToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private TextMeshProUGUI label;

    void Start()
    {
        if (toggle != null)
        {
            toggle.isOn = GameManager.NoCameraMode;
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }
        UpdateLabel();
    }

    void OnToggleChanged(bool value)
    {
        GameManager.SetNoCameraMode(value);
        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (label != null)
            label.text = GameManager.NoCameraMode ? "No Camera (Click Chest)" : "Camera";
    }
}
