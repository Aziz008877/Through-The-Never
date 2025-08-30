using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour, IActorHp
{
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private Image _hpFillValue;
    [SerializeField] private float _currentHP, _minHp, _maxHP;
    [SerializeField] private UnityEvent _onPlayerDead;
    private bool _canBeDamaged = true;
    private Transform _lastAggressor;
    private bool _deadReported;
    public float CurrentHP { get => _currentHP; set => _currentHP = value; }
    public float MinHP { get => _minHp; set => _minHp = value; }
    public float MaxHP { get => _maxHP; set => _maxHP = value; }
    public Action<float> OnHpValueUpdated;
    public Action OnPlayerDead;
    public event IncomingDamageHandler OnIncomingDamage;
    public Action<float, IDamageable> OnActorReceivedDamage { get; set; }
    public Action OnActorDead { get; set; }

    private void Start()
    {
        ClampHP();
        UpdateHP();
    }
    
    public void ReceiveDamage(float damageValue, IDamageable source)
    {
        if (!_canBeDamaged || damageValue <= 0f) return;

        OnIncomingDamage?.Invoke(ref damageValue, source);
        if (damageValue <= 0f) return;
        
        if (source is Component c && c) _lastAggressor = c.transform;

        _currentHP = Mathf.Max(_currentHP - damageValue, _minHp);
        OnActorReceivedDamage?.Invoke(damageValue, source);
        UpdateHP();
    }
    
    public void ApplyDamage(DamageContext ctx)
    {
        if (!_canBeDamaged || ctx.Damage <= 0f) return;
        
        Transform killer = ctx.Attacker ? ctx.Attacker.transform : null;

        if (!killer && ctx.SourceGO)
        {
            var owner = ctx.SourceGO.GetComponentInParent<IHasOwner>();
            if (owner?.Owner) killer = owner.Owner;

            if (!killer)
            {
                var ac = ctx.SourceGO.GetComponentInParent<ActorContext>();
                if (ac) killer = ac.transform;
            }

            if (!killer) killer = ctx.SourceGO.transform;
        }

        _lastAggressor = killer;
        
        float dmg = ctx.Damage;
        if (OnIncomingDamage != null)
        {
            OnIncomingDamage.Invoke(ref dmg, null);
            if (dmg <= 0f) return;
        }

        _currentHP = Mathf.Max(_currentHP - dmg, _minHp);
        OnActorReceivedDamage?.Invoke(dmg, null);
        UpdateHP();
    }

    public void UpdateHP()
    {
        ClampHP();
        OnHpValueUpdated?.Invoke(_currentHP);

        if (_currentHP <= _minHp)
        {
            if (!_deadReported)
            {
                _deadReported = true;
                if (_lastAggressor)
                    PlayerDeathReporter.ReportPlayerKilledBy(_lastAggressor);
            }

            OnPlayerDead?.Invoke();
            _onPlayerDead?.Invoke();
            OnActorDead?.Invoke();
        }

        if (_hpText) _hpText.text = $"{_currentHP} / {_maxHP}";
        if (_hpFillValue) _hpFillValue.fillAmount = Mathf.Clamp01(CurrentHP / MaxHP);
    }

    public void ReceiveHP(float hpValue)
    {
        _currentHP += hpValue;
        UpdateHP();
    }

    public void SetCanBeDamagedState(bool state) => _canBeDamaged = state;

    private void ClampHP() => _currentHP = Mathf.Clamp(_currentHP, _minHp, _maxHP);

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
        _deadReported = false;
        _lastAggressor = null;
        UpdateHP();
        SetCanBeDamagedState(true);
    }
}
