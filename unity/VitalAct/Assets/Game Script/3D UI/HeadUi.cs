using UnityEngine;

public class HeadUI : MonoBehaviour
{
  public GameObject Head;
    void Awake()
  {
    Head.SetActive(false);
  }
  public void ShowUI()   => Head.SetActive(true);
  public void HideUI()   => Head.SetActive(false);
  public void ToggleUI() => Head.SetActive(!Head.activeSelf);
}