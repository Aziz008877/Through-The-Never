using System;
using UnityEngine;
using UnityEngine.Events;
public class PlayerHP : MonoBehaviour
{
    [SerializeField] private float _currentHP, _minHp, _maxHP;
    [SerializeField] private UnityEvent _onPlayerDead;
    public delegate void IncomingDamageHandler(ref float damage, IDamageable source);
    private bool _canBeDamaged = true;
    public float CurrentHP => _currentHP;
    public float MinHP => _minHp;
    public float MaxHP => _maxHP;
    public Action<float> OnHpValueUpdated;
    public Action OnPlayerDead;
    public Action<float, IDamageable> OnPlayerReceivedDamage;
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

    public void ReceiveDamage(float damageValue, IDamageable source)
    {
        if (!_canBeDamaged || damageValue <= 0f) return;

        OnIncomingDamage?.Invoke(ref damageValue, source);
        if (damageValue <= 0f) return;

        _currentHP = Mathf.Max(_currentHP - damageValue, _minHp);
        OnPlayerReceivedDamage?.Invoke(damageValue, source);
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
    
    public void AddMaxHP(float amount, bool healToFull = true)
    {
        _maxHP += amount;
        if (healToFull) _currentHP = _maxHP;
        UpdateHP();
    }

    public void RemoveMaxHP(float amount)
    {
        _maxHP = Mathf.Max(_minHp, _maxHP - amount);
        ClampHP();
        UpdateHP();
    }
    
    public void Revive(float percent01 = 1f)
    {
        percent01 = Mathf.Clamp01(percent01);
        _currentHP = Mathf.Max(_minHp, _maxHP * percent01);
        UpdateHP();
        SetCanBeDamagedState(true);
    }

}