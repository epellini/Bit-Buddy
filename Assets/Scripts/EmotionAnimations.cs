using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionAnimations : MonoBehaviour
{
    private Animator emotionAnimator;
    private void Start()
    {
        emotionAnimator = GetComponent<Animator>();
    }
    
    // Pet Moods
    public void AngryMood() { emotionAnimator.SetTrigger("AngryTrigger"); }
    public void NormalMood() { emotionAnimator.SetTrigger("NormalTrigger"); }
    public void SadMood() { emotionAnimator.SetTrigger("SadTrigger"); }
    public void HappyMood() { emotionAnimator.SetTrigger("HappyTrigger"); }
    public void SickMood() { emotionAnimator.SetTrigger("SickTrigger"); }
    public void ContentMood() { emotionAnimator.SetTrigger("ContentTrigger"); }
    public void CoolMood() { emotionAnimator.SetTrigger("CoolTrigger"); }
    public void CryMood() { emotionAnimator.SetTrigger("CryTrigger"); }
    public void VeryHappyMood() { emotionAnimator.SetTrigger("VeryHappyTrigger"); }
    public void EmptyMood() { emotionAnimator.SetTrigger("EmptyTrigger"); }
}
