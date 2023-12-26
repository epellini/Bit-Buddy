using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private StatsManager _statsManager;
    [SerializeField] private Image _hungerMeter, _thirstMeter; // Declare images

    private void FixedUpdate()
    {
        _hungerMeter.fillAmount = _statsManager.HungerPercent; // Set the fill amount of the image to the hunger percent
        _thirstMeter.fillAmount = _statsManager.ThirstPercent; // Set the fill amount of the image to the thirst percent
    }

    // [SerializeField] private GameObject _gameOverPanel;
    // [SerializeField] private GameObject _pausePanel;
    // [SerializeField] private GameObject _gamePanel;
    // [SerializeField] private GameObject _startPanel;
    // [SerializeField] private GameObject _winPanel;

    // private void OnEnable()
    // {
    //     StatsManager.OnPlayerDeath += GameOver;
    //     StatsManager.OnPlayerWin += Win;
    // }

    // private void OnDisable()
    // {
    //     StatsManager.OnPlayerDeath -= GameOver;
    //     StatsManager.OnPlayerWin -= Win;
    // }

    // private void Start()
    // {
    //     _gameOverPanel.SetActive(false);
    //     _pausePanel.SetActive(false);
    //     _gamePanel.SetActive(false);
    //     _startPanel.SetActive(true);
    //     _winPanel.SetActive(false);
    // }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         Pause();
    //     }
    // }

    // public void Pause()
    // {
    //     _pausePanel.SetActive(true);
    //     _gamePanel.SetActive(false);
    //     Time.timeScale = 0;
    // }

    // public void Resume()
    // {
    //     _pausePanel.SetActive(false);
    //     _gamePanel.SetActive(true);
    //     Time.timeScale = 1;
    // }

    // public void GameOver()
    // {
    //     _gameOverPanel.SetActive(true);
    //     _gamePanel.SetActive(false);
    //     Time.timeScale = 0;
    // }

    // public void Win()
    // {
    //     _winPanel.SetActive(true);
    //     _gamePanel.SetActive(false);
    //     Time.timeScale = 0;
    // }

    // public void StartGame()
    // {
    //     _startPanel.SetActive(false);
    //     _gamePanel.SetActive(true);
    //     Time.timeScale = 1;
    // }

    // public void Quit()
    // {
    //     Application.Quit();
    // }


}
