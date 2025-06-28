using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private float _currentHP, _minHp, _maxHP;
    [SerializeField] private UnityEvent _onPlayerDead;

    [Header("HP Visuals")]
    [SerializeField] private Image _hpFillValue;
    [SerializeField] private GameObject _edgeGlowEffect;
    [SerializeField] private ParticleSystem _healVFX;

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
        _edgeGlowEffect.SetActive(true);
        _hpFillValue.fillAmount = _currentHP / _maxHP;

        OnHpValueUpdated?.Invoke(_currentHP);
        
        if (_currentHP <= _minHp)
        {
            OnPlayerDead?.Invoke();
            _onPlayerDead?.Invoke();
        }
    }

    public void ReceiveHP(float hpValue)
    {
        _healVFX.Play();
        _currentHP += hpValue;
        UpdateHP();
    }

    public void ReceiveDamage(float damageValue)
    {
        _currentHP -= damageValue;
        UpdateHP();
    }

    private void ClampHP()
    {
        _currentHP = Mathf.Clamp(_currentHP, _minHp, _maxHP);
    }
}