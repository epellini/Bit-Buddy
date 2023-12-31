using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAnimations : MonoBehaviour
{
    private Animator actionAnimator;

    private void Start()
    {
        actionAnimator = GetComponent<Animator>();
    }
    public void IdleAction() { actionAnimator.SetTrigger("IdleTrigger"); }
    public void EatAction() { actionAnimator.SetTrigger("EatTrigger"); }
    public void PlayAction() { actionAnimator.SetTrigger("PlayTrigger"); }
    public void PoopAction() { actionAnimator.SetTrigger("PoopTrigger"); }
    public void CleanAction() { actionAnimator.SetTrigger("CleanTrigger"); }
    public void HealAction() { actionAnimator.SetTrigger("MedicineTrigger"); }
    public void DeathAction() { actionAnimator.SetTrigger("DeathTrigger"); }
}