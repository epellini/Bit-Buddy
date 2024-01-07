using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public GameObject popUpPanel; // Assign this in the inspector

    public void TogglePopUp()
    {
        // If the pop-up is active, hide it. Otherwise, show it.
        popUpPanel.SetActive(!popUpPanel.activeSelf);
    }
}
