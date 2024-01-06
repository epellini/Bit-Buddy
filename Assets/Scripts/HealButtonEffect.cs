using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealButtonEffect : MonoBehaviour
{
    private Animator healthButtonAnimator;
    private void Start()
    {
        healthButtonAnimator = GetComponent<Animator>();
    }

    public void ButtonSick() { healthButtonAnimator.SetTrigger("SickButtonTrigger"); }
    public void ButtonHealthy() { healthButtonAnimator.SetTrigger("HealthyButtonTrigger"); }
}
