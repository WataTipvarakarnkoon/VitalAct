using UnityEngine;
using UnityEngine.UI;

public class CprTimerFill : MonoBehaviour
{
    public Image fill;
    public float duration;
    public float timer;
    bool running;

    void Start()
    {
        ResetTimer();
        running = true;
    }

    void Update()
    {
        if (!running) return;

        timer -= Time.deltaTime;
        if (timer <= 0f) timer = 0f;

        if (fill != null)
            fill.fillAmount = timer / duration;

        if (timer <= 0f)
        {
            running = false;
            GameManager.instance.EndSession();
        }
    }

    public void ResetTimer()
    {
        timer = duration;
        if (fill != null) fill.fillAmount = 1f;
    }
}
