using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class StatsManager : MonoBehaviour
{
    private Animator stageAnimator;
     public RuntimeAnimatorController eggController;
    public RuntimeAnimatorController babyController;
    public RuntimeAnimatorController adultController;
    public RuntimeAnimatorController seniorController;
    public RuntimeAnimatorController deathController;

    public enum LifeStage { Egg, Baby, Adult, Senior, Death }
    public LifeStage CurrentLifeStage { get; private set; } = LifeStage.Egg;

    // // Time requirements for each stage
    private readonly TimeSpan eggTimeRequirement = TimeSpan.FromSeconds(5);
    private readonly TimeSpan babyTimeRequirement = TimeSpan.FromSeconds(15);
    private readonly TimeSpan adultTimeRequirement = TimeSpan.FromSeconds(25);
    private readonly TimeSpan seniorTimeRequirement = TimeSpan.FromSeconds(35);


    private float _hungerDecreaseRatePerHour = 30000f;
    private float _thirstDecreaseRatePerHour = 30000f;
    private float _cleanDecreaseRatePerHour = 30000f;
    private float _energyDecreaseRatePerHour = 30000f;

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

     // Provide public access to the DateTime and TimeSpan variables
    // public DateTime LastUpdateTime => _lastUpdateTime;
    // public DateTime PetBirthTime => _petBirthTime;
    // public TimeSpan CurrentPetAge => _currentPetAge;



    private void Start()
    {
        stageAnimator = GetComponent<Animator>();
        UpdateLifeStage();
        // Load current life stage
        CurrentLifeStage = (LifeStage)PlayerPrefs.GetInt("CurrentLifeStage");
        //CurrentLifeStage = (LifeStage)PlayerPrefs.GetInt("CurrentLifeStage", (int)LifeStage.Egg);
        Debug.Log("Current Life Stage: " + CurrentLifeStage.ToString());
        
        string birthTimeStr = PlayerPrefs.GetString("PetBirthTime", "");
        if (!string.IsNullOrEmpty(birthTimeStr))
        {
            long temp;
            if (long.TryParse(birthTimeStr, out temp))
            {
                _petBirthTime = DateTime.FromBinary(temp);
            }
        }
        else
        {
            _petBirthTime = DateTime.Now;
            PlayerPrefs.SetString("PetBirthTime", _petBirthTime.ToBinary().ToString());
            PlayerPrefs.Save();
        }


        // Initialize _lastUpdateTime using PlayerPrefs or current time
        string lastPlayTimeStr = PlayerPrefs.GetString("LastPlayTime", "");
        if (!string.IsNullOrEmpty(lastPlayTimeStr))
        {
            long temp;
            if (long.TryParse(lastPlayTimeStr, out temp))
            {
                _lastUpdateTime = DateTime.FromBinary(temp);
                TimeSpan timeSinceLastPlay = DateTime.Now - _lastUpdateTime;
                _currentPetAge += timeSinceLastPlay;
            }
            else
            {
                // Set a default value for _lastUpdateTime if parsing fails
                _lastUpdateTime = DateTime.Now;
                PlayerPrefs.SetString("LastPlayTime", _lastUpdateTime.ToBinary().ToString());
                PlayerPrefs.Save();
            }
        }
        else
        {
            _lastUpdateTime = DateTime.Now;
            PlayerPrefs.SetString("LastPlayTime", _lastUpdateTime.ToBinary().ToString());
            PlayerPrefs.Save();
        }

        // Load the last saved health status and low happiness duration
        _currentHealthStatus = (HealthStatus)PlayerPrefs.GetInt(HealthStatusKey, 0); // Assuming 0 is healthy, 1 is sick
        _lowHappinessDuration = PlayerPrefs.GetFloat(LowHappinessDurationKey, 0f);

        string lastHealthCheckTimeStr = PlayerPrefs.GetString(LastHealthCheckTimeKey, "");
        if (!string.IsNullOrEmpty(lastHealthCheckTimeStr))
        {
            DateTime lastHealthCheckTime = DateTime.FromBinary(Convert.ToInt64(lastHealthCheckTimeStr));
            TimeSpan timePassedSinceLastCheck = DateTime.Now - lastHealthCheckTime;

            // Update the low happiness duration based on the time passed
            if (_currentHealthStatus == HealthStatus.Sick || HappinessPercent < 0.2f)
            {
                _lowHappinessDuration += (float)timePassedSinceLastCheck.TotalHours;
            }
            // Cap the _lowHappinessDuration to a maximum to avoid excessive accumulation
            _lowHappinessDuration = Mathf.Min(_lowHappinessDuration, 12f);
        }
        else
        {
            // If there's no last check time, set the current time as the last check time
            PlayerPrefs.SetString(LastHealthCheckTimeKey, DateTime.Now.ToBinary().ToString());
        }

        // Check if PlayerPrefs have been set for hunger and thirst, and if not, use default values.
        if (!PlayerPrefs.HasKey(HungerKey) || !PlayerPrefs.HasKey(ThirstKey) || !PlayerPrefs.HasKey(CleanlinessKey) || !PlayerPrefs.HasKey(FunKey) || !PlayerPrefs.HasKey(HappinessKey) || !PlayerPrefs.HasKey(EnergyKey))
        {
            _currentHunger = _maxHunger; // Set a default value for hunger
            _currentThirst = _maxThirst; // Set a default value for thirst
            _currentCleanliness = _maxCleanliness; // Set a default value for cleanliness
            _currentFun = _maxFun; // Set a default value for fun
            _currentHappiness = _maxHappiness; // Set a default value for happiness
            _currentEnergy = _maxEnergy; // Set a default value for energy
        }
        else
        {
            // Load the saved hunger and thirst values from PlayerPrefs
            _currentHunger = PlayerPrefs.GetFloat(HungerKey, _maxHunger);
            _currentThirst = PlayerPrefs.GetFloat(ThirstKey, _maxThirst);
            _currentCleanliness = PlayerPrefs.GetFloat(CleanlinessKey, _maxCleanliness);
            _currentFun = PlayerPrefs.GetFloat(FunKey, _maxFun);
            _currentHappiness = PlayerPrefs.GetFloat(HappinessKey, _maxHappiness);
            _currentEnergy = PlayerPrefs.GetFloat(EnergyKey, _maxEnergy);
        }

        // Load player stats (hunger and thirst) from PlayerPrefs
        LoadStats(out _currentHunger, out _currentThirst, out _currentCleanliness, out _currentFun, out _currentHappiness, out _currentEnergy);

        // Calculate time passed using TimeSpan
        TimeSpan timePassed = DateTime.Now - _lastUpdateTime;

        // Ensure that stats don't go below zero or exceed their maximum values.
        _currentHunger = Mathf.Clamp(_currentHunger, 0, _maxHunger);
        _currentThirst = Mathf.Clamp(_currentThirst, 0, _maxThirst);
        _currentCleanliness = Mathf.Clamp(_currentCleanliness, 0, _maxCleanliness);
        _currentFun = Mathf.Clamp(_currentFun, 0, _maxFun);
        _currentHappiness = Mathf.Clamp(_currentHappiness, 0, _maxHappiness);
        _currentEnergy = Mathf.Clamp(_currentEnergy, 0, _maxEnergy);

        // Update health status based on new low happiness duration
        UpdateHealthStatus();
        ApplyLifeStageToAnimator(CurrentLifeStage);

        // Store the current time as the last update time
        _lastUpdateTime = DateTime.Now;
        PlayerPrefs.SetString("LastPlayTime", _lastUpdateTime.ToBinary().ToString());
        PlayerPrefs.Save();

        Debug.Log($"Current Hunger: {_currentHunger}, Current Thirst: {_currentThirst}, Current Cleanliness: {_currentCleanliness}, Current Fun: {_currentFun}, Current Happiness: {_currentHappiness}");
        Debug.Log($"Time Passed: {timePassed}");
        Debug.Log($"Time IN Seconds Passed: {timePassed.TotalSeconds} seconds");

    }

    private void Update()
    {
        // Calculate time passed since the last frame using DateTime.
        DateTime currentTime = DateTime.Now;
        TimeSpan timePassed = currentTime - _lastUpdateTime;
        _lastUpdateTime = currentTime;
        ApplyLifeStageToAnimator(CurrentLifeStage);

        // Convert time passed to hours (assuming your decrease rates are per hour).
        float hoursPassed = (float)timePassed.TotalHours;

        // Decrease hunger and thirst based on the time passed and decrease rates per hour.
        float hungerDecrease = _hungerDecreaseRatePerHour * hoursPassed;
        float thirstDecrease = _thirstDecreaseRatePerHour * hoursPassed;
        float cleanDecrease = _cleanDecreaseRatePerHour * hoursPassed;
        float funDecrease = _cleanDecreaseRatePerHour * hoursPassed;
        float energyDecrease = _energyDecreaseRatePerHour * hoursPassed;
        UpdateLifeStage();
        CalculateHappiness();
        UpdateHealthStatus();

        if (_currentHunger > 0)
        {
            _currentHunger -= hungerDecrease;
            if (_currentHunger < 0)
            {
                _currentHunger = 0;
            }
        }

        if (_currentThirst > 0)
        {
            _currentThirst -= thirstDecrease;
            if (_currentThirst < 0)
            {
                _currentThirst = 0;
            }
        }

        if (_currentCleanliness > 0)
        {
            _currentCleanliness -= cleanDecrease;
            if (_currentCleanliness < 0)
            {
                _currentCleanliness = 0;
            }
        }

        if (_currentFun > 0)
        {
            _currentFun -= funDecrease;
            if (_currentFun < 0)
            {
                _currentFun = 0;
            }
        }

        if (_currentEnergy > 0)
        {
            _currentEnergy -= energyDecrease;
            if (_currentEnergy < 0)
            {
                _currentEnergy = 0;
            }
        }

        if (_currentHunger <= 0 && _currentThirst <= 0 && _currentCleanliness <= 0)
        {
            // Handle player death or any other relevant logic.
            // OnPlayerDeath?.Invoke();
            // Debug.Log("You died");
            _currentHunger = 0;
            _currentThirst = 0;
        }
    }


    private void UpdateHealthStatus()
    {
        // Check if the happiness is below the threshold
        if (HappinessPercent < 0.5f) // 20%
        {
            // Increase the duration of low happiness
            _lowHappinessDuration += Time.deltaTime / 3600f; // Convert seconds to hours
        }
        else
        {
            // Reset the low happiness duration if happiness is above the threshold
            _lowHappinessDuration = 0f;
        }

        // Check if the pet has been unhappy for 12 hours or more
        if (_lowHappinessDuration >= 0.0100f) // 5 minutes currently.
        {
            // Change the health status to sick
            _currentHealthStatus = HealthStatus.Sick;
        }
        else
        {
            // Otherwise, ensure the health status is healthy
            _currentHealthStatus = HealthStatus.Healthy;
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
            currentHappiness -= _cleanDecreaseRatePerHour * timePassedInHours;
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

    public string GetFormattedStage()
    {
        //return CurrentLifeStage.ToString();

        if(CurrentLifeStage == LifeStage.Death)
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

        if (CurrentLifeStage == LifeStage.Egg && age >= eggTimeRequirement)
        {
            CurrentLifeStage = LifeStage.Baby;
            Debug.Log("Transitioned to Baby Stage");
        }
        else if (CurrentLifeStage == LifeStage.Baby && age >= babyTimeRequirement)
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
            case LifeStage.Egg:
                stageAnimator.runtimeAnimatorController = eggController;
                break;
            case LifeStage.Baby:
                stageAnimator.runtimeAnimatorController = babyController;
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
