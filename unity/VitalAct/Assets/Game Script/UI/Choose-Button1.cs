
using UnityEngine;
using UnityEngine.UI;

public class ChooseButton1 : MonoBehaviour
{
    Button button;
    public GameObject[] gameObjects;

  void Start()
  {
    button = GetComponent<Button>();
    button.onClick.AddListener(ToggleObject);
  }
  public void ToggleObject()
    {   
        GameManager.instance.Choosed();

        foreach(GameObject obj in gameObjects)
        {   
            if(obj != null)
                obj.SetActive(!obj.activeSelf);
        }
}
}