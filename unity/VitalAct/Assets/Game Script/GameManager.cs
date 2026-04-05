using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool NoCameraMode = false;
    public Objective objective;

    public enum GameState
    {
        Assess,
        Identify,
        Choose,
        Do
    }

    public GameState CurrentState;

    void Awake()
    {
        instance = this;
        NoCameraMode = PlayerPrefs.GetInt("NoCameraMode", 0) == 1;
    }

    public static void SetNoCameraMode(bool value)
    {
        NoCameraMode = value;
        PlayerPrefs.SetInt("NoCameraMode", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetState(GameState NewState)
    {
        CurrentState = NewState;

        switch (CurrentState)
        {
            case GameState.Assess:
                Debug.Log("GameState1");
                objective.SetObjective("Check the victim's condition.");
                break;
            case GameState.Identify:
                Debug.Log("GameState2");    
                objective.SetObjective("Press TAB, and fill the checklist.");
                break;
                case GameState.Choose:
                Debug.Log("GameState3");  
                objective.SetObjective("Select the button.");
                break;
            case GameState.Do:
                Debug.Log("GameState4");  
                objective.SetObjective("Perform CPR.");
                break;
        }
    }

    public void ScanCompleted()
    {
        SetState(GameState.Identify);
    }

       public void AllTogglesSeleted()
    {
        SetState(GameState.Choose);
    }
}