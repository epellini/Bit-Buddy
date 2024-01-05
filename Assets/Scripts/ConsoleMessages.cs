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

    /// <summary>
    /// Console Messages
    /// </summary>

    public void ShowNoLongerAngryMessage() { UpdateConsoleMessage("Pet has calmed down and is no longer angry."); }
    public void ShowOverFeedingMessage()
    {
        UpdateConsoleMessage("Pet has become angry due to overfeeding!");
        // ClearMessageAfterSeconds(3.0f); // This will clear the message after 3 seconds
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
