using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EggClickHandler : MonoBehaviour
{
    public GameStateManager gameStateManager;
    public int requiredClicks = 6;
    private int currentClickCount = 0;
    public Button uiButton;
    public Image eggImage; // The UI Image component representing the egg

    // Array of Sprites to switch between each click
    public Sprite[] eggSprites;

    void Start()
    {
        uiButton.onClick.AddListener(OnButtonClick);
        if(eggSprites.Length > 0)
        {
            // Start with the first sprite
            eggImage.sprite = eggSprites[0];
        }
    }

    void OnButtonClick()
    {
        currentClickCount++;
        StartCoroutine(ChangeSize());

        // Switch to the next sprite, if available
        if (currentClickCount < eggSprites.Length)
        {
            eggImage.sprite = eggSprites[currentClickCount];
        }

        if (currentClickCount >= requiredClicks)
        {
            TransitionToNextStage();
        }
    }

    IEnumerator ChangeSize()
    {
        // Temporarily increase the button's scale
        uiButton.transform.localScale *= 1.05f; // Adjust the multiplier as needed for desired effect

        // Wait for a short duration
        yield return new WaitForSeconds(0.2f); // Adjust time as needed

        // Revert to the original scale
        uiButton.transform.localScale = Vector3.one;
    }

    void TransitionToNextStage()
    {
        Debug.Log("Transition to next stage!");
        currentClickCount = 0; // Reset click count for next time
        gameStateManager.StartGame(); // Placeholder for whatever transition you need
    }
}
