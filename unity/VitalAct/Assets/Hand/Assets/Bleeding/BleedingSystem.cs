using System;
using UnityEngine;

public enum PressureQuality
{
    None,
    TooLight,
    Good,
    TooHard
}

public enum BleedingGameState
{
    Prepare,
    Active,
    Success
}

public class BleedingSystem : MonoBehaviour
{
    [Header("Scenario")]
    [SerializeField] private bool autoStartOnPlay = true;
    [SerializeField] private float criticalBleedingThreshold = 0.7f;

    [Header("Bleeding Rates")]
    [SerializeField] private float bleedRateIdle = 0.08f;
    [SerializeField] private float reduceRateTooLight = 0.015f;
    [SerializeField] private float reduceRateGood = 0.12f;
    [SerializeField] private float reduceRateTooHard = 0.07f;
    [SerializeField] private float releaseSurgeRate = 0.25f;
    [SerializeField] private float releaseSurgeDuration = 0.8f;

    [Header("Pressure Thresholds")]
    [SerializeField] private float lightThreshold = 0.25f;
    [SerializeField] private float hardThreshold = 0.8f;

    [Header("Stability")]
    [SerializeField] private float instabilityThreshold = 1.2f;
    [SerializeField] private float stabilitySmoothing = 8f;
    [SerializeField] private float stabilityRecoverSpeed = 5f;
    [SerializeField] private float instabilityPenalty = 0.45f;
    [SerializeField] private float unstablePenaltyStart = 0.55f;
    [SerializeField] private float unstableBleedPenalty = 0.1f;

    [Header("Success")]
    [SerializeField] private float winBleedingThreshold = 0.15f;
    [SerializeField] private float winHoldDuration = 8f;
    [SerializeField] private float minimumSessionDuration = 12f;
    [SerializeField] private float minimumGoodPressureDuration = 6f;
    [SerializeField] private float minimumStableDuration = 5f;
    [SerializeField] private int requiredSurvivedSurges = 1;

    [Header("Bleed Surge")]
    [SerializeField] private bool enableBleedSurges = true;
    [SerializeField] private Vector2 surgeIntervalRange = new Vector2(5.5f, 9f);
    [SerializeField] private float surgeDuration = 1.6f;
    [SerializeField] private float surgeBleedRate = 0.16f;
    [SerializeField] private float surgeDepthBonus = 0.12f;

    [Header("Output")]
    [Range(0f, 1f)] [SerializeField] private float bleedingLevel = 1f;
    [SerializeField] private float holdTime = 0f;
    [Range(0f, 1f)] [SerializeField] private float stabilityScore = 1f;
    [SerializeField] private PressureQuality pressureQuality = PressureQuality.None;
    [SerializeField] private BleedingGameState currentState = BleedingGameState.Prepare;

    public event Action<float> BleedingChanged;
    public event Action<float> HoldTimeChanged;
    public event Action<float> StabilityChanged;
    public event Action<PressureQuality> PressureQualityChanged;
    public event Action<BleedingGameState> StateChanged;
    public event Action<bool> CriticalStateChanged;
    public event Action ScenarioSucceeded;
    public event Action<BleedingSessionData> SessionCompleted;
    public event Action<bool> SurgeStateChanged;

    public float BleedingLevel => bleedingLevel;
    public float HoldTime => holdTime;
    public float StabilityScore => stabilityScore;
    public PressureQuality CurrentPressureQuality => pressureQuality;
    public BleedingGameState CurrentState => currentState;
    public bool IsPressing => _isPressing;
    public float CurrentPressDepth => _pressDepth;
    public bool IsCritical => currentState == BleedingGameState.Active && bleedingLevel >= criticalBleedingThreshold;
    public float WinHoldDuration => winHoldDuration;
    public float WinBleedingThreshold => winBleedingThreshold;
    public BleedingSessionData LastSessionData => _sessionData;
    public bool IsBleedSurging => _surgeActive;
    public float TimeToControl => Mathf.Clamp(winHoldDuration - holdTime, 0f, winHoldDuration);
    public float RecommendedMinDepth => Mathf.Clamp01(lightThreshold + (_surgeActive ? surgeDepthBonus : 0f));
    public string BleedingStatusLabel => bleedingLevel >= 0.75f ? "Heavy" : bleedingLevel >= 0.45f ? "Moderate" : bleedingLevel >= 0.2f ? "Controlled" : "Nearly Stopped";
    public float SessionElapsed => _sessionData != null ? _sessionData.totalDuration : 0f;
    public bool IsSimulationPaused => _simulationPaused;
    public bool IsInputEnabled => _inputEnabled;

    private bool _isPressing;
    private float _pressDepth;
    private float _prevPressDepth;
    private float _releaseSurgeTimer;
    private bool _wasPressingLastFrame;
    private bool _lastCriticalState;

    private float _lastBleedingBroadcast = -1f;
    private float _lastHoldBroadcast = -1f;
    private float _lastStabilityBroadcast = -1f;
    private PressureQuality _lastPressureBroadcast = PressureQuality.None;
    private BleedingSessionData _sessionData;
    private float _depthIntegral;
    private bool _surgeActive;
    private float _surgeTimer;
    private float _nextSurgeTimer;
    private bool _surgeSucceeded;
    private bool _simulationPaused;
    private bool _inputEnabled = true;

    private void Start()
    {
        ResetSystem();

        if (autoStartOnPlay)
        {
            BeginScenario();
        }
    }

    [ContextMenu("Apply Training Defaults")]
    public void ApplyTrainingDefaults()
    {
        bleedRateIdle = 0.1f;
        reduceRateTooLight = 0.005f;
        reduceRateGood = 0.07f;
        reduceRateTooHard = 0.04f;
        releaseSurgeRate = 0.32f;
        releaseSurgeDuration = 1.1f;

        lightThreshold = 0.35f;
        hardThreshold = 0.78f;

        instabilityThreshold = 0.9f;
        stabilitySmoothing = 9f;
        stabilityRecoverSpeed = 4f;
        instabilityPenalty = 0.32f;
        unstablePenaltyStart = 0.7f;
        unstableBleedPenalty = 0.16f;

        winBleedingThreshold = 0.08f;
        winHoldDuration = 10f;
        minimumSessionDuration = 18f;

        enableBleedSurges = true;
        surgeIntervalRange = new Vector2(4.5f, 7.5f);
        surgeDuration = 2f;
        surgeBleedRate = 0.2f;
        surgeDepthBonus = 0.18f;
    }

    [ContextMenu("Apply Hard Training Defaults")]
    public void ApplyHardTrainingDefaults()
    {
        ApplyTrainingDefaults();
        reduceRateGood = 0.055f;
        releaseSurgeRate = 0.38f;
        unstableBleedPenalty = 0.22f;
        winHoldDuration = 12f;
        minimumSessionDuration = 22f;
        surgeIntervalRange = new Vector2(4f, 6f);
        surgeDuration = 2.4f;
        surgeBleedRate = 0.24f;
        surgeDepthBonus = 0.22f;
    }

    public void SetAutoStartOnPlay(bool value)
    {
        autoStartOnPlay = value;
    }

    public void ResetSystem()
    {
        bleedingLevel = 1f;
        holdTime = 0f;
        stabilityScore = 1f;
        pressureQuality = PressureQuality.None;
        _isPressing = false;
        _pressDepth = 0f;
        _prevPressDepth = 0f;
        _releaseSurgeTimer = 0f;
        _wasPressingLastFrame = false;
        _lastCriticalState = false;
        _depthIntegral = 0f;
        _sessionData = null;
        _surgeActive = false;
        _surgeTimer = 0f;
        _surgeSucceeded = false;
        _nextSurgeTimer = UnityEngine.Random.Range(surgeIntervalRange.x, surgeIntervalRange.y);
        _simulationPaused = false;
        _inputEnabled = true;

        SetState(BleedingGameState.Prepare);
        BroadcastAll();
    }

    public void BeginScenario()
    {
        if (currentState == BleedingGameState.Success)
        {
            ResetSystem();
        }

        _sessionData = new BleedingSessionData();
        _depthIntegral = 0f;
        _surgeActive = false;
        _surgeTimer = 0f;
        _surgeSucceeded = false;
        _nextSurgeTimer = UnityEngine.Random.Range(surgeIntervalRange.x, surgeIntervalRange.y);
        SetState(BleedingGameState.Active);
    }

    public void SetInput(bool isPressing, float pressDepth)
    {
        if (!_inputEnabled || _simulationPaused)
        {
            _isPressing = false;
            _pressDepth = 0f;
            return;
        }

        _isPressing = isPressing;
        _pressDepth = isPressing ? Mathf.Clamp01(pressDepth) : 0f;
    }

    public void SetInputEnabled(bool enabled)
    {
        _inputEnabled = enabled;
        if (!enabled)
        {
            _isPressing = false;
            _pressDepth = 0f;
        }
    }

    public void SetSimulationPaused(bool paused)
    {
        _simulationPaused = paused;
        if (paused)
        {
            _isPressing = false;
            _pressDepth = 0f;
        }
    }

    public void ApplyTrainingPenalty(float bleedIncrease, bool resetHold)
    {
        bleedingLevel = Mathf.Clamp01(bleedingLevel + Mathf.Max(0f, bleedIncrease));
        if (resetHold)
        {
            holdTime = 0f;
            _releaseSurgeTimer = Mathf.Max(_releaseSurgeTimer, releaseSurgeDuration);
        }
        BroadcastIfNeeded();
    }

    private void Update()
    {
        if (currentState != BleedingGameState.Active)
        {
            return;
        }

        if (_simulationPaused)
        {
            return;
        }

        UpdatePressureQuality();
        UpdateStability();
        UpdateHoldTime();
        UpdateSurge();
        UpdateSessionStats();
        UpdateBleeding();
        CheckSuccess();
        BroadcastIfNeeded();

        _prevPressDepth = _pressDepth;
        _wasPressingLastFrame = _isPressing;
    }

    private void UpdateSessionStats()
    {
        if (_sessionData == null)
        {
            return;
        }

        _sessionData.totalDuration += Time.deltaTime;

        if (!_isPressing)
        {
            return;
        }

        _sessionData.totalPressTime += Time.deltaTime;
        _depthIntegral += _pressDepth * Time.deltaTime;
        _sessionData.averageDepth = _sessionData.totalPressTime > 0.001f ? _depthIntegral / _sessionData.totalPressTime : 0f;
        _sessionData.maxDepth = Mathf.Max(_sessionData.maxDepth, _pressDepth);

        switch (pressureQuality)
        {
            case PressureQuality.Good:
                _sessionData.goodPressureTime += Time.deltaTime;
                break;
            case PressureQuality.TooLight:
                _sessionData.tooLightTime += Time.deltaTime;
                break;
            case PressureQuality.TooHard:
                _sessionData.tooHardTime += Time.deltaTime;
                break;
        }

        if (stabilityScore >= unstablePenaltyStart)
        {
            _sessionData.stableTime += Time.deltaTime;
        }
    }

    private void UpdatePressureQuality()
    {
        PressureQuality nextQuality = PressureQuality.None;

        if (_isPressing)
        {
            if (_pressDepth < lightThreshold)
            {
                nextQuality = PressureQuality.TooLight;
            }
            else if (_pressDepth > hardThreshold)
            {
                nextQuality = PressureQuality.TooHard;
            }
            else
            {
                nextQuality = PressureQuality.Good;
            }
        }

        if (nextQuality == pressureQuality)
        {
            return;
        }

        pressureQuality = nextQuality;
        PressureQualityChanged?.Invoke(pressureQuality);
    }

    private void UpdateStability()
    {
        if (!_isPressing)
        {
            stabilityScore = Mathf.MoveTowards(stabilityScore, 1f, Time.deltaTime * stabilityRecoverSpeed);
            return;
        }

        float deltaPerSecond = Mathf.Abs(_pressDepth - _prevPressDepth) / Mathf.Max(Time.deltaTime, 0.0001f);
        float targetStability = 1f - Mathf.Clamp01(deltaPerSecond / instabilityThreshold);
        stabilityScore = Mathf.Lerp(stabilityScore, targetStability, Time.deltaTime * stabilitySmoothing);
        stabilityScore = Mathf.Clamp01(stabilityScore);
    }

    private void UpdateHoldTime()
    {
        if (_isPressing)
        {
            holdTime += Time.deltaTime;
            _releaseSurgeTimer = 0f;
            return;
        }

        if (_wasPressingLastFrame)
        {
            _releaseSurgeTimer = releaseSurgeDuration;
        }

        holdTime = 0f;

        if (_releaseSurgeTimer > 0f)
        {
            _releaseSurgeTimer -= Time.deltaTime;
        }
    }

    private void UpdateBleeding()
    {
        float delta = 0f;

        if (!_isPressing)
        {
            float riseRate = _releaseSurgeTimer > 0f ? releaseSurgeRate : bleedRateIdle;
            delta = riseRate * Time.deltaTime;
        }
        else
        {
            float stabilityFactor = Mathf.Lerp(instabilityPenalty, 1f, stabilityScore);
            float requiredDepth = RecommendedMinDepth;
            bool belowSurgeRequirement = _surgeActive && _pressDepth < requiredDepth;

            switch (pressureQuality)
            {
                case PressureQuality.TooLight:
                    delta = -reduceRateTooLight * stabilityFactor * Time.deltaTime;
                    break;

                case PressureQuality.Good:
                    delta = -reduceRateGood * stabilityFactor * Time.deltaTime;
                    break;

                case PressureQuality.TooHard:
                    delta = -reduceRateTooHard * stabilityFactor * Time.deltaTime;
                    break;
            }

            if (stabilityScore < unstablePenaltyStart)
            {
                float instabilityAmount = 1f - Mathf.InverseLerp(0f, unstablePenaltyStart, stabilityScore);
                delta += unstableBleedPenalty * instabilityAmount * Time.deltaTime;
            }

            if (_surgeActive)
            {
                if (belowSurgeRequirement)
                {
                    delta += surgeBleedRate * Time.deltaTime;
                }
                else
                {
                    delta -= reduceRateGood * 0.35f * stabilityFactor * Time.deltaTime;
                }
            }
        }

        bleedingLevel = Mathf.Clamp01(bleedingLevel + delta);
    }

    private void UpdateSurge()
    {
        if (!enableBleedSurges)
        {
            return;
        }

        if (_surgeActive)
        {
            if (_isPressing && _pressDepth >= RecommendedMinDepth && stabilityScore >= unstablePenaltyStart)
            {
                _surgeSucceeded = true;
            }

            _surgeTimer -= Time.deltaTime;
            if (_surgeTimer <= 0f)
            {
                if (_surgeSucceeded && _sessionData != null)
                {
                    _sessionData.survivedSurgeCount++;
                }
                _surgeActive = false;
                _surgeSucceeded = false;
                _nextSurgeTimer = UnityEngine.Random.Range(surgeIntervalRange.x, surgeIntervalRange.y);
                SurgeStateChanged?.Invoke(false);
            }
            return;
        }

        _nextSurgeTimer -= Time.deltaTime;
        if (_nextSurgeTimer <= 0f)
        {
            _surgeActive = true;
            _surgeTimer = surgeDuration;
            _surgeSucceeded = false;
            SurgeStateChanged?.Invoke(true);
        }
    }

    private void CheckSuccess()
    {
        if (_sessionData == null)
        {
            return;
        }

        if (_sessionData.totalDuration < minimumSessionDuration)
        {
            return;
        }

        if (_sessionData.goodPressureTime < minimumGoodPressureDuration)
        {
            return;
        }

        if (_sessionData.stableTime < minimumStableDuration)
        {
            return;
        }

        if (_sessionData.survivedSurgeCount < requiredSurvivedSurges)
        {
            return;
        }

        if (_surgeActive)
        {
            return;
        }

        if (bleedingLevel > winBleedingThreshold || holdTime < winHoldDuration)
        {
            return;
        }

        SetState(BleedingGameState.Success);
        _isPressing = false;
        _pressDepth = 0f;
        CompleteSession();
        ScenarioSucceeded?.Invoke();
    }

    private void CompleteSession()
    {
        if (_sessionData == null)
        {
            return;
        }

        _sessionData.finalBleedingLevel = bleedingLevel;
        SessionCompleted?.Invoke(_sessionData);
    }

    private void SetState(BleedingGameState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
        StateChanged?.Invoke(currentState);
    }

    private void BroadcastAll()
    {
        _lastBleedingBroadcast = bleedingLevel;
        _lastHoldBroadcast = holdTime;
        _lastStabilityBroadcast = stabilityScore;
        _lastPressureBroadcast = pressureQuality;
        _lastCriticalState = IsCritical;

        BleedingChanged?.Invoke(bleedingLevel);
        HoldTimeChanged?.Invoke(holdTime);
        StabilityChanged?.Invoke(stabilityScore);
        PressureQualityChanged?.Invoke(pressureQuality);
        CriticalStateChanged?.Invoke(_lastCriticalState);
        SurgeStateChanged?.Invoke(_surgeActive);
    }

    private void BroadcastIfNeeded()
    {
        if (Mathf.Abs(_lastBleedingBroadcast - bleedingLevel) > 0.001f)
        {
            _lastBleedingBroadcast = bleedingLevel;
            BleedingChanged?.Invoke(bleedingLevel);
        }

        if (Mathf.Abs(_lastHoldBroadcast - holdTime) > 0.001f)
        {
            _lastHoldBroadcast = holdTime;
            HoldTimeChanged?.Invoke(holdTime);
        }

        if (Mathf.Abs(_lastStabilityBroadcast - stabilityScore) > 0.001f)
        {
            _lastStabilityBroadcast = stabilityScore;
            StabilityChanged?.Invoke(stabilityScore);
        }

        if (_lastPressureBroadcast != pressureQuality)
        {
            _lastPressureBroadcast = pressureQuality;
            PressureQualityChanged?.Invoke(pressureQuality);
        }

        bool isCriticalNow = IsCritical;
        if (_lastCriticalState != isCriticalNow)
        {
            _lastCriticalState = isCriticalNow;
            CriticalStateChanged?.Invoke(isCriticalNow);
        }
    }
}
