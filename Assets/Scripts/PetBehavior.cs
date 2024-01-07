using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class PetBehavior : MonoBehaviour
{
    public ConsoleMessages consoleMessages;
    public StatsManager statsManager;
    public enum PetMood { Happy, Content, Normal, Angry, Sad, Bored, Hungry, Sick, Thirsty, Dirty, Tired }
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
    float currentHappinessPercentage;
    float currentThirstPercentage;
    float currentFunPercentage;
    float currentCleanlinessPercentage;
    float currentEnergyPercentage;

    private void GetHappiness()
    {
        currentHappinessPercentage = statsManager.HappinessPercent;
    }

    private void Start()
    {
        GetHappiness();
        //Grab the current status percentage from the stats manager
        currentHungerPercentage = statsManager.HungerPercent;
        currentFunPercentage = statsManager.FunPercent;
        currentThirstPercentage = statsManager.ThirstPercent;
        currentCleanlinessPercentage = statsManager.CleanlinessPercent;
        currentEnergyPercentage = statsManager.EnergyPercent;
        UpdateHappiness();

        //Energy
        if (currentEnergyPercentage < 0.3f)
        {
            currentMood = PetMood.Tired;
        }
        else
        {
            currentMood = PetMood.Normal;
        }

        //Cleanliness
        if (currentCleanlinessPercentage < 0.2f)
        {
            currentMood = PetMood.Dirty;
        }
        else
        {
            currentMood = PetMood.Normal;
        }

        /// HUNGER
        if (currentHungerPercentage < 0.5f)
        {
            currentMood = PetMood.Hungry;
        }
        else
        {
            currentMood = PetMood.Normal;
        }

        // FUN
        if (currentFunPercentage < 0.5f)
        {
            currentMood = PetMood.Bored;
        }
        else
        {
            currentMood = PetMood.Normal;
        }

        // THIRST
        if (currentThirstPercentage < 0.4f)
        {
            currentMood = PetMood.Thirsty;
        }
        else
        {
            currentMood = PetMood.Normal;
        }
    }

    private float lastHappinessPercentage = -1; // Initialize to a value that won't match any state
    private bool isWaiting = false; // Flag to check if currently waiting

    public void UpdateHappiness()
    {
        if (currentHappinessPercentage != lastHappinessPercentage && !isWaiting)
        {
            StartCoroutine(ChangeMoodWithDelay());
        }
    }

    private IEnumerator ChangeMoodWithDelay()
    {
        isWaiting = true; // Set the flag to true as we are starting the wait

        if (currentHappinessPercentage <= 0.2f)
        {
            emotionAnimator.CryMood();
        }
        else if (currentHappinessPercentage <= 0.4f)
        {
            emotionAnimator.SadMood();
        }
        else if (currentHappinessPercentage <= 0.6f)
        {
            emotionAnimator.ContentMood();
        }
        else if (currentHappinessPercentage <= 0.8f)
        {
            emotionAnimator.HappyMood();
        }
        else // currentHappinessPercentage > 0.8f
        {
            emotionAnimator.VeryHappyMood();
        }
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        lastHappinessPercentage = currentHappinessPercentage; // Update the last happiness percentage
        isWaiting = false; // Reset the flag as waiting is over
    }

    private void Update()
    {
        currentHungerPercentage = statsManager.HungerPercent;
        currentFunPercentage = statsManager.FunPercent;
        currentThirstPercentage = statsManager.ThirstPercent;
        currentCleanlinessPercentage = statsManager.CleanlinessPercent;
        currentEnergyPercentage = statsManager.EnergyPercent;

        GetHappiness();
        UpdateHappiness();

        //handle mood due to ENERGY
        if (currentEnergyPercentage < 0.3f)
        {
            if (currentMood != PetMood.Tired)
            {
                SetTiredMood();
            }
        }
        else if (currentMood == PetMood.Tired) { ResetTiredMood(); }

        //Handle mood due to CLEANLINESS
        if (currentCleanlinessPercentage < 0.2f)
        {
            if (currentMood != PetMood.Dirty)
            {
                SetDirtyMood();
            }
        }
        else if (currentMood == PetMood.Dirty) { ResetDirtyMood(); }

        // Handle mood due to THIRST
        if (currentThirstPercentage < 0.4f)
        {
            if (currentMood != PetMood.Thirsty)
            {
                SetThirstyMood();
            }
        }
        else if (currentMood == PetMood.Thirsty) { ResetMood(); }

        // Handle mood due to BOREDOM
        if (currentFunPercentage < 0.6f)
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
        else if (currentMood == PetMood.Sick) { ResetSick(); }

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

    private void SetTiredMood()
    {
        currentMood = PetMood.Tired;
        consoleMessages.ShowTiredMessage();
    }
    private void SetSickMood()
    {
        currentMood = PetMood.Sick;
        consoleMessages.ShowSickMessage();
        // add the button flashing here too
    }
    public void ResetSick()
    {
        consoleMessages.ClearConsoleMessage();
    }

    private void SetAngryMood()
    {
        currentMood = PetMood.Angry;
    }

    private void SetDirtyMood()
    {
        currentMood = PetMood.Dirty;
        consoleMessages.ShowDirtyMessage();
    }

    private void ResetDirtyMood()
    {
        if (currentMood == PetMood.Dirty)
        {
            consoleMessages.ShowNoLongerDirtyMessage();
        }
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

    private void SetThirstyMood()
    {
        currentMood = PetMood.Thirsty;
        consoleMessages.ShowThirstyMessage();
    }

    private void ResetTiredMood()
    {
        if (currentMood == PetMood.Tired)
        {
            consoleMessages.ShowNoLongerTiredMessage();
        }
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
    }

    public void ResetDeath()
    {
        currentMood = PetMood.Normal;
        consoleMessages.ClearConsoleMessage();
    }
}

