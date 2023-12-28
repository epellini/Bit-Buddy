using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatsManager : MonoBehaviour
{
    private float _hungerDecreaseRatePerHour = 4600f;
    private float _thirstDecreaseRatePerHour = 1800f;
    private const string HungerKey = "Hunger";
    private const string ThirstKey = "Thirst";

    [Header("Hunger")]
    private float _maxHunger = 100f;
    private float _currentHunger;
    public float HungerPercent => _currentHunger / _maxHunger;

    [Header("Thirst")]
    private float _maxThirst = 100f;
    private float _currentThirst;
    public float ThirstPercent => _currentThirst / _maxHunger;

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

        // Check if PlayerPrefs have been set for hunger and thirst, and if not, use default values.
        if (!PlayerPrefs.HasKey(HungerKey) || !PlayerPrefs.HasKey(ThirstKey))
        {
            _currentHunger = _maxHunger; // Set a default value for hunger
            _currentThirst = _maxThirst; // Set a default value for thirst
        }
        else
        {
            // Load the saved hunger and thirst values from PlayerPrefs
            _currentHunger = PlayerPrefs.GetFloat(HungerKey, _maxHunger);
            _currentThirst = PlayerPrefs.GetFloat(ThirstKey, _maxThirst);
        }


        // Load player stats (hunger and thirst) from PlayerPrefs
        LoadStats(out _currentHunger, out _currentThirst);

        // Calculate time passed using TimeSpan
        TimeSpan timePassed = DateTime.Now - _lastUpdateTime;

        // // Decrease hunger and thirst based on the time passed and decrease rates per hour.
        // float hungerDecrease = (_hungerDecreaseRatePerHour / 3600) * (float)timePassed.TotalSeconds;
        // float thirstDecrease = (_thirstDecreaseRatePerHour / 3600) * (float)timePassed.TotalSeconds;

        // _currentHunger -= hungerDecrease;
        // _currentThirst -= thirstDecrease;

        // Ensure that stats don't go below zero or exceed their maximum values.
        _currentHunger = Mathf.Clamp(_currentHunger, 0, _maxHunger);
        _currentThirst = Mathf.Clamp(_currentThirst, 0, _maxThirst);

        // Store the current time as the last update time
        _lastUpdateTime = DateTime.Now;

        PlayerPrefs.SetString("LastPlayTime", _lastUpdateTime.ToBinary().ToString());
        PlayerPrefs.Save();

        Debug.Log($"Current Hunger: {_currentHunger}, Current Thirst: {_currentThirst}");
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

        if (_currentHunger <= 0 && _currentThirst <= 0)
        {
            // Handle player death or any other relevant logic.
            // OnPlayerDeath?.Invoke();
            // Debug.Log("You died");
            _currentHunger = 0;
            _currentThirst = 0;
        }
    }


    public void IncreaseHunger(float hungerAmount)
    {
        _currentHunger += hungerAmount;
        if (_currentHunger > _maxHunger)
        {
            _currentHunger = _maxHunger;
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

    private void OnApplicationQuit()
    {
        // Save the current system time as a string in the player prefs class
        long currentSystemTime = System.DateTime.Now.ToBinary();
        PlayerPrefs.SetString("sysString", currentSystemTime.ToString());

        print("Saving this date to prefs: " + System.DateTime.Now);
        SaveStats(_currentHunger, _currentThirst);
    }

    private void SaveStats(float currentHunger, float currentThirst)
    {
        PlayerPrefs.SetFloat(HungerKey, currentHunger);
        PlayerPrefs.SetFloat(ThirstKey, currentThirst);
        PlayerPrefs.Save();
    }

    private void LoadStats(out float currentHunger, out float currentThirst)
    {
        currentHunger = PlayerPrefs.GetFloat(HungerKey, 100f); // Default value of 100
        currentThirst = PlayerPrefs.GetFloat(ThirstKey, 100f); // Default value of 100
        
        if (PlayerPrefs.HasKey("sysString"))
        {
            // Get the old system time from player prefs as a string
            long temp = Convert.ToInt64(PlayerPrefs.GetString("sysString"));

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

            // Decrease the stats based on the time passed.
            currentHunger -= _hungerDecreaseRatePerHour * timePassedInHours;
            currentThirst -= _thirstDecreaseRatePerHour * timePassedInHours;

            // Ensure that stats don't go below zero or exceed their maximum values.
            currentHunger = Mathf.Clamp(currentHunger, 0, _maxHunger);
            currentThirst = Mathf.Clamp(currentThirst, 0, _maxThirst);
        }
    }
}




