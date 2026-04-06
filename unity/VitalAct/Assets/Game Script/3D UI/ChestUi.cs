using UnityEngine;

public class ChestUI : MonoBehaviour
{
  public GameObject Chest;
    void Awake()
  {
    Chest.SetActive(false);
  }
  public void ShowUI()   => Chest.SetActive(true);
  public void HideUI()   => Chest.SetActive(false);
  public void ToggleUI() => Chest.SetActive(!Chest.activeSelf);
}