using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PetBehavior : MonoBehaviour
{
    public ConsoleMessages consoleMessages;
    public StatsManager statsManager;
    public enum PetMood { Happy, Normal, Angry, Sad, Bored, Hungry, Sick }
    private PetMood currentMood = PetMood.Normal;
    private float lastAngryTime;

    // Variables for feed tracking
    private int rapidFeedAttempts = 0;
    private const int MaxFeedAttemptsBeforeAngry = 3;
    private float lastFeedTime;
    private const float FeedCooldown = 5.0f; // Time in seconds before counter resets

    // Variables for drink tracking
    private int rapidDrinkAttempts = 0;
    private const int MaxDrinkAttemptsBeforeAngry = 3; // Adjust if needed
    private float lastDrinkTime;
    private const float DrinkCooldown = 5.0f; // Time in seconds before counter resets
    public PetMood CurrentMood => currentMood;
    public ActionAnimations actionAnimator;
    public EmotionAnimations emotionAnimator;

    float currentHungerPercentage;
    float currentThirstPercentage;
    float currentCleanlinessPercentage;
    float currentFunPercentage;
    float currentEnergyPercentage;
    float currentHappinessPercentage;


    private void Start()
    {
        //Grab the current status percentage from the stats manager
        float currentHungerPercentage = statsManager.HungerPercent;
        float currentFunPercentage = statsManager.FunPercent;
        float currentThirstPercentage = statsManager.ThirstPercent;
        float currentCleanlinessPercentage = statsManager.CleanlinessPercent;
        float currentEnergyPercentage = statsManager.EnergyPercent;

        UpdateHappiness();
        Debug.Log($"Happiness is {currentHappinessPercentage}");

        // HAPPINESS
        if (currentHappinessPercentage <= 1f)
        {
            emotionAnimator.VeryHappyMood();
        }
        if (currentHappinessPercentage <= 0.8f)
        {
            emotionAnimator.HappyMood();
        }


        /// HUNGER
        if (currentHungerPercentage < 0.2f)
        {
            currentMood = PetMood.Hungry;
        }
        else
        {
            currentMood = PetMood.Normal;
        }

        // FUN
        if (currentFunPercentage < 0.2f)
        {
            currentMood = PetMood.Bored;
        }
        else
        {
            currentMood = PetMood.Normal;
        }
    }

    public void UpdateHappiness()
    {
        currentHappinessPercentage = statsManager.HappinessPercent;

        switch (currentHappinessPercentage)
        {
            case float n when (n <= 1f):
                emotionAnimator.EmptyMood();
                emotionAnimator.VeryHappyMood();
                break;
            case float n when (n <= 0.8f):
                emotionAnimator.EmptyMood();
                emotionAnimator.HappyMood();
                break;
            case float n when (n <= 0.6f):
                emotionAnimator.EmptyMood();
                emotionAnimator.ContentMood();
                break;
            case float n when (n <= 0.4f):
                emotionAnimator.EmptyMood();
                emotionAnimator.SadMood();
                break;
            case float n when (n <= 0.2f):
                emotionAnimator.EmptyMood();
                emotionAnimator.CryMood();
                break;
            case float n when (n <= 0f):
                emotionAnimator.EmptyMood();
                break;
        }
    }

    private void Update()
    {
        currentHungerPercentage = statsManager.HungerPercent;
        currentFunPercentage = statsManager.FunPercent;
        UpdateHappiness();


        // Handle mood due to BOREDOM
        if (currentFunPercentage < 0.2f)
        {
            if (currentMood != PetMood.Bored)
            {
                SetBoredMood();
            }
        }
        else if (currentMood == PetMood.Bored) { ResetMood(); }

        // Handle mood due to HUNGER
        if (currentHungerPercentage < 0.5f)
        {
            if (currentMood != PetMood.Hungry)
            {
                SetHungryMood();
            }
        }
        else if (currentMood == PetMood.Hungry) { ResetMood(); }

        //Handle mood due to SICKNESS
        if (statsManager.CurrentHealthStatus == StatsManager.HealthStatus.Sick)
        {
            if (currentMood != PetMood.Sick)
            {
                SetSickMood();
            }
        }
        else if (currentMood == PetMood.Sick) { ResetMood(); }



        // Handle mood due to anger
        if (currentMood == PetMood.Angry && Time.time - lastAngryTime > FeedCooldown)
        {
            ResetMood();
        }

        // ... You might have other mood checks here
    }


    // Call this method when the pet is fed
    public void RegisterFeed()
    {
        if (currentMood == PetMood.Angry)
        {
            return;
        }

        if (Time.time - lastFeedTime < FeedCooldown)
        {
            rapidFeedAttempts++;
            if (rapidFeedAttempts > MaxFeedAttemptsBeforeAngry)
            {
                SetAngryMood();
                lastAngryTime = Time.time; // Update the last time pet became angry
                consoleMessages.ShowOverFeedingMessage();
                rapidFeedAttempts = 0; // Reset rapidFeedAttempts to avoid immediate re-triggering after cooldown
            }
        }
        else
        {
            rapidFeedAttempts = 0;
            lastFeedTime = Time.time;
            ResetAngryMood(); // Update last feed time if enough time has passed
        }
    }

    // Call this method when the pet is given water
    public void RegisterDrink()
    {
        if (currentMood == PetMood.Angry) return;

        if (Time.time - lastDrinkTime < DrinkCooldown)
        {
            rapidDrinkAttempts++;
            if (rapidDrinkAttempts > MaxDrinkAttemptsBeforeAngry)
            {
                SetAngryMood();
                lastAngryTime = Time.time; // Update the last time pet became angry
                consoleMessages.ShowOverDrinkingMessage();
                rapidDrinkAttempts = 0; // Reset rapidDrinkAttempts to avoid immediate re-triggering after cooldown
            }
        }
        else
        {
            rapidDrinkAttempts = 0;
            lastDrinkTime = Time.time; // Update last drink time if enough time has passed
            ResetAngryMood();
        }
    }

    private void SetSadMood()
    {
        currentMood = PetMood.Sad;
        emotionAnimator.SadMood();
    }

    private void SetHappyMood()
    {
        currentMood = PetMood.Happy;
        emotionAnimator.HappyMood();
    }

    private void SetSickMood()
    {
        currentMood = PetMood.Sick;
        consoleMessages.ShowSickMessage();
        // add the button flashing here too
    }

    private void SetAngryMood()
    {
        currentMood = PetMood.Angry;
    }

    private void SetBoredMood()
    {
        currentMood = PetMood.Bored;
        consoleMessages.ShowBoredMessage();

    }
    private void SetHungryMood()
    {
        currentMood = PetMood.Hungry;
        consoleMessages.ShowHungryMessage();
    }

    private void ResetAngryMood()
    {
        if (currentMood == PetMood.Angry)
        {
            rapidFeedAttempts = 0;
            rapidDrinkAttempts = 0;
            consoleMessages.ShowNoLongerAngryMessage();
        }
    }

    private void ResetMood()
    {
        currentMood = PetMood.Normal;
        consoleMessages.ClearConsoleMessage();
        emotionAnimator.EmptyMood();
    }
}

