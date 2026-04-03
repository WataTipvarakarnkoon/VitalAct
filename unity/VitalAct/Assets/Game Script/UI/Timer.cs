using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    public Image fillImage;
    public float duration;

    public float timer;

    void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(1 - timer / duration);

        fillImage.fillAmount = t;
    }
}