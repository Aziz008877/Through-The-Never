using System;
using UnityEngine;
using UnityEngine.Events;
public class PlayerHP : MonoBehaviour
{
    [SerializeField] private float _currentHP, _minHp, _maxHP;
    [SerializeField] private UnityEvent _onPlayerDead;
    private bool _canBeDamaged = true;

    public Action<float> OnHpValueUpdated;
    public Action OnPlayerDead;

    private void Start()
    {
        ClampHP();
        UpdateHP();
    }

    private void UpdateHP()
    {
        ClampHP();

        OnHpValueUpdated?.Invoke(_currentHP);
        
        if (_currentHP <= _minHp)
        {
            OnPlayerDead?.Invoke();
            _onPlayerDead?.Invoke();
        }
    }

    public void ReceiveHP(float hpValue)
    {
        _currentHP += hpValue;
        UpdateHP();
    }

    public void ReceiveDamage(float damageValue)
    {
        _currentHP -= damageValue;
        UpdateHP();
    }

    public void SetCanBeDamagedState(bool state)
    {
        _canBeDamaged = state;
    }

    private void ClampHP()
    {
        _currentHP = Mathf.Clamp(_currentHP, _minHp, _maxHP);
    }
}