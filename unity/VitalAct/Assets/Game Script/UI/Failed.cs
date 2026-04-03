using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Failed : MonoBehaviour
{
    public CanvasGroup group;
    public TimerBar timer;
    public float fadeSpeed;
    public bool shouldFade = false;

    // Update is called once per frame
    void Update()
    {
        if(timer.timer >= timer.duration)
        {
           shouldFade = true; 
        }

        if(shouldFade && group.alpha < 1)
        {
            group.alpha += Time.deltaTime * fadeSpeed;
        }
    }
}
