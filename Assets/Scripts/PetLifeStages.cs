// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;

// public class PetLifeStages : MonoBehaviour
// {
//     private Animator stageAnimator;
//     // References to the Animator Controllers for each life stage
//     public RuntimeAnimatorController eggController;
//     public RuntimeAnimatorController babyController;
//     public RuntimeAnimatorController adultController;
//     public RuntimeAnimatorController seniorController;
//     public RuntimeAnimatorController deathController;

//     private DateTime _lastUpdateTime;
//     private DateTime _petBirthTime;
//     private TimeSpan _currentPetAge;
//     [SerializeField] private StatsManager _statsManager;

//     public enum LifeStage { Egg, Baby, Adult, Senior, Death }
//     public LifeStage CurrentLifeStage { get; private set; } = LifeStage.Egg;


//     // Time requirements for each stage
//     private readonly TimeSpan eggTimeRequirement = TimeSpan.FromSeconds(10);
//     private readonly TimeSpan babyTimeRequirement = TimeSpan.FromMinutes(20);
//     private readonly TimeSpan adultTimeRequirement = TimeSpan.FromMinutes(30);
//     private readonly TimeSpan seniorTimeRequirement = TimeSpan.FromMinutes(40);

//     private void Start()
//     {

//         // Get the Animator component
//         stageAnimator = GetComponent<Animator>();

//         // Load or initialize pet birth time
//         string birthTimeStr = PlayerPrefs.GetString("PetBirthTime", "");
//         _petBirthTime = !string.IsNullOrEmpty(birthTimeStr) ? DateTime.FromBinary(Convert.ToInt64(birthTimeStr)) : DateTime.Now;

//         // Load or initialize last update time and calculate elapsed time
//         string lastPlayTimeStr = PlayerPrefs.GetString("LastPlayTime", "");
//         if (!string.IsNullOrEmpty(lastPlayTimeStr))
//         {
//             _lastUpdateTime = DateTime.FromBinary(Convert.ToInt64(lastPlayTimeStr));
//         }
//         else
//         {
//             _lastUpdateTime = DateTime.Now;
//         }

//         // Calculate elapsed time since the game was last played and update pet age
//         TimeSpan timeSinceLastPlay = DateTime.Now - _lastUpdateTime;
//         _currentPetAge += timeSinceLastPlay;

//         // Set current time as last update time and save
//         _lastUpdateTime = DateTime.Now;
//         PlayerPrefs.SetString("LastPlayTime", _lastUpdateTime.ToBinary().ToString());
//         PlayerPrefs.Save();

//         // Now, load the current life stage from PlayerPrefs
//         CurrentLifeStage = (LifeStage)PlayerPrefs.GetInt("CurrentLifeStage", (int)LifeStage.Egg);
//         Debug.Log("Current Life Stage: " + CurrentLifeStage.ToString());
//         UpdateLifeStage();
//         ApplyLifeStageToAnimator(CurrentLifeStage);
//     }

//     private void Update()
//     {
//         UpdateLifeStage();
//     }

//     private void OnApplicationQuit()
//     {
//         // Save the current pet age
//         PlayerPrefs.SetString("PetBirthTime", _petBirthTime.ToBinary().ToString());
//         PlayerPrefs.Save();

//         // Save the current life stage
//         PlayerPrefs.SetInt("CurrentLifeStage", (int)CurrentLifeStage);
//         PlayerPrefs.Save();

//     }

//     public void UpdateLifeStage()
//     {
//         TimeSpan age = _statsManager.CurrentPetAge;
//         LifeStage previousStage = CurrentLifeStage;

//         if (CurrentLifeStage == LifeStage.Egg && age >= eggTimeRequirement)
//         {
//             CurrentLifeStage = LifeStage.Baby;
//             stageAnimator.runtimeAnimatorController = babyController;
//             Debug.Log("Transitioned to Baby Stage");
//         }
//         else if (CurrentLifeStage == LifeStage.Baby && age >= babyTimeRequirement)
//         {
//             CurrentLifeStage = LifeStage.Adult;
//             stageAnimator.runtimeAnimatorController = adultController;
//             Debug.Log("Transitioned to Adult Stage");
//         }
//         else if (CurrentLifeStage == LifeStage.Adult && age >= adultTimeRequirement)
//         {
//             CurrentLifeStage = LifeStage.Senior;
//             stageAnimator.runtimeAnimatorController = seniorController;
//             Debug.Log("Transitioned to Senior Stage");
//         }
//         else if (CurrentLifeStage == LifeStage.Senior && age >= seniorTimeRequirement)
//         {
//             CurrentLifeStage = LifeStage.Death;
//             stageAnimator.runtimeAnimatorController = deathController;
//             Debug.Log("Transitioned to Death Stage");
//         }

//         // If the life stage has changed, log the current stage.
//         if (previousStage != CurrentLifeStage)
//         {
//             Debug.Log("Current Life Stage: " + CurrentLifeStage.ToString());
//         }
//     }


//     private void ApplyLifeStageToAnimator(LifeStage stage)
//     {
//         switch (stage)
//         {
//             case LifeStage.Egg:
//                 stageAnimator.runtimeAnimatorController = eggController;
//                 break;
//             case LifeStage.Baby:
//                 stageAnimator.runtimeAnimatorController = babyController;
//                 break;
//             case LifeStage.Adult:
//                 stageAnimator.runtimeAnimatorController = adultController;
//                 break;
//             case LifeStage.Senior:
//                 stageAnimator.runtimeAnimatorController = seniorController;
//                 break;
//             case LifeStage.Death:
//                 stageAnimator.runtimeAnimatorController = deathController;
//                 break;
//             default:
//                 break;
//         }
//     }
// }
