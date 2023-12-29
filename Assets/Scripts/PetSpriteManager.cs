using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSpriteManager : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    //Define sprites for different moods and states
    public Sprite happySprite;
    public Sprite angrySprite;
    public Sprite normalSprite;
    public Sprite sadSprite;
    // ... other sprites

    // Start is called before the first frame update
    void Start()
    {
         // Get the SpriteRenderer component from the same GameObject this script is attached to
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
    }

    // Call this method to change the pet's sprite based on its mood
    public void UpdatePetSprite(PetBehavior.PetMood mood)
    {
        switch(mood)
        {
            case PetBehavior.PetMood.Happy:
                spriteRenderer.sprite = happySprite;
                // Or if using Animator
                // animator.SetTrigger("HappyTrigger");
                break;
            case PetBehavior.PetMood.Angry:
                spriteRenderer.sprite = angrySprite;
                // animator.SetTrigger("AngryTrigger");
                break;
            case PetBehavior.PetMood.Normal:
                spriteRenderer.sprite = normalSprite;
                // animator.SetTrigger("NormalTrigger");
                break;
            // ... handle other moods
        }
    }

    // You might also have methods for playing specific animations for actions like eating or drinking
    public void PlayEatingAnimation()
    {
        // Code to play eating animation
        // animator.SetTrigger("EatTrigger");
    }

    public void PlayDrinkingAnimation()
    {
        // Code to play drinking animation
        // animator.SetTrigger("DrinkTrigger");
    }

}
