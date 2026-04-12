using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Burst.CompilerServices;

public class Scroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Button button;
    public Animator animator;
    public GameObject hintIncorrect;
    public GameObject hintCorrect;

    [Header("Choices")]
    public Toggle[] correctChoices;
    public Toggle[] incorrectChoices;

    private bool isOpen = false;
    private Coroutine coroutine;

    void Start()
    {
        scrollRect.verticalNormalizedPosition = 1f;

        button.onClick.AddListener(() =>
        {
            if (!isOpen) return;

            bool correct = System.Array.TrueForAll(correctChoices, t => t.isOn);
            bool incorrect = System.Array.Exists(incorrectChoices, t => t.isOn);
            bool hasCorrect = System.Array.Exists(correctChoices, t => t.isOn);
            if (correct && !incorrect) OnCorrect();
            else if(hasCorrect && !incorrect) PartialCorrect();
            else OnIncorrect();
        });
    }

    void OnCorrect()
    {
        Debug.Log("Correct!");
        hintIncorrect.SetActive(false);
        GameManager.instance.Choosed();
        CloseScroll();       
    }

    void OnIncorrect()
    {
        Debug.Log("Incorrect!");
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        } 
        coroutine = StartCoroutine(HideHint());
    }

    void PartialCorrect()
    {
        Debug.Log("Partially Correct");
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        } 
        coroutine = StartCoroutine(HideCorrectHint());
    }
    System.Collections.IEnumerator HideCorrectHint()
    {
        if (hintIncorrect.activeSelf)
        {
            hintIncorrect.SetActive(false);
        }
        hintCorrect.SetActive(true);
        yield return new WaitForSeconds(3f);
        hintCorrect.SetActive(false);
    }

    System.Collections.IEnumerator HideHint()
    {   
        if (hintCorrect.activeSelf)
        {
            hintCorrect.SetActive(false);
        }
        hintIncorrect.SetActive(true);
        yield return new WaitForSeconds(3f);
        hintIncorrect.SetActive(false);
    }
  void Update()
    {
        if(GameManager.instance.CurrentState == GameManager.GameState.Choose && !isOpen)
        {
            OpenScroll();
        }
    
    }

    void OpenScroll()
    {
        if (!isOpen)
        {
            isOpen = true;
            animator.SetTrigger("Up");
        }
    }

    public void CloseScroll()
    {
        if (isOpen)
        {
            isOpen = false;
            animator.SetTrigger("Down");
        }
    }
}
