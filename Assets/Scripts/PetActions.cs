using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetActions : MonoBehaviour
{

    public StatsManager statsManager;

    public void Feed()
    {
        float hungerAmountToIncrease = 20f;
        statsManager.IncreaseHunger(hungerAmountToIncrease);
    }

    public void GiveWater()
    {
        float thirstAmountToIncrease = 20f;
        statsManager.IncreaseThirst(thirstAmountToIncrease);
    }
}
