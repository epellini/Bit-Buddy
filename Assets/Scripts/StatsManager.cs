            using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatsManager : MonoBehaviour
{
    [Header("Hunger")]
    [SerializeField] private float _maxHunger;
    [SerializeField] private float _hungerDecreaseRate = 1f;
    private float _currentHunger;

    public float HungerPercent => _currentHunger / _maxHunger;


    [Header("Thirst")]
    [SerializeField] private float _maxThirst;
    [SerializeField] private float _thirstDecreaseRate = 1f;
    private float _currentThirst;

    public float ThirstPercent => _currentThirst / _maxThirst;

    public static UnityAction OnPlayerDeath;

    private void Start()
    {
        _currentHunger = _maxHunger;
        _currentThirst = _maxThirst;
    }


    private void Update()
    {
        _currentHunger -= _hungerDecreaseRate * Time.deltaTime;
        _currentThirst -= _thirstDecreaseRate * Time.deltaTime;

        if (_currentHunger <= 0 || _currentThirst <= 0)
        {
            OnPlayerDeath?.Invoke();
            Debug.Log("You died");
             _currentHunger = 0;
            _currentThirst = 0;
        }
   
    }


    public void IncreaseHunger(float hungerAmount)
    {
        _currentHunger += hungerAmount;
        //_currentHunger = Mathf.Clamp(_currentHunger, 0, _maxHunger);
        if (_currentHunger > _maxHunger)
        {
            _currentHunger = _maxHunger;
        }

    }

    public void IncreaseThirst(float thirstAmount)
    {
        _currentThirst += thirstAmount;
        //_currentThirst = Mathf.Clamp(_currentThirst, 0, _maxThirst);
        if (_currentThirst > _maxThirst)
        {
            _currentThirst = _maxThirst;
        }
    }
}
