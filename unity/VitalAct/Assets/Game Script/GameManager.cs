using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject ChecklistButton;
    public GameObject [] CPR;
    public static GameManager instance;

    [Header("References")]
    public CprHandDetector handDetector;
    public CprCycleCounter cycleCounter;
    public Objective objective;
    string Scene;


    public enum GameState
    {
        Assess,
        Identify,
        Choose,
        SetUp,
        Do,
        Result
    }

    public GameState CurrentState;

    // Events
    public static System.Action<GameState> OnStateChanged;
    public static System.Action<CprSessionData> OnSessionComplete;
    public static bool NoCameraMode = false;

    public static void SetNoCameraMode(bool value)
    {
        NoCameraMode = value;
        PlayerPrefs.SetInt("NoCameraMode", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Data
    CprSessionData data;
    public CprSessionData LastSessionData => data;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        Scene = SceneManager.GetActiveScene().name;
    }

    void OnEnable()  => CprHandDetector.OnCompression += OnCompression;
    void OnDisable() => CprHandDetector.OnCompression -= OnCompression;

    void Start()
    {
        if (FindObjectOfType<CprSessionAnalytics>() == null)
            gameObject.AddComponent<CprSessionAnalytics>();

        SetState(GameState.Assess);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
        HandleStateChange();

        switch (newState)
        {
            case GameState.Assess:
                if (objective != null)
                    Debug.Log("GameState Assess");
                    objective.SetObjective("Check the victim's condition.");
                break;

            case GameState.Identify:
                if (objective != null)
                Debug.Log("GameState Identify");
                    objective.SetObjective("Press the checklist button, and fill the checklist.");
                break;

            case GameState.Choose:
                if (objective != null)
                Debug.Log("GameState Choose");
                    if(Scene == "CPR")
                        objective.SetObjective("Select the button.");
                    else
                        objective.SetObjective("Choose the correct answers.");
                break;
            case GameState.SetUp:
                if (objective != null)
                    Debug.Log("SetUp");
                break;
            case GameState.Do:
                if (objective != null)
                Debug.Log("GameState Do");
                    if(Scene == "CPR")
                        objective.SetObjective("Perform CPR.");
                    else
                        objective.SetObjective("Stop the bleeding with pressure");
                BeginSession();
                break;

            case GameState.Result:
                Debug.Log("GameState Result");
                break;
        }
    }
    public void ScanCompleted() => SetState(GameState.Identify);
    public void AllTogglesSeleted() => SetState(GameState.Choose);
    public void Setup() => SetState(GameState.SetUp);
    public void Choosed() => SetState(GameState.Do);
    public void ResultScreen() => SetState(GameState.Result);

    // CPR LOGIC
    void BeginSession()
    {
        data = new CprSessionData();
        cycleCounter?.ResetAll();
    }

    void OnCompression(float rate, float depth)
    {
        if (CurrentState != GameState.Do || data == null) return;
        if (rate <= 0f) return; // skip first compressions before rate is established

        data.totalCompressions++;

        if (rate >= 100f && rate <= 120f)
            data.goodRateCompressions++;

        if (depth >= 0.5f)
            data.goodDepthCompressions++;
    }

    public void EndSession()
    {
        if (CurrentState == GameState.Result || data == null) return;

        data.completedCycles = cycleCounter != null ? cycleCounter.completedCycles : 0;

        SetState(GameState.Result);

        // Send data to UI
        OnSessionComplete?.Invoke(data);
    }

    void HandleStateChange()
    {   
        if(CurrentState == GameState.Identify)
        {
            if(ChecklistButton != null)
            ChecklistButton.SetActive(true);
        }
        else
        {
            if(ChecklistButton != null)
            ChecklistButton.SetActive(false);
        }


        if(CurrentState == GameState.Do)
        {   
            foreach (var obj in CPR)
            {
                if(obj != null)
                obj.SetActive(true);
            }
        }
        else
        {
            foreach (var obj in CPR)
            {
                if(obj != null)
                obj.SetActive(false);
            }
        }
    }
}