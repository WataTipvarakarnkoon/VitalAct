using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    public Animator animator;
    public Button[] buttons;
    private bool isOpen = false;

    void Start()
    {
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

    void Update()
    {
        if (GameManager.instance.CurrentState == GameManager.GameState.Choose && Input.GetKeyDown(KeyCode.Tab))
        {
            OpenButtons();
        }
    }
}