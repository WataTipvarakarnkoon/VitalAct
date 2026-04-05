using System;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistUI : MonoBehaviour
{
    GameManager gameManager;
    public Animator animator;
    public Button button;
    bool isOpen = false;
    public bool IsOpen => isOpen;

  void Start()
  {
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
  }
  void Toggle()
    {
      isOpen = !isOpen;
      if (isOpen)
      {
        animator.SetTrigger("SlideUP");
      }
      else
      {
        animator.SetTrigger("SlideDown");
      }

    }
}