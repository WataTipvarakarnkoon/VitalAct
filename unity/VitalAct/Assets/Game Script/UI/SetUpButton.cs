using UnityEngine;
using UnityEngine.UI;

public class SetUpButton : MonoBehaviour
{
    public Animator animator;
    public Button buttonPressed;
    public Button[] buttons;
    private bool isOpen = false;

    void Start()
    {   
        buttonPressed.onClick.AddListener(() =>
        {
            OpenButtons();
        });

        foreach(Button btn in buttons)
        {
            btn.onClick.AddListener(OnButtonPressed);
        }
    }

    public void OpenButtons()
    {
        if (!isOpen)
        {
            isOpen = true;
            animator.SetTrigger("ButtonsUP");
        }
    }

    void OnButtonPressed()
    {
        if (isOpen)
        {
            isOpen = false;
            animator.SetTrigger("ButtonsDown");
        }
    }
}