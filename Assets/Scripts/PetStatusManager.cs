using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class StatsManager : MonoBehaviour
{
    public PetBehavior petBehavior;
    private Animator stageAnimator;
    public RuntimeAnimatorController babyController;
    public RuntimeAnimatorController childController;
    public RuntimeAnimatorController adultController;
    public RuntimeAnimatorController seniorController;
    public RuntimeAnimatorController deathController;

    public enum LifeStage { Baby, Child, Adult, Senior, Death }
    public LifeStage CurrentLifeStage { get; private set; } = LifeStage.Baby;

    // // Time requirements for each stage
    private readonly TimeSpan babyTimeRequirement = TimeSpan.FromHours(14);
    private readonly TimeSpan childTimeRequirement = TimeSpan.FromDays(5);
    private readonly TimeSpan adultTimeRequirement = TimeSpan.FromDays(30);
    private readonly TimeSpan seniorTimeRequirement = TimeSpan.FromDays(4000); // Need to change this to something else eventually.

    private float _hungerDecreaseRatePerHour = 16.67f; // 6 hours to reach 0
    private float _thirstDecreaseRatePerHour = 25f; // 4 hours to reach 0
    private float _cleanDecreaseRatePerHour = 11.11f; // 9 hours to reach 0
    private float _energyDecreaseRatePerHour = 8.33f; // 12 hours to reach 0
    private float _funDecreaseRatePerHour = 28.57f; // 3.5 hours to reach 0

    private const string HungerKey = "Hunger";
    private const string ThirstKey = "Thirst";
    private const string CleanlinessKey = "Cleanliness";
    private const string FunKey = "Fun";
    private const string HappinessKey = "Happiness";
    private const string EnergyKey = "Energy";
    private const string HealthStatusKey = "HealthStatus";
    private const string LowHappinessDurationKey = "LowHappinessDuration";
    private const string LastHealthCheckTimeKey = "LastHealthCheckTime";

    public enum HealthStatus { Healthy, Sick }
    private HealthStatus _currentHealthStatus = HealthStatus.Healthy;
    // Public property to expose _currentHealthStatus
    public HealthStatus CurrentHealthStatus { get { return _currentHealthStatus; } }
    private float _lowHappinessDuration = 0f; // Time in hours the pet has been below the happiness threshold

    [Header("Energy")]
    private float _maxEnergy = 100f;
    private float _currentEnergy;
    public float EnergyPercent => _currentEnergy / _maxEnergy;


    [Header("Hunger")]
    private float _maxHunger = 100f;
    private float _currentHunger;
    public float HungerPercent => _currentHunger / _maxHunger;

    [Header("Thirst")]
    private float _maxThirst = 100f;
    private float _currentThirst;
    public float ThirstPercent => _currentThirst / _maxHunger;

    [Header("Cleanliness")]
    private float _maxCleanliness = 100f;
    private float _currentCleanliness;
    public float CleanlinessPercent => _currentCleanliness / _maxCleanliness;

    [Header("Fun")]
    private float _maxFun = 100f;
    private float _currentFun;
    public float FunPercent => _currentFun / _maxFun;

    [Header("Happiness")]
    private float _maxHappiness = 100f;
    private float _currentHappiness;
    public float HappinessPercent => _currentHappiness / _maxHappiness;
    public static UnityAction OnPlayerDeath;

    private DateTime _lastUpdateTime;
    private DateTime _petBirthTime;
    private TimeSpan _currentPetAge;
    public EmotionAnimations emotionAnimator;

    public UIManager uiManager;

    private const string IsPetCreatedKey = "IsPetCreated";

    private void Start()
    {
        CheckJustBorn();
        petBehavior = GetComponent<PetBehavior>();
        stageAnimator = GetComponent<Animator>();
        UpdateLifeStage();
        CurrentLifeStage = (LifeStage)PlayerPrefs.GetInt("CurrentLifeStage");
        Debug.Log("Current Life Stage: " + CurrentLifeStage.ToString());

        string birthTimeStr = PlayerPrefs.GetString("PetBirthTime", "");
        if (!string.IsNullOrEmpty(birthTimeStr))
        {
            long tempBirthTime;
            if (long.TryParse(birthTimeStr, out tempBirthTime))
            {
                _petBirthTime = DateTime.FromBinary(tempBirthTime);
            }
        }
        else
        {
            _petBirthTime = DateTime.Now;
            PlayerPrefs.SetString("PetBirthTime", _petBirthTime.ToBinary().ToString());
            PlayerPrefs.Save();
        }

        string lastPlayTimeStr = PlayerPrefs.GetString("LastPlayTime", "");
        DateTime lastPlayTime;
        if (!string.IsNullOrEmpty(lastPlayTimeStr) && long.TryParse(lastPlayTimeStr, out long tempLastPlayTime))
        {
            lastPlayTime = DateTime.FromBinary(tempLastPlayTime);
        }
        else
        {
            lastPlayTime = DateTime.Now;
            PlayerPrefs.SetString("LastPlayTime", lastPlayTime.ToBinary().ToString());
        }

        TimeSpan timeSinceLastPlay = DateTime.Now - lastPlayTime;
        ApplyTimePassedToStats(timeSinceLastPlay);

        //LoadStats(out _currentHunger, out _currentThirst, out _currentCleanliness, out _currentFun, out _currentHappiness, out _currentEnergy);

        TimeSpan timePassed = DateTime.Now - lastPlayTime;
        _currentHunger = Mathf.Clamp(_currentHunger, 0, _maxHunger);
        _currentThirst = Mathf.Clamp(_currentThirst, 0, _maxThirst);
        _currentCleanliness = Mathf.Clamp(_currentCleanliness, 0, _maxCleanliness);
        _currentFun = Mathf.Clamp(_currentFun, 0, _maxFun);
        _currentHappiness = Mathf.Clamp(_currentHappiness, 0, _maxHappiness);
        _currentEnergy = Mathf.Clamp(_currentEnergy, 0, _maxEnergy);

        UpdateHealthStatus();
        ApplyLifeStageToAnimator(CurrentLifeStage);
        _lastUpdateTime = DateTime.Now;
        PlayerPrefs.SetString("LastPlayTime", _lastUpdateTime.ToBinary().ToString());
        PlayerPrefs.Save();
        Debug.Log($"Current Hunger: {_currentHunger}, Current Thirst: {_currentThirst}, Current Cleanliness: {_currentCleanliness}, Current Fun: {_currentFun}, Current Happiness: {_currentHappiness}");
        Debug.Log($"Time Passed: {timePassed}");
    }


    public void CheckJustBorn()
    {
       // Check if it's the first time creating the pet
        if (PlayerPrefs.GetInt(IsPetCreatedKey, 0) == 0)
        {
            // It's the first time, set all needs to 50%
            _currentHunger = _maxHunger * 0.5f;
            _currentThirst = _maxThirst * 0.5f;
            _currentCleanliness = _maxCleanliness * 0.5f;
            _currentFun = _maxFun * 0.5f;
            _currentHappiness = _maxHappiness * 0.5f;
            _currentEnergy = _maxEnergy * 0.5f;

            // Now mark that the pet has been created
            PlayerPrefs.SetInt(IsPetCreatedKey, 1);
        }
        else
        {
            // Not the first time, load stats from PlayerPrefs or set to full
            LoadStats(out _currentHunger, out _currentThirst, out _currentCleanliness, out _currentFun, out _currentHappiness, out _currentEnergy);
        }
    }
    private bool _hasPlayerDied;
    private void Update()
    {
        // Calculate time passed since the last frame using DateTime.
        DateTime currentTime = DateTime.Now;
        TimeSpan timePassed = currentTime - _lastUpdateTime;
        _lastUpdateTime = currentTime;

        // Apply the time passed to the stats
        ApplyTimePassedToStats(timePassed);

        // Other game logic
        ApplyLifeStageToAnimator(CurrentLifeStage);
        UpdateLifeStage();
        CalculateHappiness();
        UpdateHealthStatus();

        // Handle player death or any other relevant logic.
        if (!_hasPlayerDied && (_lowHappinessDuration >= 42f || CurrentLifeStage == LifeStage.Death))
        {
            emotionAnimator.enabled = false;
            uiManager.GameOver();
            _hasPlayerDied = true;
            CurrentLifeStage = LifeStage.Death;
            stageAnimator.enabled = false;
            OnPlayerDeath?.Invoke();
        }

    }


    private void ApplyTimePassedToStats(TimeSpan timePassed)
    {
        float hoursPassed = (float)timePassed.TotalHours;

        //calculate decrease rates
        _currentHunger = Mathf.Clamp(_currentHunger - _hungerDecreaseRatePerHour * hoursPassed, 0, _maxHunger);
        _currentThirst = Mathf.Clamp(_currentThirst - _thirstDecreaseRatePerHour * hoursPassed, 0, _maxThirst);
        _currentCleanliness = Mathf.Clamp(_currentCleanliness - _cleanDecreaseRatePerHour * hoursPassed, 0, _maxCleanliness);
        _currentFun = Mathf.Clamp(_currentFun - _funDecreaseRatePerHour * hoursPassed, 0, _maxFun);
        _currentHappiness = Mathf.Clamp(_currentHappiness - _funDecreaseRatePerHour * hoursPassed, 0, _maxHappiness);
        _currentEnergy = Mathf.Clamp(_currentEnergy - _energyDecreaseRatePerHour * hoursPassed, 0, _maxEnergy);

        // Update health status based on new low happiness duration
        UpdateHealthStatus();
    }


    public void ResetPet()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt(IsPetCreatedKey, 0);
        PlayerPrefs.Save();
        // Reset UI Elements
        uiManager.StartGame();
        //uiMnanager.UpdateStageText(GetFormattedStage());
        _currentHunger = _maxHunger;
        _currentThirst = _maxThirst;
        _currentCleanliness = _maxCleanliness;
        _currentFun = _maxFun;
        _currentHappiness = _maxHappiness;
        _currentEnergy = _maxEnergy;
        _currentHealthStatus = HealthStatus.Healthy;
        _lowHappinessDuration = 0f;
        _lastUpdateTime = DateTime.Now;
        _petBirthTime = DateTime.Now;
        _currentPetAge = TimeSpan.Zero;
        CurrentLifeStage = LifeStage.Baby;
        emotionAnimator.enabled = true;
        emotionAnimator.VeryHappyMood();
        petBehavior.ResetDeath();
        stageAnimator.enabled = true;
        _hasPlayerDied = false;
        ApplyLifeStageToAnimator(CurrentLifeStage);
    }

    public HealButtonEffect healButtonEffect;

    private void UpdateHealthStatus()
    {
        // Check if the happiness is below the threshold
        if (HappinessPercent < 0.2f) // 20%
        {
            // Increase the duration of low happiness
            _lowHappinessDuration += Time.deltaTime / 3600f; // Convert seconds to hours
        }
        else
        {
            // Reset the low happiness duration if happiness is above the threshold
            _lowHappinessDuration = 0f;
        }

        // Check if the pet has been unhappy for 30 hours or more
        if (_lowHappinessDuration >= 14f)
        {
            // Change the health status to sick
            _currentHealthStatus = HealthStatus.Sick;
            healButtonEffect.ButtonSick();
        }
        else
        {
            // Otherwise, ensure the health status is healthy
            _currentHealthStatus = HealthStatus.Healthy;
            healButtonEffect.ButtonHealthy();
        }
    }

    // Define weights for each need
    private float hungerWeight = 0.7f;
    private float thirstWeight = 0.6f;
    private float cleanlinessWeight = 1f;
    private float funWeight = 0.8f;
    private float energyWeight = 0.7f;
    private float happinessChangeRate = 0.0004f;

    private void CalculateHappiness()
    {
        // Calculate the weighted sum of the needs
        float totalWeight = hungerWeight + thirstWeight + cleanlinessWeight + funWeight + energyWeight;
        float weightedSum = HungerPercent * hungerWeight + ThirstPercent * thirstWeight + CleanlinessPercent * cleanlinessWeight + FunPercent * funWeight + EnergyPercent * energyWeight;

        // Calculate the weighted average
        float weightedAverage = weightedSum / totalWeight;

        // Calculate the change in happiness
        float happinessChange = (weightedAverage * _maxHappiness - _currentHappiness) * happinessChangeRate;

        // Update the current happiness with a moving average
        _currentHappiness += happinessChange;

        // Ensure that happiness does not go below 0 or above the maximum
        _currentHappiness = Mathf.Clamp(_currentHappiness, 0, _maxHappiness);
    }

    private void OnApplicationQuit()
    {
        // Save the current system time as a string in the player prefs class
        long currentSystemTime = System.DateTime.Now.ToBinary();
        PlayerPrefs.SetString("LastPlayTime", currentSystemTime.ToString());
        print("Saving this date to prefs: " + System.DateTime.Now);

        // Save current life stage
        PlayerPrefs.SetInt("CurrentLifeStage", (int)CurrentLifeStage);

        // No need to save the birth time again if it hasn't changed, but ensure it's there on first run
        if (!PlayerPrefs.HasKey("PetBirthTime"))
        {
            PlayerPrefs.SetString("PetBirthTime", _petBirthTime.ToBinary().ToString());
        }

        SaveStats(_currentHunger, _currentThirst, _currentCleanliness, _currentFun, _currentHappiness, _currentEnergy);
    }

    private void SaveStats(float currentHunger, float currentThirst, float currentCleanliness, float currentFun, float currentHappiness, float currentEnergy)
    {
        PlayerPrefs.SetFloat(HungerKey, currentHunger);
        PlayerPrefs.SetFloat(ThirstKey, currentThirst);
        PlayerPrefs.SetFloat(CleanlinessKey, currentCleanliness);
        PlayerPrefs.SetFloat(FunKey, currentFun);
        PlayerPrefs.SetFloat(HappinessKey, currentHappiness);
        PlayerPrefs.SetFloat(EnergyKey, currentEnergy);

        PlayerPrefs.SetInt(HealthStatusKey, (int)_currentHealthStatus);
        PlayerPrefs.SetFloat(LowHappinessDurationKey, _lowHappinessDuration);
        PlayerPrefs.SetString(LastHealthCheckTimeKey, DateTime.Now.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    private void LoadStats(out float currentHunger, out float currentThirst, out float currentCleanliness, out float currentFun, out float currentHappiness, out float currentEnergy)
    {
        currentHunger = PlayerPrefs.GetFloat(HungerKey, 100f); // Default value of 100
        currentThirst = PlayerPrefs.GetFloat(ThirstKey, 100f); // Default value of 100
        currentCleanliness = PlayerPrefs.GetFloat(CleanlinessKey, 100f); // Default value of 100
        currentFun = PlayerPrefs.GetFloat(FunKey, 100f); // Default value of 100
        currentHappiness = PlayerPrefs.GetFloat(HappinessKey, 100f); // Default value of 100
        currentEnergy = PlayerPrefs.GetFloat(EnergyKey, 100f); // Default value of 100

        if (PlayerPrefs.HasKey("LastPlayTime"))
        {
            // Get the old system time from player prefs as a string
            long temp = Convert.ToInt64(PlayerPrefs.GetString("LastPlayTime"));

            //CurrentLifeStage = (LifeStage)PlayerPrefs.GetInt("CurrentLifeStage");

            // Convert the old time from binary to a DateTime variable
            DateTime oldSystemTime = DateTime.FromBinary(temp);

            // Get the current system time
            DateTime currentSystemTime = System.DateTime.Now;

            // Calculate the time passed as a TimeSpan
            TimeSpan timePassed = currentSystemTime - oldSystemTime;

            // Convert timePassed to hours
            float timePassedInHours = (float)timePassed.TotalHours;

            currentHunger = PlayerPrefs.GetFloat(HungerKey, 100f); // Default value of 100
            currentThirst = PlayerPrefs.GetFloat(ThirstKey, 100f); // Default value of 100
            currentCleanliness = PlayerPrefs.GetFloat(CleanlinessKey, 100f); // Default value of 100
            currentFun = PlayerPrefs.GetFloat(FunKey, 100f); // Default value of 100
            currentHappiness = PlayerPrefs.GetFloat(HappinessKey, 100f); // Default value of 100
            currentEnergy = PlayerPrefs.GetFloat(EnergyKey, 100f); // Default value of 100

            // Decrease the stats based on the time passed.
            currentHunger -= _hungerDecreaseRatePerHour * timePassedInHours;
            currentThirst -= _thirstDecreaseRatePerHour * timePassedInHours;
            currentCleanliness -= _cleanDecreaseRatePerHour * timePassedInHours;
            currentFun -= _cleanDecreaseRatePerHour * timePassedInHours;
            currentHappiness -= _funDecreaseRatePerHour * timePassedInHours;
            currentEnergy -= _energyDecreaseRatePerHour * timePassedInHours;

            // Ensure that stats don't go below zero or exceed their maximum values.
            currentHunger = Mathf.Clamp(currentHunger, 0, _maxHunger);
            currentThirst = Mathf.Clamp(currentThirst, 0, _maxThirst);
            currentCleanliness = Mathf.Clamp(currentCleanliness, 0, _maxCleanliness);
            currentFun = Mathf.Clamp(currentFun, 0, _maxFun);
            currentHappiness = Mathf.Clamp(currentHappiness, 0, _maxHappiness);
            currentEnergy = Mathf.Clamp(currentEnergy, 0, _maxEnergy);
        }
    }

    public string GetFormattedAge()
    {
        if (CurrentLifeStage == LifeStage.Death)
        {
            return $"DAYS SURVIVED: {_currentPetAge.Days}";
        }

        _currentPetAge = DateTime.Now - _petBirthTime;  // Calculate current pet age
        // Return formatted age string
        return $"AGE:{_currentPetAge.Days} DAYS";
        // {currentPetAge.Hours} hours, {currentPetAge.Minutes} minutes, {currentPetAge.Seconds} seconds";
    }

    public PetName petName;

    public string GetFormattedName()
    {
        string petName = PlayerPrefs.GetString("PetName");
        return petName.ToString();
    }

    public string GetFormattedStage()
    {
        if (CurrentLifeStage == LifeStage.Death)
        {
            return null;
        }
        else
        {
            return $"STAGE:{CurrentLifeStage.ToString()}";
        }
    }


    public void UpdateLifeStage()
    {
        //TimeSpan age = CurrentPetAge;
        TimeSpan age = _currentPetAge;
        LifeStage previousStage = CurrentLifeStage;

        if (CurrentLifeStage == LifeStage.Baby && age >= babyTimeRequirement)
        {
            CurrentLifeStage = LifeStage.Child;
            Debug.Log("Transitioned to Child Stage");
        }
        else if (CurrentLifeStage == LifeStage.Child && age >= childTimeRequirement)
        {
            CurrentLifeStage = LifeStage.Adult;
            Debug.Log("Transitioned to Adult Stage");
        }
        else if (CurrentLifeStage == LifeStage.Adult && age >= adultTimeRequirement)
        {
            CurrentLifeStage = LifeStage.Senior;
            Debug.Log("Transitioned to Senior Stage");
        }
        else if (CurrentLifeStage == LifeStage.Senior && age >= seniorTimeRequirement)
        {
            CurrentLifeStage = LifeStage.Death;
            Debug.Log("Transitioned to Death Stage");
        }

        // If the life stage has changed, log the current stage.
        if (previousStage != CurrentLifeStage)
        {
            Debug.Log("Current Life Stage: " + CurrentLifeStage.ToString());
        }
    }

    private void ApplyLifeStageToAnimator(LifeStage stage)
    {
        switch (stage)
        {
            case LifeStage.Baby:
                stageAnimator.runtimeAnimatorController = babyController;
                break;
            case LifeStage.Child:
                stageAnimator.runtimeAnimatorController = childController;
                break;
            case LifeStage.Adult:
                stageAnimator.runtimeAnimatorController = adultController;
                break;
            case LifeStage.Senior:
                stageAnimator.runtimeAnimatorController = seniorController;
                break;
            case LifeStage.Death:
                stageAnimator.runtimeAnimatorController = deathController;
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// ACTION BUTTON METHODS
    /// </summary>
    public void IncreaseHunger(float hungerAmount)
    {
        _currentHunger += hungerAmount;
        if (_currentHunger > _maxHunger)
        {
            _currentHunger = _maxHunger;
        }
    }
    public void DecreaseHunger(float hungerAmount)
    {
        _currentHunger -= hungerAmount;
        if (_currentHunger < 0)
        {
            _currentHunger = 0;
        }
    }

    public void IncreaseThirst(float thirstAmount)
    {
        _currentThirst += thirstAmount;
        if (_currentThirst > _maxThirst)
        {
            _currentThirst = _maxThirst;
        }
    }
    public void DecreaseThirst(float thirstAmount)
    {
        _currentThirst -= thirstAmount;
        if (_currentThirst < 0)
        {
            _currentThirst = 0;
        }
    }

    public void IncreaseCleanliness(float cleanlinessAmount)
    {
        _currentCleanliness += cleanlinessAmount;
        if (_currentCleanliness > _maxCleanliness)
        {
            _currentCleanliness = _maxCleanliness;
        }
    }

    public void DecreaseCleanliness(float cleanlinessAmount)
    {
        _currentCleanliness -= cleanlinessAmount;
        if (_currentCleanliness < 0)
        {
            _currentCleanliness = 0;
        }
    }

    public void IncreaseFun(float funAmount)
    {
        _currentFun += funAmount;
        if (_currentFun > _maxFun)
        {
            _currentFun = _maxFun;
        }
    }

    public void DecreaseFun(float funAmount)
    {
        _currentFun -= funAmount;
        if (_currentFun < 0)
        {
            _currentFun = 0;
        }
    }

    public void IncreaseHappiness(float happinessAmount)
    {
        _currentHappiness += happinessAmount;
        if (_currentHappiness > _maxHappiness)
        {
            _currentHappiness = _maxHappiness;
        }
    }

    public void DecreaseHappiness(float happinessAmount)
    {
        _currentHappiness -= happinessAmount;
        if (_currentHappiness < 0)
        {
            _currentHappiness = 0;
        }
    }

    public void IncreaseEnergy(float energyAmount)
    {
        _currentEnergy += energyAmount;
        if (_currentEnergy > _maxEnergy)
        {
            _currentEnergy = _maxEnergy;
        }
    }

    public float DecreaseEnergy(float energyAmount)
    {
        _currentEnergy -= energyAmount;
        if (_currentEnergy < 0)
        {
            _currentEnergy = 0;
        }
        return _currentEnergy;
    }
    public void HealPet()
    {
        _lowHappinessDuration = 0f;
    }
}
