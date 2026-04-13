using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Return : MonoBehaviour
{
    public Button button;
    void Awake()
    {
        if (button != null)
            button.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
    }
}
