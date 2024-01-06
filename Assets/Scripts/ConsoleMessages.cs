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
public void ShowAngryNoEatMessage() { UpdateConsoleMessage("Grrr! Not in the mood for snacks!"); ClearMessageAfterSeconds(4.0f); }
public void ShowSickNoEatMessage() { UpdateConsoleMessage("Ugh, my tummy's rumbling in a bad way... No food, please!"); ClearMessageAfterSeconds(4.0f); }
public void ShowFullNoEatMessage() { UpdateConsoleMessage("No more, please! I'm stuffed!"); ClearMessageAfterSeconds(4.0f); }
public void ShowOverFeedingMessage() { UpdateConsoleMessage("I'm feeling grumpy from too much food!"); ClearMessageAfterSeconds(4.0f); }
public void ShowHungryMessage() { UpdateConsoleMessage("My belly's rumbling... It's snack time!"); }

// Thirst related messages:
public void ShowAngryNoDrinkMessage() { UpdateConsoleMessage("Drink? Nope, not now."); ClearMessageAfterSeconds(4.0f); }
public void ShowSickNoDrinkMessage() { UpdateConsoleMessage("No drinks for me, feeling under the weather!"); ClearMessageAfterSeconds(4.0f); }
public void ShowFullNoDrinkMessage() { UpdateConsoleMessage("No thanks, I'm not thirsty right now!"); ClearMessageAfterSeconds(4.0f); }
public void ShowOverDrinkingMessage() { UpdateConsoleMessage("Too much water makes me a soggy pet!"); ClearMessageAfterSeconds(4.0f); }
public void ShowThirstyMessage() { UpdateConsoleMessage("Water, please! I'm parched!"); }

// Cleanliness related messages:
public void ShowAngryNoCleanMessage() { UpdateConsoleMessage("No baths, please! I'm not in the mood!"); ClearMessageAfterSeconds(4.0f); }
public void ShowFullNoCleanMessage() { UpdateConsoleMessage("I'm squeaky clean! No baths needed!"); ClearMessageAfterSeconds(4.0f); }
public void ShowDirtyMessage() { UpdateConsoleMessage("I'm feeling grubby, time for a bath!"); }

// Fun related messages:
public void ShowAngryNoPlayMessage() { UpdateConsoleMessage("Playtime? No thanks, I'm not feeling it!"); ClearMessageAfterSeconds(4.0f); }
public void ShowFullNoPlayMessage() { UpdateConsoleMessage("I'm all played out! Let's rest for now!"); ClearMessageAfterSeconds(4.0f); }
public void ShowBoredMessage() { UpdateConsoleMessage("So bored! Let's do something fun!"); }

// Health related messages:
public void ShowFullNoMedicineMessage() { UpdateConsoleMessage("Feeling top-notch! No meds needed!"); ClearMessageAfterSeconds(4.0f); }
public void ShowDirtyNoMedicineMessage() { UpdateConsoleMessage("Eek, I'm too icky for medicine!"); ClearMessageAfterSeconds(4.0f); }
public void ShowSickNoPlayMessage() { UpdateConsoleMessage("Not up for play, I'm feeling icky!"); ClearMessageAfterSeconds(4.0f); }
public void ShowSickMessage() { UpdateConsoleMessage("I don't feel so good..."); }

// Sleep related messages:
public void ShowFullNoSleepMessage() { UpdateConsoleMessage("Who needs sleep? I'm full of energy!"); ClearMessageAfterSeconds(4.0f); }
public void ShowSickNoSleepMessage() { UpdateConsoleMessage("Too sniffly for sleep right now."); ClearMessageAfterSeconds(4.0f); }
public void ShowTiredMessage() { UpdateConsoleMessage("Yawn! I'm ready for some dream time!"); }

public void ShowNoLongerTiredMessage() { UpdateConsoleMessage("All rested up! Ready for adventure!"); ClearMessageAfterSeconds(4.0f); }
public void ShowNoLongerDirtyMessage() { UpdateConsoleMessage("Look at me! I'm sparkling clean!"); ClearMessageAfterSeconds(4.0f); }
public void ShowNoLongerAngryMessage() { UpdateConsoleMessage("Phew! I'm back to my happy self!"); ClearMessageAfterSeconds(4.0f); }
public void ShowNoLongerSickMessage() { UpdateConsoleMessage("Hooray! I'm feeling tip-top again!"); ClearMessageAfterSeconds(4.0f); }

public void ShowAngryMessage() { UpdateConsoleMessage("You made me angry. Better watch out!"); }
public void ShowHappinessMessage() { UpdateConsoleMessage("Feeling blue... Could use some cheering up!"); }


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
