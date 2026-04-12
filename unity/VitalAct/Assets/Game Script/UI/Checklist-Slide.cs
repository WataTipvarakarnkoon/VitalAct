using System;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistUI : MonoBehaviour
{
    public Animator animator;
    public Button button;
    private CanvasGroup canvasGroup;
    bool isOpen = false;
    public bool IsOpen => isOpen;

  void Start()
  {
    canvasGroup = GetComponent<CanvasGroup>();
    if (canvasGroup != null)
    {
      canvasGroup.interactable = false;
      canvasGroup.blocksRaycasts = false;
    }
    button.onClick.AddListener(() =>
    {  if(!isOpen || GameManager.instance.CurrentState == GameManager.GameState.Identify)
    {
      Toggle();
    }
    else if (isOpen)
    {
        Toggle();
    }
    });
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Tab) && GameManager.instance.CurrentState == GameManager.GameState.Identify)
    {
      Toggle();
    }
    else if(isOpen && GameManager.instance.CurrentState == GameManager.GameState.Choose)
    {
      Toggle();
    }
  }
  void Toggle()
    {
      isOpen = !isOpen;
      if (isOpen)
      {
        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        animator.SetTrigger("SlideUP");
      }
      else
      {
        if (canvasGroup != null)
        {
          canvasGroup.interactable = false;
          canvasGroup.blocksRaycasts = false;
        }
        animator.SetTrigger("SlideDown");
      }

    }
}