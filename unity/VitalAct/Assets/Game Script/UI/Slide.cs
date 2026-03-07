using System;
using UnityEngine;

public class ChecklistUI : MonoBehaviour
{
    public Animator animator;
    bool isOpen = false;
    public bool IsOpen => isOpen;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
          Debug.Log("Tab");
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
}