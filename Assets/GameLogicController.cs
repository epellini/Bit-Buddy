using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicController : MonoBehaviour
{
    public GameObject petPlayerObject;
    private void Start()
    {
        // Initially disable the PetPlayer when the scene starts
        if(petPlayerObject != null)
            petPlayerObject.SetActive(false);
    }
    // Assuming you have methods like these to control the game state

    public void StartGame()
    {
        // Enable the PetPlayer to start all game logic when the game starts
        if(petPlayerObject != null)
            petPlayerObject.SetActive(true);

        // Additional logic to start the game goes here
    }

    public void StopGame()
    {
        // Disable the PetPlayer to stop all game logic when the game stops or pauses
        if(petPlayerObject != null)
            petPlayerObject.SetActive(false);

        // Additional logic to stop or pause the game goes here
    }

    // Include other necessary methods for game control, like PauseGame() or ResetGame()
}
