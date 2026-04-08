using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerFill : MonoBehaviour
{
    public Image          fill;
    public CprGameManager gameManager;
    public GameObject resultUI;

    public float duration;
    public float timer;
    bool  running;

    public CanvasGroup resultCanvas;
    public float fadeDuration = 1f;
    public IEnumerator Fade(bool show)
    {
        float start = resultCanvas.alpha;
        float end = show ? 1f : 0f;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            resultCanvas.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            yield return null;
        }

        resultCanvas.alpha = end;
        resultCanvas.interactable = show;
        resultCanvas.blocksRaycasts = show;
    }
    void Start()
    {
        if (gameManager == null) gameManager = FindObjectOfType<CprGameManager>();

        timer = duration;
        if (fill != null) fill.fillAmount = 1f;
    }

    void OnEnable()  => CprGameManager.OnStateChanged += OnStateChanged;
    void OnDisable() => CprGameManager.OnStateChanged -= OnStateChanged;

    void OnStateChanged(CprGameState state)
    {
        if (state == CprGameState.Playing)
        {
            ResetTimer();
            running = true;
        }
        else
        {
            running = false;
        }
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
            gameManager?.EndSession();
            Result();
        }
    }

    public void ResetTimer()
    {
        timer = duration;
        if (fill != null) fill.fillAmount = 1f;
    }

    void Result()
    {
        if(resultUI != null)
        {   
            
            StartCoroutine(Fade(true));
        }
    }
}
