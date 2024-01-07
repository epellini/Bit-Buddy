using UnityEngine;

public class PetPettingHandler : MonoBehaviour
{
    // This script could handle the petting interactions

    // Initially disabled
    void Start()
    {
        this.enabled = false;
    }

    void OnMouseDown()
    {
        // Implement pet petting logic here
        Debug.Log("Pet is being petted!");
        // Increase happiness or other stats
    }

    public void EnablePetting()
    {
        this.enabled = true; // Enable this script to allow petting
    }
}
