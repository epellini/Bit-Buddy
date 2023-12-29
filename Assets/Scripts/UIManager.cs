using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private StatsManager _statsManager;
    [SerializeField] private Image _hungerMeter, _thirstMeter, _cleanlinessMeter, _funMeter, _happinessMeter; // Declare images
    [SerializeField] private Image _healthStatus;
    [SerializeField] private Sprite _healthySprite;
    [SerializeField] private Sprite _sickSprite;


    private void FixedUpdate()
    {
        _hungerMeter.fillAmount = _statsManager.HungerPercent; // Set the fill amount of the image to the hunger percent
        _thirstMeter.fillAmount = _statsManager.ThirstPercent; // Set the fill amount of the image to the thirst percent
        _cleanlinessMeter.fillAmount = _statsManager.CleanlinessPercent; // Set the fill amount of the image to the cleanliness percent
        _funMeter.fillAmount = _statsManager.FunPercent; // Set the fill amount of the image to the fun percent
        _happinessMeter.fillAmount = _statsManager.HappinessPercent; // Set the fill amount of the image to the happiness percent


        if (_statsManager.CurrentHealthStatus == StatsManager.HealthStatus.Healthy)
        {
            _healthStatus.sprite = _healthySprite;
        }
        else
        {
            _healthStatus.sprite = _sickSprite;
        }
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
