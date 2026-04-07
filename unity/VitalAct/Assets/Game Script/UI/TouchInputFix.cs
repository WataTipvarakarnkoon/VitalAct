using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Auto-injects itself onto every scene's EventSystem at runtime.
/// No manual setup required — just keep this script in the project.
///
/// Flutter's AndroidView (Virtual Display / useAndroidViewSurface:false) forwards
/// touches as mouse events. Ensures StandaloneInputModule is active so Unity's
/// EventSystem processes them correctly.
/// </summary>
public class TouchInputFix : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnSceneLoaded()
    {
        SceneManager.sceneLoaded += (scene, mode) => Apply();
        Apply();
    }

    static void Apply()
    {
        var es = FindObjectOfType<EventSystem>();
        if (es == null) return;
        if (es.GetComponent<TouchInputFix>() != null) return;
        es.gameObject.AddComponent<TouchInputFix>();
    }

    void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Remove any existing input modules that don't handle virtual-display
        // mouse events, then ensure StandaloneInputModule is present.
        foreach (var module in GetComponents<BaseInputModule>())
        {
            if (module is StandaloneInputModule) continue;
            Destroy(module);
            Debug.Log($"[TouchInputFix] Removed {module.GetType().Name}");
        }

        if (GetComponent<StandaloneInputModule>() == null)
        {
            gameObject.AddComponent<StandaloneInputModule>();
            Debug.Log("[TouchInputFix] Added StandaloneInputModule");
        }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Debug.Log($"[TouchInputFix] touch at {Input.mousePosition}  screen:{Screen.width}x{Screen.height}");
    }
#endif
}
