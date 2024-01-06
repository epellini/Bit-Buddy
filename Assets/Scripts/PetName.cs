using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PetName : MonoBehaviour
{
    public string petName;
    public string savePetName;
    public TMP_InputField nameInputField; // Assign this in the inspector
    public TextMeshProUGUI petNameText;

    void Update(){
        petName = PlayerPrefs.GetString("PetName", "DefaultName");
        petNameText.text = petName;
    }
    public void SetName()
    {
        savePetName = nameInputField.text;
        PlayerPrefs.SetString("PetName", savePetName);
        // Don't forget to save PlayerPrefs
        PlayerPrefs.Save();
    }


}
