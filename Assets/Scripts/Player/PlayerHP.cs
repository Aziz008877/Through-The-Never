using System;
using UnityEngine;
using UnityEngine.Events;
public class PlayerHP : MonoBehaviour
{
    [SerializeField] private float _currentHP, _minHp, _maxHP;
    [SerializeField] private UnityEvent _onPlayerDead;
    public delegate void IncomingDamageHandler(ref float damage);
    private bool _canBeDamaged = true;
    public float CurrentHP => _currentHP;
    public float MinHP => _minHp;
    public float MaxHP => _maxHP;
    public Action<float> OnHpValueUpdated;
    public Action OnPlayerDead;
    public Action<float> OnPlayerReceivedDamage;
    public event IncomingDamageHandler OnIncomingDamage;
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
        if (!_canBeDamaged || damageValue <= 0) return;
        
        OnIncomingDamage?.Invoke(ref damageValue);
        if (damageValue <= 0f) return;   
        
        _currentHP = Mathf.Max(_currentHP - damageValue, _minHp);
        OnPlayerReceivedDamage?.Invoke(damageValue);
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