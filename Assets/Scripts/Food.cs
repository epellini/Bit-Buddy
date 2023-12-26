using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    public Food food;
    public Button button;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(food.FeedPlayer);
    }
    public void FeedPlayer()
    {
        
    }
}