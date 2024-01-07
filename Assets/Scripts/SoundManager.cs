using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource effectSource;  // Assign in the inspector
    public AudioClip buttonPressClip, noMoreClip, feedingClip, drinkingClip, playingClip, healingClip, eggCrackClip, sleepingClip, cleaningClip; // Assign in the inspector
    private bool isMuted = false;
    public Button soundToggleButton; // Assign in the inspector

    public Sprite muteSprite, unmuteSprite; // Assign in the inspector

    private const string MutePrefKey = "IsMuted";


    public void Start()
    {
        // Load the mute state from PlayerPrefs
        isMuted = PlayerPrefs.GetInt(MutePrefKey, 0) == 1;
        effectSource.mute = isMuted;
        UpdateMuteButtonVisuals();
    }

    void Awake()
    {
        // Singleton pattern for the sound manager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep the sound manager across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        effectSource.mute = isMuted;
        // Save the new mute state to PlayerPrefs
        PlayerPrefs.SetInt(MutePrefKey, isMuted ? 1 : 0);
        PlayerPrefs.Save();
        UpdateMuteButtonVisuals();
    }

    // Play the button press sound
    public void ButtonPress()
    {
        effectSource.clip = buttonPressClip;
        effectSource.Play();
    }

    public void NoMore()
    {
        effectSource.clip = noMoreClip;
        effectSource.Play();
    }

    // Play the feeding sound
    public void FeedingSound()
    {
        effectSource.clip = feedingClip;
        effectSource.Play();
    }

    public void SleepingSound()
    {
        effectSource.clip = sleepingClip;
        effectSource.Play();
    }
    public void CleaningSound()
    {
        effectSource.clip = cleaningClip;
        effectSource.Play();
    }

    public void DrinkingSound()
    {
        effectSource.clip = drinkingClip;
        effectSource.Play();
    }
    public void HealingSound()
    {
        effectSource.clip = healingClip;
        effectSource.Play();
    }

    public void EggCrack()
    {
        effectSource.clip = eggCrackClip;
        effectSource.Play();
    }

    public void PlayingSound()
    {
        effectSource.clip = playingClip;
        effectSource.Play();
    }

    private void UpdateMuteButtonVisuals()
    {
        if (soundToggleButton != null)
        {
            // Change the button sprite depending on the mute state
            soundToggleButton.image.sprite = isMuted ? muteSprite : unmuteSprite;
        }

    }

}
