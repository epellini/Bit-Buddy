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
    public void ShowHungryMessage()
    {
        UpdateConsoleMessage("I'm hungry!");
    }

    // Thirst related messages:
    public void ShowAngryNoDrinkMessage() { UpdateConsoleMessage("I'm angry and I don't want to drink!"); ClearMessageAfterSeconds(4.0f); }
    public void ShowSickNoDrinkMessage() { UpdateConsoleMessage("I'm sick and I don't want to drink!"); ClearMessageAfterSeconds(4.0f); }
    public void ShowFullNoDrinkMessage() { UpdateConsoleMessage("I'm not thirsty and I don't want to drink!"); ClearMessageAfterSeconds(4.0f); }
    public void ShowOverDrinkingMessage() { UpdateConsoleMessage("Pet has become angry due to overdrinking!"); ClearMessageAfterSeconds(4.0f); }

        public void ShowThirstyMessage()
    {
        UpdateConsoleMessage("I'm thirsty!");
    }

    // Cleanliness related messages:
    public void ShowAngryNoCleanMessage() { UpdateConsoleMessage("I'm angry and I don't want to be cleaned!"); }
    public void ShowFullNoCleanMessage() { UpdateConsoleMessage("I'm clean and I don't want to be take a bath!"); }
    public void ShowDirtyMessage()
    {
        UpdateConsoleMessage("I'm dirty!");
    }

    // Fun related messages:
    public void ShowAngryNoPlayMessage() { UpdateConsoleMessage("I'm angry and I don't want to play!"); }
    public void ShowFullNoPlayMessage() { UpdateConsoleMessage("I'm already having fun and I don't want to play anymore!"); }
        public void ShowBoredMessage()
    {
        UpdateConsoleMessage("I'm bored!");
    }

    // Health related messages:
    public void ShowFullNoMedicineMessage() { UpdateConsoleMessage("I'm healthy and I don't want to take medicine!"); ClearMessageAfterSeconds(4.0f); }
    public void ShowDirtyNoMedicineMessage() { UpdateConsoleMessage("I'm dirty and I don't want to take medicine!"); ClearMessageAfterSeconds(4.0f); }
    public void ShowSickNoPlayMessage() { UpdateConsoleMessage("I'm sick and I don't want to play!"); ClearMessageAfterSeconds(4.0f); }
    public void ShowSickMessage()
    {
        UpdateConsoleMessage("I'm sick!");
    }

    // Sleep related messages:
    public void ShowFullNoSleepMessage() { UpdateConsoleMessage("I'm not tired and I don't want to sleep!"); ClearMessageAfterSeconds(4.0f); }
    public void ShowSickNoSleepMessage() { UpdateConsoleMessage("I'm sick and I don't want to sleep!"); ClearMessageAfterSeconds(4.0f); }
        public void ShowTiredMessage()
    {
        UpdateConsoleMessage("I'm tired!");
    }

    public void ShowNoLongerTiredMessage()
    {
        UpdateConsoleMessage("Pet has recovered and is no longer tired.");
        ClearMessageAfterSeconds(4.0f);
    }

    public void ShowNoLongerDirtyMessage()
    {
        UpdateConsoleMessage("Pet has recovered and is no longer dirty.");
        ClearMessageAfterSeconds(4.0f);
    }

    public void ShowNoLongerAngryMessage()
    {
        UpdateConsoleMessage("Pet has calmed down and is no longer angry.");
        ClearMessageAfterSeconds(4.0f);
    }

    public void ShowNoLongerSickMessage()
    {
        UpdateConsoleMessage("Pet has recovered and is no longer sick.");
        ClearMessageAfterSeconds(4.0f);
    }

    public void ShowAngryMessage()
    {
        UpdateConsoleMessage("I'm angry!");
    }

    public void ShowHappinessMessage()
    {
        UpdateConsoleMessage("I'm sad!");
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
