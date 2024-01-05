using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsoleMessages : MonoBehaviour
{
    [SerializeField] private TMP_Text consoleText;
    public void ClearMessageAfterSeconds(float seconds)
    {
        StartCoroutine(ClearAfterDelay(seconds));
    }
    private IEnumerator ClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearConsoleMessage(); // This will clear the message
    }
    // Method to update the console message
    public void UpdateConsoleMessage(string message)
    {
        if (consoleText != null)
        {
            consoleText.text = message;
        }
        else
        {
            Debug.Log("Console Text is null");
        }
    }

    // ClearMessageAfterSeconds(3.0f); // This will clear the message after 3 seconds

    // Hunger related messages:
    public void ShowAngryNoEatMessage() { UpdateConsoleMessage("I'm angry and I don't want to eat!"); }
    public void ShowSickNoEatMessage() { UpdateConsoleMessage("I'm sick and I don't want to eat!"); }
    public void ShowFullNoEatMessage() { UpdateConsoleMessage("I'm full and I don't want to eat!"); }
    public void ShowOverFeedingMessage() { UpdateConsoleMessage("Pet has become angry due to overfeeding!"); }

    // Thirst related messages:
    public void ShowAngryNoDrinkMessage() { UpdateConsoleMessage("I'm angry and I don't want to drink!"); }
    public void ShowSickNoDrinkMessage() { UpdateConsoleMessage("I'm sick and I don't want to drink!"); }
    public void ShowFullNoDrinkMessage() { UpdateConsoleMessage("I'm not thirsty and I don't want to drink!"); }
    public void ShowOverDrinkingMessage() { UpdateConsoleMessage("Pet has become angry due to overdrinking!"); }

    // Cleanliness related messages:
    public void ShowAngryNoCleanMessage() { UpdateConsoleMessage("I'm angry and I don't want to be cleaned!"); }
    public void ShowFullNoCleanMessage() { UpdateConsoleMessage("I'm clean and I don't want to be take a bath!"); }

    // Fun related messages:
    public void ShowAngryNoPlayMessage() { UpdateConsoleMessage("I'm angry and I don't want to play!"); }
    public void ShowFullNoPlayMessage() { UpdateConsoleMessage("I'm already having fun and I don't want to play anymore!"); }

    // Health related messages:
    public void ShowFullNoMedicineMessage() { UpdateConsoleMessage("I'm healthy and I don't want to take medicine!"); ClearMessageAfterSeconds(4.0f); }
    public void ShowDirtyNoMedicineMessage() { UpdateConsoleMessage("I'm dirty and I don't want to take medicine!"); ClearMessageAfterSeconds(4.0f); }
    public void ShowSickNoPlayMessage() { UpdateConsoleMessage("I'm sick and I don't want to play!"); ClearMessageAfterSeconds(4.0f); }

    // Sleep related messages:
    public void ShowFullNoSleepMessage() { UpdateConsoleMessage("I'm not tired and I don't want to sleep!"); }
    public void ShowSickNoSleepMessage() { UpdateConsoleMessage("I'm sick and I don't want to sleep!"); }
    

    public void ShowNoLongerAngryMessage()
    {
        UpdateConsoleMessage("Pet has calmed down and is no longer angry.");
        ClearMessageAfterSeconds(5.0f);
    }

    public void ShowNoLongerSickMessage()
    {
        UpdateConsoleMessage("Pet has recovered and is no longer sick.");
        ClearMessageAfterSeconds(5.0f);
    }

    public void ShowAngryMessage()
    {
        UpdateConsoleMessage("I'm angry!");
    }

    public void ShowHungerMessage()
    {
        UpdateConsoleMessage("I'm hungry!");
    }

    public void ShowThirstMessage()
    {
        UpdateConsoleMessage("I'm thirsty!");
    }

    public void ShowCleanlinessMessage()
    {
        UpdateConsoleMessage("I'm dirty!");
    }

    public void ShowFunMessage()
    {
        UpdateConsoleMessage("I'm bored!");
    }

    public void ShowHappinessMessage()
    {
        UpdateConsoleMessage("I'm sad!");
    }

    public void ShowEnergyMessage()
    {
        UpdateConsoleMessage("I'm tired!");
    }

    public void ShowHealthMessage()
    {
        UpdateConsoleMessage("I'm sick!");
    }

    public void ClearConsoleMessage()
    {
        if (consoleText != null)
        {
            consoleText.text = "";
        }
        else
        {
            Debug.Log("Console Text is null");
        }
    }

}
