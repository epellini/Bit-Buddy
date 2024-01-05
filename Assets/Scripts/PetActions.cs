using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetActions : MonoBehaviour
{
    public StatsManager statsManager;
    public PetBehavior petBehavior;
    public ConsoleMessages consoleMessages;

    public void Feed()
    {
        // Check the pet's mood from PetBehavior
        if (petBehavior.CurrentMood == PetBehavior.PetMood.Angry)
        {
            // Show visually that the pet does not want to eat bc it's angry
            Debug.Log($"I'm angry and I don't want to eat!");
            petBehavior.RegisterFeed();
            return;
        }
        if (statsManager.CurrentHealthStatus == StatsManager.HealthStatus.Sick && statsManager.HungerPercent >= 0.2f)
        {
            // Show visually that the pet does not want to eat bc it's sick
            Debug.Log($"I'm sick and I don't want to eat!");
            petBehavior.RegisterFeed();
            return;
        }
        if (statsManager.HungerPercent > 0.9f)
        {
            // Show visually that the pet does not want to eat bc it's full
            Debug.Log($"I'm full and I don't want to eat!");
            petBehavior.RegisterFeed();
            return;
        }
        float hungerAmountToIncrease = 33f;
        statsManager.IncreaseHunger(hungerAmountToIncrease);
    }

    public void GiveWater()
    {
        // Check the pet's mood from PetBehavior
        if (petBehavior.CurrentMood == PetBehavior.PetMood.Angry)
        {
            // Show visually that the pet does not want to drink bc it's angry
            Debug.Log($"I'm angry and I don't want to DRINK!");
            petBehavior.RegisterDrink();
            return;
        }
        if (statsManager.ThirstPercent > 0.92f)
        {
            // Show visually that the pet does not want to drink bc it's full
            Debug.Log($"I'm full and I don't want to drink!");
            petBehavior.RegisterDrink();
            return;
        }
        float thirstAmountToIncrease = 25f;
        statsManager.IncreaseThirst(thirstAmountToIncrease);
    }


    public void Sleep()
    {
        if (statsManager.CurrentHealthStatus == StatsManager.HealthStatus.Sick)
        {
            // Show visually that the pet does not want to sleep bc it's sick
            Debug.Log("I'm sick and I don't want to sleep");
            return;
        }
        if (statsManager.EnergyPercent > 0.7f)
        {
            // Show visually that the pet does not want to sleep bc it's full
            Debug.Log("I'm full of energy and I don't want to sleep!");
            return;
        }

        float sleepAmountToIncrease = 100f;
        statsManager.IncreaseEnergy(sleepAmountToIncrease);
    }

    public void Clean()
    {
         if (statsManager.CleanlinessPercent > 0.80f)
        {
            // Show visually that the pet does not want to bathe bc he already clean
            Debug.Log("I'm full of fun and I don't want to BATHE!");
            return;
        }

        float hungerAmountToDecrease = 10f;
        float thirstAmountToDecrease = 10f;
        float cleanlinessAmountToIncrease = 100f;
        statsManager.DecreaseHunger(hungerAmountToDecrease);
        statsManager.DecreaseThirst(thirstAmountToDecrease);
        statsManager.IncreaseCleanliness(cleanlinessAmountToIncrease);
    }

    public void Play()
    {
        if (statsManager.CurrentHealthStatus == StatsManager.HealthStatus.Sick)
        {
            // Show visually that the pet does not want to play because it's sick
            Debug.Log("I'm sick and I don't want to play");
            return;
        }
        if (statsManager.FunPercent > 0.97f)
        {
            // Show visually that the pet does not want to play because it's already having too much fun
            Debug.Log("I'm full of fun and I don't want to play!");
            return;
        }

        // If the pet is neither too sick nor too full of fun, play affects various stats
        float hungerAmountToDecrease = 3f;
        float thirstAmountToDecrease = 5f;
        float funAmountToIncrease = 20f;
        float cleanlinessAmountToDecrease = 2f;
        float energyAmountToDecrease = 5f;
        statsManager.DecreaseEnergy(energyAmountToDecrease);
        statsManager.DecreaseHunger(hungerAmountToDecrease);
        statsManager.DecreaseThirst(thirstAmountToDecrease);
        statsManager.IncreaseFun(funAmountToIncrease);
        statsManager.DecreaseCleanliness(cleanlinessAmountToDecrease);
    }

    public void Heal()
    {
        if (statsManager.CurrentHealthStatus == StatsManager.HealthStatus.Healthy)
        {
            // Show visually that the pet is healthy and does not want to heal
            Debug.Log("I'm healthy and I don't want to heal");
            return;
        }

        // Check if the pet is sick
        if (statsManager.CurrentHealthStatus == StatsManager.HealthStatus.Sick)
        {
            // If the pet is sick, check cleanliness
            if (statsManager.CleanlinessPercent < 0.15f)
            {
                // Show visually that the pet is dirty and wants to be cleaned before healed
                Debug.Log("I'm dirty, I want a bath before healing");
                return;
            }

            // If the pet is sick and not too dirty, heal the pet
            Debug.Log("Just got healed!");
            float funAmountToDecrease = 40f;
            statsManager.DecreaseFun(funAmountToDecrease);
            statsManager.HealPet();
        }
    }
}
