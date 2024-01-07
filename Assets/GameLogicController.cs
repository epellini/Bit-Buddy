using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicController : MonoBehaviour
{
    public GameObject petPlayerObject;
    private const string HasStartedKey = "HasStartedGame";
    private void Start()
    {
        // Check if the game has been started before
        if (PlayerPrefs.GetInt(HasStartedKey, 0) == 0)
        {
            // The game has not been started yet, deactivate the PetPlayer
            if (petPlayerObject != null)
                petPlayerObject.SetActive(false);
        }
        // If the game has been started before (HasStartedKey == 1), do nothing.
        // The pet player object's active state will be managed by game state transitions.
    }

    public void StartGame()
    {
        Debug.Log("Game Started - PetPlayerObject activated");
        // Enable the PetPlayer to start all game logic when the game starts
        if (petPlayerObject != null)
            petPlayerObject.SetActive(true);

        // Additional logic to start the game goes here
    }

    public void StopGame()
    {
        //Debug.Log("Game Stopped - PetPlayerObject deactivated");
    if (petPlayerObject != null)
            petPlayerObject.SetActive(false);

        // Additional logic to stop or pause the game goes here
    }

    // Include other necessary methods for game control, like PauseGame() or ResetGame()
}
