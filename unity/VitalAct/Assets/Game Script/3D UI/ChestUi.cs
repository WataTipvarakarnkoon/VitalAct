using UnityEngine;

public class ChestUI : MonoBehaviour
{
  public GameObject Chest;
    void Start()
  {
    Chest.SetActive(false);
  }
  public void ToggleUI()
{
    if (Chest.activeSelf)
    {
        Chest.SetActive(false);
    }
    else
    {
        Chest.SetActive(true);
    }
}
}



