using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetAnimations : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    //Define sprites for different moods and states
    public Sprite angrySprite;
    public Sprite happySprite;
    public Sprite normalSprite;
    public Sprite sadSprite;
    public Sprite boredSprite;
    public Sprite sleepingSprite;

    // ... other sprites
    private void Start()
    {
        // Get the SpriteRenderer component from the same GameObject this script is attached to
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void AngryMood() { animator.SetTrigger("AngryTrigger"); }
    public void HappyMood() { animator.SetTrigger("HappyTrigger"); }
    public void NormalMood() { animator.SetTrigger("NormalTrigger"); }
    public void SadMood() { animator.SetTrigger("SadTrigger"); }

}
