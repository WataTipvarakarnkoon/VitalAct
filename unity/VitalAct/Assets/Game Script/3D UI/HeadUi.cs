using UnityEngine;

public class HeadUI : MonoBehaviour
{
  public GameObject Head;
    void Start()
  {
    Head.SetActive(false);
  }
  public void ToggleUI()
{
    if (Head.activeSelf)
    {
        Head.SetActive(false);
    }
    else
    {
        Head.SetActive(true);
    }
}
}