using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatsManager : MonoBehaviour
{
    private float _hungerDecreaseRatePerHour = 4600f;
    private float _thirstDecreaseRatePerHour = 1800f;
    private float _cleanDecreaseRatePerHour = 1800f;

    private const string HungerKey = "Hunger";
    private const string ThirstKey = "Thirst";
    private const string CleanlinessKey = "Cleanliness";
    private const string FunKey = "Fun";
    private const string HappinessKey = "Happiness";
    private const string HealthStatusKey = "HealthStatus";
    private const string LowHappinessDurationKey = "LowHappinessDuration";
    private const string LastHealthCheckTimeKey = "LastHealthCheckTime";

    public enum HealthStatus { Healthy, Sick }
    private HealthStatus _currentHealthStatus = HealthStatus.Healthy;
    // Public property to expose _currentHealthStatus
    public HealthStatus CurrentHealthStatus { get { return _currentHealthStatus; } }
    private float _lowHappinessDuration = 0f; // Time in hours the pet has been below the happiness threshold


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

    private void Start()
    {
        // Initialize _lastUpdateTime using PlayerPrefs or current time
        string lastPlayTimeStr = PlayerPrefs.GetString("LastPlayTime");
        if (!string.IsNullOrEmpty(lastPlayTimeStr))
        {
            long temp;
            if (long.TryParse(lastPlayTimeStr, out temp))
            {
                _lastUpdateTime = DateTime.FromBinary(temp);
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
        if (!PlayerPrefs.HasKey(HungerKey) || !PlayerPrefs.HasKey(ThirstKey) || !PlayerPrefs.HasKey(CleanlinessKey) || !PlayerPrefs.HasKey(FunKey) || !PlayerPrefs.HasKey(HappinessKey))
        {
            _currentHunger = _maxHunger; // Set a default value for hunger
            _currentThirst = _maxThirst; // Set a default value for thirst
            _currentCleanliness = _maxCleanliness; // Set a default value for cleanliness
            _currentFun = _maxFun; // Set a default value for fun
            _currentHappiness = _maxHappiness; // Set a default value for happiness
        }
        else
        {
            // Load the saved hunger and thirst values from PlayerPrefs
            _currentHunger = PlayerPrefs.GetFloat(HungerKey, _maxHunger);
            _currentThirst = PlayerPrefs.GetFloat(ThirstKey, _maxThirst);
            _currentCleanliness = PlayerPrefs.GetFloat(CleanlinessKey, _maxCleanliness);
            _currentFun = PlayerPrefs.GetFloat(FunKey, _maxFun);
            _currentHappiness = PlayerPrefs.GetFloat(HappinessKey, _maxHappiness);
        }

        // Load player stats (hunger and thirst) from PlayerPrefs
        LoadStats(out _currentHunger, out _currentThirst, out _currentCleanliness, out _currentFun, out _currentHappiness);

        // Calculate time passed using TimeSpan
        TimeSpan timePassed = DateTime.Now - _lastUpdateTime;

        // Ensure that stats don't go below zero or exceed their maximum values.
        _currentHunger = Mathf.Clamp(_currentHunger, 0, _maxHunger);
        _currentThirst = Mathf.Clamp(_currentThirst, 0, _maxThirst);
        _currentCleanliness = Mathf.Clamp(_currentCleanliness, 0, _maxCleanliness);
        _currentFun = Mathf.Clamp(_currentFun, 0, _maxFun);
        _currentHappiness = Mathf.Clamp(_currentHappiness, 0, _maxHappiness);

        // Update health status based on new low happiness duration
        UpdateHealthStatus();

        // Store the current time as the last update time
        _lastUpdateTime = DateTime.Now;
        PlayerPrefs.SetString("LastPlayTime", _lastUpdateTime.ToBinary().ToString());
        PlayerPrefs.Save();

        Debug.Log($"Current Hunger: {_currentHunger}, Current Thirst: {_currentThirst}, Current Cleanliness: {_currentCleanliness}, Current Fun: {_currentFun}, Current Happiness: {_currentHappiness}");
        Debug.Log($"Time Passed: {timePassed}");
    }

    private void Update()
    {
        // Calculate time passed since the last frame using DateTime.
        DateTime currentTime = DateTime.Now;
        TimeSpan timePassed = currentTime - _lastUpdateTime;
        _lastUpdateTime = currentTime;

        // Convert time passed to hours (assuming your decrease rates are per hour).
        float hoursPassed = (float)timePassed.TotalHours;

        // Decrease hunger and thirst based on the time passed and decrease rates per hour.
        float hungerDecrease = _hungerDecreaseRatePerHour * hoursPassed;
        float thirstDecrease = _thirstDecreaseRatePerHour * hoursPassed;
        float cleanDecrease = _cleanDecreaseRatePerHour * hoursPassed;
        float funDecrease = _cleanDecreaseRatePerHour * hoursPassed;
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
    private float hungerWeight = 2f;
    private float thirstWeight = 1.5f;
    private float cleanlinessWeight = 1.1f;
    private float funWeight = 1f;

    private void CalculateHappiness()
    {
        // Calculate the weighted sum of the needs
        float totalWeight = hungerWeight + thirstWeight + cleanlinessWeight + funWeight;
        float weightedSum = HungerPercent * hungerWeight + ThirstPercent * thirstWeight + CleanlinessPercent * cleanlinessWeight + FunPercent * funWeight;

        // Calculate the weighted average
        float weightedAverage = weightedSum / totalWeight;

        // Assign this weighted average to the current happiness
        _currentHappiness = weightedAverage * _maxHappiness;

        // Ensure that happiness does not go below 0 or above the maximum
        _currentHappiness = Mathf.Clamp(_currentHappiness, 0, _maxHappiness);
    }


    private void OnApplicationQuit()
    {
        // Save the current system time as a string in the player prefs class
        long currentSystemTime = System.DateTime.Now.ToBinary();
        PlayerPrefs.SetString("LastPlayTime", currentSystemTime.ToString());
        print("Saving this date to prefs: " + System.DateTime.Now);
        SaveStats(_currentHunger, _currentThirst, _currentCleanliness, _currentFun, _currentHappiness);
    }

    private void SaveStats(float currentHunger, float currentThirst, float currentCleanliness, float currentFun, float currentHappiness)
    {
        PlayerPrefs.SetFloat(HungerKey, currentHunger);
        PlayerPrefs.SetFloat(ThirstKey, currentThirst);
        PlayerPrefs.SetFloat(CleanlinessKey, currentCleanliness);
        PlayerPrefs.SetFloat(FunKey, currentFun);
        PlayerPrefs.SetFloat(HappinessKey, currentHappiness);

        PlayerPrefs.SetInt(HealthStatusKey, (int)_currentHealthStatus);
        PlayerPrefs.SetFloat(LowHappinessDurationKey, _lowHappinessDuration);
        PlayerPrefs.SetString(LastHealthCheckTimeKey, DateTime.Now.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    private void LoadStats(out float currentHunger, out float currentThirst, out float currentCleanliness, out float currentFun, out float currentHappiness)
    {
        currentHunger = PlayerPrefs.GetFloat(HungerKey, 100f); // Default value of 100
        currentThirst = PlayerPrefs.GetFloat(ThirstKey, 100f); // Default value of 100
        currentCleanliness = PlayerPrefs.GetFloat(CleanlinessKey, 100f); // Default value of 100
        currentFun = PlayerPrefs.GetFloat(FunKey, 100f); // Default value of 100
        currentHappiness = PlayerPrefs.GetFloat(HappinessKey, 100f); // Default value of 100

        if (PlayerPrefs.HasKey("LastPlayTime"))
        {
            // Get the old system time from player prefs as a string
            long temp = Convert.ToInt64(PlayerPrefs.GetString("LastPlayTime"));

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

            // Decrease the stats based on the time passed.
            currentHunger -= _hungerDecreaseRatePerHour * timePassedInHours;
            currentThirst -= _thirstDecreaseRatePerHour * timePassedInHours;
            currentCleanliness -= _cleanDecreaseRatePerHour * timePassedInHours;
            currentFun -= _cleanDecreaseRatePerHour * timePassedInHours;
            currentHappiness -= _cleanDecreaseRatePerHour * timePassedInHours;

            // Ensure that stats don't go below zero or exceed their maximum values.
            currentHunger = Mathf.Clamp(currentHunger, 0, _maxHunger);
            currentThirst = Mathf.Clamp(currentThirst, 0, _maxThirst);
            currentCleanliness = Mathf.Clamp(currentCleanliness, 0, _maxCleanliness);
            currentFun = Mathf.Clamp(currentFun, 0, _maxFun);
            currentHappiness = Mathf.Clamp(currentHappiness, 0, _maxHappiness);
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

    public void HealPet()
    {
         _lowHappinessDuration = 0f;
    }
}
