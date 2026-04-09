
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContinueButton : MonoBehaviour
{
    Button button;
    public GameObject video;
    public CanvasGroup canvasGroup;
    public float       fadeDuration = 0.5f;

  void Start()
  {
    button = GetComponent<Button>();
    button.onClick.AddListener(ToggleObject);
  }
  public void ToggleObject()
  {   
      GameManager.instance.Setup(); 
          if(video != null)
    {
      StartCoroutine(FadeOut());
      GameManager.instance.Choosed();
    } 
  }

  IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f-(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha          = 0f;
        canvasGroup.interactable   = false;
        canvasGroup.blocksRaycasts = false;
        video.SetActive(false);
    }
}