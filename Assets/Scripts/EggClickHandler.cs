using UnityEngine;

public class EggClickHandler : MonoBehaviour
{
    public int requiredClicks = 5; // Number of clicks needed to transition
    private int currentClickCount = 0; // Current number of clicks
    public Vector3 clickScaleChange = new Vector3(0.1f, 0.1f, 0.1f); // Temporary scale change on click
    private Vector3 originalScale; // To store the original scale

    void Start()
    {
        // Store the original scale of the egg
        originalScale = transform.localScale;
    }

    void OnMouseDown()
    {
        // Increase the click count
        currentClickCount++;

        // Temporarily scale up the egg for visual feedback
        transform.localScale = originalScale + clickScaleChange;

        // Check if the required number of clicks has been reached
        if(currentClickCount >= requiredClicks)
        {
            // Transition to the next stage
            TransitionToNextStage();
        }
    }

    void OnMouseUp()
    {
        // When the click is released, scale back down to original size
        transform.localScale = originalScale;
    }

    void TransitionToNextStage()
    {
        Debug.Log("Egg has transitioned to the next stage!");
        // Implement the logic to transition the egg to the next stage
    }
}
