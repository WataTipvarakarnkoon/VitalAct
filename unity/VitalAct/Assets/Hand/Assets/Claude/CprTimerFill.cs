using UnityEngine;
using UnityEngine.UI;

public class CprTimerFill : MonoBehaviour
{
    public Image fill;
    public float duration;
    public float timer;
    public CprHandDetector handDetector;
    bool running;

    void Start()
    {
        if (handDetector == null) handDetector = FindObjectOfType<CprHandDetector>();
        ResetTimer();
        running = true;
    }

    void Update()
    {
        if (GameManager.instance.CurrentState == GameManager.GameState.SetUp)
        return;

        timer -= Time.deltaTime;
        if (timer <= 0f) timer = 0f;

        if (timer <= 0f)
        {
            if (handDetector != null) handDetector.enabled = false;
            GameManager.instance.EndSession();

        }
        if(timer == 0f && running)
        {
            running = false;
            GameManager.instance.ResultScreen();
        }
        
        if (fill != null)
            fill.fillAmount = timer / duration;
    }

    public void ResetTimer()
    {
        timer = duration;
        if (fill != null) fill.fillAmount = 1f;
    }
}
