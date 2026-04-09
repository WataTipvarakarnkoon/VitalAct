
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChooseButton1 : MonoBehaviour
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
      video.SetActive(true);
      StartCoroutine(FadeIn());
    } 
  }

  IEnumerator FadeIn()
    {
      float t = 0f;
      while (t < fadeDuration)
      {
          t += Time.deltaTime;
          canvasGroup.alpha = t / fadeDuration;
          yield return null;
      }
      canvasGroup.alpha          = 1f;
      canvasGroup.interactable   = true;
      canvasGroup.blocksRaycasts = true;
    }
}