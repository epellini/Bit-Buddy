using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialLoadManager : MonoBehaviour
{
void Start()
{
    if (PlayerPrefs.HasKey("PetName"))
    {
        string petName = PlayerPrefs.GetString("PetName");
        Debug.Log("PetName exists with value: " + petName);
        if (!string.IsNullOrWhiteSpace(petName))
        {
            Debug.Log("Loading Game Scene");
            SceneManager.LoadScene("Game");  // make sure this is the exact name of your game scene
        }
    }
    else
    {
        Debug.Log("No PetName in PlayerPrefs, staying in the current scene.");
    }
}
}