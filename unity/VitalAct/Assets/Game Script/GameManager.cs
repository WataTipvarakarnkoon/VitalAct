using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public CprHandDetector handDetector;
    public CprCycleCounter cycleCounter;
    public Objective objective;


    public enum GameState
    {
        Assess,
        Identify,
        Choose,
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

        switch (newState)
        {
            case GameState.Assess:
                if (objective != null)
                    objective.SetObjective("Check the victim's condition.");
                break;

            case GameState.Identify:
                if (objective != null)
                    objective.SetObjective("Press TAB, and fill the checklist.");
                break;

            case GameState.Choose:
                if (objective != null)
                    objective.SetObjective("Select the button.");
                break;

            case GameState.Do:
                if (objective != null)
                    objective.SetObjective("Perform CPR.");

                BeginSession();
                break;

            case GameState.Result:
                break;
        }
    }
    public void ScanCompleted() => SetState(GameState.Identify);
    public void AllTogglesSeleted() => SetState(GameState.Choose);
    public void Choosed() => SetState(GameState.Do);
    public void ResultScreen() => SetState(GameState.Result);

    // CPR LOGIC
    void BeginSession()
    {
        data = new CprSessionData();
        cycleCounter?.ResetAll();
    }

    void OnCompression()
    {
        if (CurrentState != GameState.Do || data == null) return;

        data.totalCompressions++;

        float rate  = handDetector != null ? handDetector.compressionRate    : 0f;
        float depth = handDetector != null ? handDetector.compressionDepth01 : 0f;

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
}