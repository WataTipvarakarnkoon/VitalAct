using UnityEngine;
using UnityEngine.UI;

public class ToggleColorChange : MonoBehaviour
{
    private Toggle toggle;
    private Image backgroundImage;
    
    public Color activeColor = new Color(0.3f, 0.3f, 0.3f);
    public Color inactiveColor = Color.white;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        backgroundImage = GetComponentInChildren<Image>();
        
        if (toggle == null)
        {
            Debug.LogError("No Toggle component found on " + gameObject.name);
            return;
        }
        
        if (backgroundImage == null)
        {
            Debug.LogError("No Image component found in children of " + gameObject.name);
            return;
        }
        
        toggle.onValueChanged.AddListener(OnToggleChanged);
        
        OnToggleChanged(toggle.isOn);
        
        Debug.Log("ToggleColorChange initialized on " + gameObject.name);
    }

    void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            backgroundImage.color = activeColor;
            Debug.Log(gameObject.name + " is ON - color changed to dark gray");
        }
        else
        {
            backgroundImage.color = inactiveColor;
            Debug.Log(gameObject.name + " is OFF - color changed to white");
        }
    }
}