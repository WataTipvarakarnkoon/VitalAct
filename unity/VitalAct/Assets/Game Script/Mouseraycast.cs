using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
  public ChestUI Chest;
  public HeadUI Head;
  public ChecklistUI checklist;
  private int scannedCount = 0;
  private int requiredScans = 2;
  private HashSet<GameObject> scannedBodyParts = new HashSet<GameObject>();
  public Objective objective;

  void Update()
  {
    if (checklist.IsOpen)
    {
      return;
    }
    if (Input.GetMouseButtonDown(0))
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2f);
      RaycastHit hit;

      if(Physics.Raycast(ray, out hit))
      {
        Debug.Log("Hit" + hit.collider.name);
        if (hit.collider.CompareTag("Chest"))
        {
          Chest.ToggleUI();

          if (!scannedBodyParts.Contains(hit.collider.gameObject))
          {
            scannedBodyParts.Add(hit.collider.gameObject);
            scannedCount++;
            Debug.Log($"Scanned {hit.collider.name}! Progress: {scannedCount}/{requiredScans}");
          
            if (scannedCount >= requiredScans)
              {
                  GameManager.instance.CurrentState = GameManager.GameState.Identify;
                  objective.SetObjective("Press TAB, and fill the checklist.");
              }
          }
          
        }
        if (hit.collider.CompareTag("Head"))
        {
          Head.ToggleUI();

          if (!scannedBodyParts.Contains(hit.collider.gameObject))
          {
            scannedBodyParts.Add(hit.collider.gameObject);
            scannedCount++;
            Debug.Log($"Scanned {hit.collider.name}! Progress: {scannedCount}/{requiredScans}");
          
            if (scannedCount >= requiredScans)
              {
                  GameManager.instance.CurrentState = GameManager.GameState.Identify;
                  objective.SetObjective("Press TAB, and fill the checklist.");
              }
          }
        }
      }
    }
  }

    void Start()
  {
      Toggle[] allToggles = FindObjectsByType<Toggle>(FindObjectsSortMode.None);
      foreach (Toggle toggle in allToggles)
      {
          toggle.onValueChanged.AddListener(OnToggleChanged);
      }
  }

  void OnToggleChanged(bool value)
  {
      if (AreAllTogglersSelected())
      {
          GameManager.instance.CurrentState = GameManager.GameState.CPR;
          
          if (objective != null)
              objective.SetObjective("Perform CPR");
      }
      else
      {
          // Reset state if not all selected
          GameManager.instance.CurrentState = GameManager.GameState.Assess;
          
          if (objective != null)
              objective.SetObjective("Complete Assessment");
      }
  }

  bool AreAllTogglersSelected()
  {
      Toggle[] allToggles = FindObjectsByType<Toggle>(FindObjectsSortMode.None);
      Dictionary<ToggleGroup, bool> groupHasSelection = new Dictionary<ToggleGroup, bool>();
      
      foreach (Toggle toggle in allToggles)
      {
          if (toggle.group != null)
          {
              if (!groupHasSelection.ContainsKey(toggle.group))
                  groupHasSelection[toggle.group] = false;
              
              if (toggle.isOn)
                  groupHasSelection[toggle.group] = true;
          }
      }
      
      foreach (var group in groupHasSelection)
      {
          if (!group.Value) return false;
      }
      return true;
  }

}
