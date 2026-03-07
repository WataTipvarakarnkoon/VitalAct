using TMPro;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public TextMeshProUGUI objectiveUI;
    public void SetObjective(string text)
  {
    objectiveUI.text = text;
  }
}
