using System;
using UnityEngine;

public enum BleedingTrainingPhase
{
    Assess,
    Checklist,
    InitialChoice,
    ApplyPressure,
    ReassessChoice,
    Results
}

public class BleedingTrainingFlow : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private BleedingSystem bleedingSystem;
    [SerializeField] private BleedingResultUI resultUI;
    [SerializeField] private Objective objective;

    [Header("Optional Panels")]
    [SerializeField] private GameObject assessPanel;
    [SerializeField] private GameObject checklistPanel;
    [SerializeField] private GameObject initialChoicePanel;
    [SerializeField] private GameObject pressurePanel;
    [SerializeField] private GameObject reassessPanel;
    [SerializeField] private GameObject resultsPanel;

    [Header("Flow Timing")]
    [SerializeField] private bool autoStartOnPlay = true;
    [SerializeField] private float assessmentMinimumDuration = 3f;
    [SerializeField] private bool requireChecklistBeforePressure = true;
    [SerializeField] private bool requireReassessChoice = true;

    [Header("Debug")]
    [SerializeField] private bool enableDebugKeyboard = true;
    [SerializeField] private bool showDebugOverlay = true;

    public event Action<BleedingTrainingPhase> PhaseChanged;
    public event Action<string, string> PromptChanged;
    public event Action<bool, bool, bool, bool> ChecklistChanged;
    public event Action<string[]> OptionsChanged;

    public BleedingTrainingPhase CurrentPhase => _currentPhase;
    public string CurrentTitle => _currentTitle;
    public string CurrentInstruction => _currentInstruction;
    public bool SceneSafeChecked => _sceneSafeChecked;
    public bool SevereBleedingChecked => _severeBleedingChecked;
    public bool EmergencyCalledChecked => _emergencyCalledChecked;
    public bool ReadyForPressureChecked => _readyForPressureChecked;

    private BleedingTrainingPhase _currentPhase;
    private string _currentTitle = "";
    private string _currentInstruction = "";
    private string[] _currentOptions = Array.Empty<string>();

    private float _assessmentTimer;
    private bool _sceneSafeChecked;
    private bool _severeBleedingChecked;
    private bool _emergencyCalledChecked;
    private bool _readyForPressureChecked;
    private bool _reassessPromptShown;
    private int _incorrectChoices;
    private int _checklistSubmissionCount;
    private bool _checklistCompleted;
    private int _initialChoiceAttempts;
    private bool _initialChoiceCorrect;
    private int _reassessChoiceAttempts;
    private bool _reassessChoiceCorrect;

    private void Awake()
    {
        if (bleedingSystem == null)
        {
            bleedingSystem = FindObjectOfType<BleedingSystem>();
        }

        if (bleedingSystem != null)
        {
            bleedingSystem.SetAutoStartOnPlay(false);
        }
    }

    private void OnEnable()
    {
        if (bleedingSystem != null)
        {
            bleedingSystem.SessionCompleted += OnSessionCompleted;
            bleedingSystem.SurgeStateChanged += OnSurgeStateChanged;
        }
    }

    private void OnDisable()
    {
        if (bleedingSystem != null)
        {
            bleedingSystem.SessionCompleted -= OnSessionCompleted;
            bleedingSystem.SurgeStateChanged -= OnSurgeStateChanged;
        }
    }

    private void Start()
    {
        if (autoStartOnPlay)
        {
            BeginTraining();
        }
    }

    private void Update()
    {
        if (_currentPhase == BleedingTrainingPhase.Assess)
        {
            _assessmentTimer += Time.deltaTime;
        }

        if (enableDebugKeyboard)
        {
            HandleDebugKeyboard();
        }
    }

    [ContextMenu("Begin Bleeding Training")]
    public void BeginTraining()
    {
        _assessmentTimer = 0f;
        _sceneSafeChecked = false;
        _severeBleedingChecked = false;
        _emergencyCalledChecked = false;
        _readyForPressureChecked = false;
        _reassessPromptShown = false;
        _incorrectChoices = 0;
        _checklistSubmissionCount = 0;
        _checklistCompleted = false;
        _initialChoiceAttempts = 0;
        _initialChoiceCorrect = false;
        _reassessChoiceAttempts = 0;
        _reassessChoiceCorrect = false;

        if (bleedingSystem != null)
        {
            bleedingSystem.ResetSystem();
            bleedingSystem.SetInputEnabled(false);
            bleedingSystem.SetSimulationPaused(true);
        }

        SetPanelState(resultsPanel, false);

        EnterPhase(
            BleedingTrainingPhase.Assess,
            "Assess the victim",
            "Look for life-threatening bleeding. Confirm the scene is safe, identify severe bleeding, and prepare to control it before touching the wound.",
            new[] { "Continue" });
    }

    public void ContinueAssessment()
    {
        if (_currentPhase != BleedingTrainingPhase.Assess)
        {
            return;
        }

        if (_assessmentTimer < assessmentMinimumDuration)
        {
            SetPrompt(
                "Assess the victim",
                $"Keep assessing for {assessmentMinimumDuration - _assessmentTimer:0.0}s more before moving on.");
            return;
        }

        EnterPhase(
            BleedingTrainingPhase.Checklist,
            "Checklist",
            "Before direct pressure, confirm the scene is safe, severe bleeding is identified, help is being called, and you are ready to apply firm pressure.",
            new[] { "Submit checklist" });
    }

    public void SetSceneSafe(bool value)
    {
        _sceneSafeChecked = value;
        EmitChecklist();
    }

    public void SetSevereBleedingIdentified(bool value)
    {
        _severeBleedingChecked = value;
        EmitChecklist();
    }

    public void SetEmergencyCalled(bool value)
    {
        _emergencyCalledChecked = value;
        EmitChecklist();
    }

    public void SetReadyForPressure(bool value)
    {
        _readyForPressureChecked = value;
        EmitChecklist();
    }

    public void SubmitChecklist()
    {
        if (_currentPhase != BleedingTrainingPhase.Checklist)
        {
            return;
        }

        if (requireChecklistBeforePressure &&
            (!(_sceneSafeChecked && _severeBleedingChecked && _emergencyCalledChecked && _readyForPressureChecked)))
        {
            SetPrompt(
                "Checklist incomplete",
                "Complete all checklist items before direct pressure. Severe bleeding control should not start until the scene is safe and help is on the way.");
            return;
        }

        _checklistSubmissionCount++;
        _checklistCompleted = true;

        EnterPhase(
            BleedingTrainingPhase.InitialChoice,
            "Choose the next action",
            "Life-threatening bleeding is identified. What should you do next?",
            new[]
            {
                "1. Apply firm direct pressure now",
                "2. Keep lifting the wound to inspect it",
                "3. Wait for help before doing anything",
                "4. Search for a pulse before treating the bleeding"
            });
    }

    public void ChooseApplyDirectPressure()
    {
        ResolveInitialChoice(true,
            "Correct. Apply firm direct pressure immediately.",
            "Direct pressure is the first priority for severe external bleeding.");
    }

    public void ChooseKeepInspecting()
    {
        ResolveInitialChoice(false,
            "Incorrect. Do not keep lifting the wound to inspect it.",
            "Repeatedly lifting pressure breaks clot formation and worsens bleeding.");
    }

    public void ChooseWaitForHelp()
    {
        ResolveInitialChoice(false,
            "Incorrect. Do not wait passively for help.",
            "Severe bleeding needs immediate control while help is being called.");
    }

    public void ChooseSearchForPulse()
    {
        ResolveInitialChoice(false,
            "Incorrect. Control the severe bleeding first.",
            "A pulse check should not delay direct pressure for life-threatening bleeding.");
    }

    public void ChooseMaintainPressureAndAddDressing()
    {
        ResolveReassessChoice(true,
            "Correct. Maintain firm pressure.",
            "If blood soaks through, keep pressure and add more dressing on top rather than removing the first one.");
    }

    public void ChooseLiftHandToCheck()
    {
        ResolveReassessChoice(false,
            "Incorrect. Do not lift your hand to check yet.",
            "Releasing direct pressure can restart heavy bleeding.");
    }

    public void ChooseRemoveFirstDressing()
    {
        ResolveReassessChoice(false,
            "Incorrect. Do not remove the first dressing.",
            "Leave the original dressing in place and add material on top while maintaining pressure.");
    }

    private void ResolveInitialChoice(bool correct, string title, string feedback)
    {
        if (_currentPhase != BleedingTrainingPhase.InitialChoice)
        {
            return;
        }

        BleedingSessionData session = EnsureTrainingSession();
        _initialChoiceAttempts++;
        if (session != null) session.initialChoiceAttempts = _initialChoiceAttempts;

        if (correct)
        {
            _initialChoiceCorrect = true;
            if (session != null) session.initialChoiceCorrect = true;
            BeginPressurePhase();
            return;
        }

        _incorrectChoices++;
        if (session != null)
        {
            session.incorrectChoices = _incorrectChoices;
        }

        SetPrompt(title, feedback);
    }

    private void ResolveReassessChoice(bool correct, string title, string feedback)
    {
        if (_currentPhase != BleedingTrainingPhase.ReassessChoice)
        {
            return;
        }

        BleedingSessionData session = EnsureTrainingSession();
        _reassessChoiceAttempts++;
        if (session != null) session.reassessChoiceAttempts = _reassessChoiceAttempts;

        if (!correct)
        {
            _incorrectChoices++;
            if (session != null)
            {
                session.incorrectChoices = _incorrectChoices;
            }

            if (bleedingSystem != null)
            {
                bleedingSystem.ApplyTrainingPenalty(0.2f, true);
            }
        }
        else
        {
            _reassessChoiceCorrect = true;
            if (session != null) session.reassessChoiceCorrect = true;
        }

        if (bleedingSystem != null)
        {
            bleedingSystem.SetSimulationPaused(false);
            bleedingSystem.SetInputEnabled(true);
        }

        EnterPhase(
            BleedingTrainingPhase.ApplyPressure,
            title,
            feedback,
            Array.Empty<string>());
    }

    private void BeginPressurePhase()
    {
        if (bleedingSystem == null)
        {
            return;
        }

        bleedingSystem.ResetSystem();
        bleedingSystem.BeginScenario();
        bleedingSystem.SetSimulationPaused(false);
        bleedingSystem.SetInputEnabled(true);

        BleedingSessionData session = EnsureTrainingSession();
        if (session != null)
        {
            session.assessmentDuration = _assessmentTimer;
            session.checklistSubmissionCount = _checklistSubmissionCount;
            session.checklistCompleted = _checklistCompleted;
            session.sceneSafeChecked = _sceneSafeChecked;
            session.severeBleedingChecked = _severeBleedingChecked;
            session.emergencyCalledChecked = _emergencyCalledChecked;
            session.readyForPressureChecked = _readyForPressureChecked;
            session.incorrectChoices = _incorrectChoices;
            session.initialChoiceAttempts = _initialChoiceAttempts;
            session.initialChoiceCorrect = _initialChoiceCorrect;
            session.reassessChoiceAttempts = _reassessChoiceAttempts;
            session.reassessChoiceCorrect = _reassessChoiceCorrect;
        }

        EnterPhase(
            BleedingTrainingPhase.ApplyPressure,
            "Apply direct pressure",
            "Keep one hand in the control zone. Hold steady pressure, survive blood surges, and do not release pressure just to check the wound.",
            Array.Empty<string>());
    }

    private void OnSurgeStateChanged(bool active)
    {
        if (!requireReassessChoice || _reassessPromptShown || active)
        {
            return;
        }

        if (_currentPhase != BleedingTrainingPhase.ApplyPressure || bleedingSystem == null)
        {
            return;
        }

        if (bleedingSystem.SessionElapsed < 6f)
        {
            return;
        }

        _reassessPromptShown = true;
        bleedingSystem.SetSimulationPaused(true);
        bleedingSystem.SetInputEnabled(false);

        EnterPhase(
            BleedingTrainingPhase.ReassessChoice,
            "Reassess the wound",
            "Blood is still present but pressure is helping. What should you do now?",
            new[]
            {
                "1. Maintain pressure and add more dressing on top if needed",
                "2. Lift your hand now to check the wound",
                "3. Remove the first dressing and start again"
            });
    }

    private void OnSessionCompleted(BleedingSessionData data)
    {
        if (data == null)
        {
            return;
        }

        data.incorrectChoices = _incorrectChoices;
        data.assessmentDuration = _assessmentTimer;
        data.checklistSubmissionCount = _checklistSubmissionCount;
        data.checklistCompleted = _checklistCompleted;
        data.sceneSafeChecked = _sceneSafeChecked;
        data.severeBleedingChecked = _severeBleedingChecked;
        data.emergencyCalledChecked = _emergencyCalledChecked;
        data.readyForPressureChecked = _readyForPressureChecked;
        data.initialChoiceAttempts = _initialChoiceAttempts;
        data.initialChoiceCorrect = _initialChoiceCorrect;
        data.reassessChoiceAttempts = _reassessChoiceAttempts;
        data.reassessChoiceCorrect = _reassessChoiceCorrect;

        EnterPhase(
            BleedingTrainingPhase.Results,
            "Bleeding controlled",
            "Direct pressure was maintained long enough to control the bleeding.",
            Array.Empty<string>());

        SetPanelState(resultsPanel, true);
    }

    private BleedingSessionData EnsureTrainingSession()
    {
        if (bleedingSystem == null)
        {
            return null;
        }

        return bleedingSystem.LastSessionData;
    }

    private void EnterPhase(BleedingTrainingPhase phase, string title, string instruction, string[] options)
    {
        _currentPhase = phase;
        _currentTitle = title;
        _currentInstruction = instruction;
        _currentOptions = options ?? Array.Empty<string>();

        UpdateOptionalPanels();

        if (objective != null)
        {
            objective.SetObjective(title);
        }

        PhaseChanged?.Invoke(_currentPhase);
        PromptChanged?.Invoke(_currentTitle, _currentInstruction);
        OptionsChanged?.Invoke(_currentOptions);
    }

    private void SetPrompt(string title, string instruction)
    {
        _currentTitle = title;
        _currentInstruction = instruction;
        if (objective != null)
        {
            objective.SetObjective(title);
        }
        PromptChanged?.Invoke(_currentTitle, _currentInstruction);
    }

    private void EmitChecklist()
    {
        ChecklistChanged?.Invoke(_sceneSafeChecked, _severeBleedingChecked, _emergencyCalledChecked, _readyForPressureChecked);
    }

    private void UpdateOptionalPanels()
    {
        SetPanelState(assessPanel, _currentPhase == BleedingTrainingPhase.Assess);
        SetPanelState(checklistPanel, _currentPhase == BleedingTrainingPhase.Checklist);
        SetPanelState(initialChoicePanel, _currentPhase == BleedingTrainingPhase.InitialChoice);
        SetPanelState(pressurePanel, _currentPhase == BleedingTrainingPhase.ApplyPressure);
        SetPanelState(reassessPanel, _currentPhase == BleedingTrainingPhase.ReassessChoice);
    }

    private static void SetPanelState(GameObject panel, bool visible)
    {
        if (panel != null)
        {
            panel.SetActive(visible);
        }
    }

    private void HandleDebugKeyboard()
    {
        switch (_currentPhase)
        {
            case BleedingTrainingPhase.Assess:
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ContinueAssessment();
                }
                break;

            case BleedingTrainingPhase.Checklist:
                if (Input.GetKeyDown(KeyCode.Alpha1)) SetSceneSafe(!_sceneSafeChecked);
                if (Input.GetKeyDown(KeyCode.Alpha2)) SetSevereBleedingIdentified(!_severeBleedingChecked);
                if (Input.GetKeyDown(KeyCode.Alpha3)) SetEmergencyCalled(!_emergencyCalledChecked);
                if (Input.GetKeyDown(KeyCode.Alpha4)) SetReadyForPressure(!_readyForPressureChecked);
                if (Input.GetKeyDown(KeyCode.Return)) SubmitChecklist();
                break;

            case BleedingTrainingPhase.InitialChoice:
                if (Input.GetKeyDown(KeyCode.Alpha1)) ChooseApplyDirectPressure();
                if (Input.GetKeyDown(KeyCode.Alpha2)) ChooseKeepInspecting();
                if (Input.GetKeyDown(KeyCode.Alpha3)) ChooseWaitForHelp();
                if (Input.GetKeyDown(KeyCode.Alpha4)) ChooseSearchForPulse();
                break;

            case BleedingTrainingPhase.ReassessChoice:
                if (Input.GetKeyDown(KeyCode.Alpha1)) ChooseMaintainPressureAndAddDressing();
                if (Input.GetKeyDown(KeyCode.Alpha2)) ChooseLiftHandToCheck();
                if (Input.GetKeyDown(KeyCode.Alpha3)) ChooseRemoveFirstDressing();
                break;
        }
    }

    private void OnGUI()
    {
        if (!showDebugOverlay)
        {
            return;
        }

        GUI.Box(new Rect(18f, 18f, 520f, 250f), "Bleeding Training Debug");
        GUI.Label(new Rect(32f, 48f, 490f, 24f), $"Phase: {_currentPhase}");
        GUI.Label(new Rect(32f, 72f, 490f, 24f), $"Title: {_currentTitle}");
        GUI.Label(new Rect(32f, 96f, 490f, 60f), _currentInstruction);

        float y = 150f;
        if (_currentPhase == BleedingTrainingPhase.Checklist)
        {
            GUI.Label(new Rect(32f, y, 490f, 24f), $"1 Scene safe: {_sceneSafeChecked}");
            GUI.Label(new Rect(32f, y + 22f, 490f, 24f), $"2 Severe bleeding identified: {_severeBleedingChecked}");
            GUI.Label(new Rect(32f, y + 44f, 490f, 24f), $"3 Emergency help called: {_emergencyCalledChecked}");
            GUI.Label(new Rect(32f, y + 66f, 490f, 24f), $"4 Ready for direct pressure: {_readyForPressureChecked}");
            GUI.Label(new Rect(32f, y + 96f, 490f, 24f), "Press Enter to submit checklist");
            return;
        }

        for (int i = 0; i < _currentOptions.Length; i++)
        {
            GUI.Label(new Rect(32f, y + i * 22f, 490f, 24f), _currentOptions[i]);
        }
    }
}
