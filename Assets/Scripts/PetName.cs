using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PetName : MonoBehaviour
{
    public Button playButton;
    public TMP_InputField nameInputField;

    private void Start(){
        UpdatePlayButtonState();
        nameInputField.onValueChanged.AddListener(delegate { UpdatePlayButtonState(); });
    }
    public void SetName()
    {
        string savePetName = nameInputField.text;
        PlayerPrefs.SetString("PetName", savePetName);
        //PlayerPrefs.SetInt("HasStaredGame",1);
        PlayerPrefs.Save();
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void UpdatePlayButtonState()
    {
        if(!string.IsNullOrWhiteSpace(nameInputField.text) && nameInputField.text.Length >= 3)
        {
            playButton.interactable = true;
        }
        else
        {
            playButton.interactable = false;
        }
    }
}
