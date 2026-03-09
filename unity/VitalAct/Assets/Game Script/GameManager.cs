using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Objective objective;

    public enum GameState
    {
        Assess,
        Identify,
        CPR
    }

    public GameState CurrentState;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        switch (CurrentState)
        {
            case GameState.Assess:
                objective.SetObjective("Check the victim's condition.");
                break;
            case GameState.Identify:  
                objective.SetObjective("Press TAB, and fill the checklist.");
                break;
            case GameState.CPR:
                objective.SetObjective("Perform CPR.");
                break;
        }
    }
}