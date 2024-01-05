using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBehavior : MonoBehaviour
{
    public ConsoleMessages consoleMessages;
    public enum PetMood { Happy, Normal, Angry, Sad, Bored }
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
                currentMood = PetMood.Angry;
                lastAngryTime = Time.time; // Update the last time pet became angry
                consoleMessages.ShowOverFeedingMessage();
                Debug.Log("Pet has become angry due to overfeeding!");
                emotionAnimator.AngryMood();
                rapidFeedAttempts = 0; // Reset rapidFeedAttempts to avoid immediate re-triggering after cooldown
            }
        }
        else
        {
            rapidFeedAttempts = 0;
            lastFeedTime = Time.time; // Update last feed time if enough time has passed
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
                currentMood = PetMood.Angry;
                lastAngryTime = Time.time; // Update the last time pet became angry
                Debug.Log("Pet has become angry due to overdrinking!");
                consoleMessages.ShowOverDrinkingMessage();
                emotionAnimator.AngryMood();
                rapidDrinkAttempts = 0; // Reset rapidDrinkAttempts to avoid immediate re-triggering after cooldown
            }
        }
        else
        {
            rapidDrinkAttempts = 0;
            lastDrinkTime = Time.time; // Update last drink time if enough time has passed
        }
    }

    private void Update()
    {
        // If the pet has been angry and enough time has passed, reset the mood
        if (currentMood == PetMood.Angry && Time.time - lastAngryTime > FeedCooldown)
        {
            currentMood = PetMood.Normal;
            rapidFeedAttempts = 0;
            rapidDrinkAttempts = 0;
            consoleMessages.ShowNoLongerAngryMessage();
            Debug.Log("Pet has calmed down and is no longer angry.");
            // Trigger the Normal status animation
            emotionAnimator.EmptyMood();
        }
    }
}
