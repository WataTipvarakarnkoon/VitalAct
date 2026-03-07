using UnityEngine;

public class TimerBar : MonoBehaviour
{
    public RectTransform fill;
    public float duration = 5f;

    float timer;
    float startWidth;

    void Start()
    {
        startWidth = fill.sizeDelta.x;
    }

    void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(1 - timer / duration);

        fill.sizeDelta = new Vector2(startWidth * t, fill.sizeDelta.y);
    }
}