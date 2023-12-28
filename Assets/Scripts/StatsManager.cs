using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatsManager : MonoBehaviour
{
    private float _hungerDecreaseRatePerHour = 3000f;
    private float _thirstDecreaseRatePerHour = 3400f;
    private const string HungerKey = "Hunger";
    private const string ThirstKey = "Thirst";

    [Header("Hunger")]
    private float _maxHunger = 100f;
    private float _currentHunger;
    public float HungerPercent => _currentHunger / _maxHunger;

    [Header("Thirst")]
    private float _maxThirst = 100f;
    private float _currentThirst;
    public float ThirstPercent => _currentThirst / _maxThirst;

    public static UnityAction OnPlayerDeath;

    private float _lastUpdateTime;
    private void Start()
    {
        _lastUpdateTime = Time.realtimeSinceStartup; // Initialize _lastUpdateTime

        // Check if PlayerPrefs have been set for hunger and thirst, and if not, use default values.
        if (!PlayerPrefs.HasKey(HungerKey) || !PlayerPrefs.HasKey(ThirstKey))
        {
            _currentHunger = _maxHunger; // Set a default value for hunger
            _currentThirst = _maxThirst; // Set a default value for thirst
        }
        else
        {
            float loadedHunger, loadedThirst;
            LoadStats(out loadedHunger, out loadedThirst);
            
            // Retrieve the exit time when the game was last quit
            float lastPlayTime = PlayerPrefs.GetFloat("LastPlayTime", Time.realtimeSinceStartup);

            // Calculate time passed since the game was last played.
            float currentTime = Time.realtimeSinceStartup;
            //float lastPlayTime = PlayerPrefs.GetFloat("LastPlayTime", currentTime);

            // Ensure that timePassed is always positive.
            float timePassed = lastPlayTime > currentTime ? 0 : currentTime - lastPlayTime;

            // Update the stats based on the time passed.
            _currentHunger = loadedHunger - (_hungerDecreaseRatePerHour / 3600) * timePassed;
            _currentThirst = loadedThirst - (_thirstDecreaseRatePerHour / 3600) * timePassed;

            // Ensure that stats don't go below zero or exceed their maximum values.
            _currentHunger = Mathf.Clamp(_currentHunger, 0, _maxHunger);
            _currentThirst = Mathf.Clamp(_currentThirst, 0, _maxThirst);

            Debug.Log($"Loaded Hunger: {loadedHunger}, Loaded Thirst: {loadedThirst}");
            Debug.Log($"Current Hunger: {_currentHunger}, Current Thirst: {_currentThirst}");
            Debug.Log($"Time Passed: {timePassed}");
        }
    }


    private void Update()
    {
        // Calculate time passed since the last frame.
        float currentTime = Time.realtimeSinceStartup;
        float timePassed = currentTime - _lastUpdateTime;
        _lastUpdateTime = currentTime;

        // Decrease hunger and thirst based on the time passed and decrease rates per hour.
        float hungerDecrease = _hungerDecreaseRatePerHour / 3600 * timePassed;
        float thirstDecrease = _thirstDecreaseRatePerHour / 3600 * timePassed;

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
            //OnPlayerDeath?.Invoke();
            //Debug.Log("You died");
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
        //PlayerPrefs.SetFloat("LastPlayTime", _lastUpdateTime); // Save the initial time when quitting
        PlayerPrefs.SetFloat("LastPlayTime", Time.realtimeSinceStartup); // Save the initial time when quitting
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
        float lastPlayTime = PlayerPrefs.GetFloat("LastPlayTime", Time.realtimeSinceStartup);
        float currentTime = Time.realtimeSinceStartup;
        float timePassed =  currentTime - lastPlayTime;

        currentHunger = PlayerPrefs.GetFloat(HungerKey, 100f); // Default value of 100
        currentThirst = PlayerPrefs.GetFloat(ThirstKey, 100f); // Default value of 100

        // Decrease the stats based on the time passed.
        currentHunger += _hungerDecreaseRatePerHour / 3600 * timePassed;
        currentThirst += _thirstDecreaseRatePerHour / 3600 * timePassed;

        // Ensure that stats don't go below zero or exceed their maximum values.
        currentHunger = Mathf.Clamp(currentHunger, 0, _maxHunger);
        currentThirst = Mathf.Clamp(currentThirst, 0, _maxThirst);
    }
}




