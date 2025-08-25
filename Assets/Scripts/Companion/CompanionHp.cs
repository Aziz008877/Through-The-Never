using System;
using System.Collections;
using UnityEngine;

public class CompanionHp : MonoBehaviour, IActorHp
{
    [SerializeField] float _maxHp = 100f;
    [SerializeField] float _dotTickRate = 1f;

    public float CurrentHP { get; set; }
    public float MinHP { get; set; } = 0f;
    public float MaxHP { get; set; }
    public bool  CanBeDamaged { get; set; } = true;
    public bool  IsDotActive  { get; set; }

    public event Action<Transform>           OnEnemyDead;
    public void ReceiveDamage(float damageValue, IDamageable source)
    {
        throw new NotImplementedException();
    }

    public event IncomingDamageHandler       OnIncomingDamage;     // NEW
    public void SetCanBeDamagedState(bool state)
    {
        throw new NotImplementedException();
    }

    public void Revive(float percent)
    {
        throw new NotImplementedException();
    }

    public void AddMaxHP(float amount, bool healToFull = true)
    {
        throw new NotImplementedException();
    }

    public void RemoveMaxHP(float amount)
    {
        throw new NotImplementedException();
    }

    public Action OnActorDead { get; set; }
    public Action<float, IDamageable> OnActorReceivedDamage { get; set; }

    void Awake() => CurrentHP = MaxHP = _maxHp;

    /* ---------- Damage ---------- */
    public void ReceiveDamage(float dmg, SkillDamageType t = SkillDamageType.Basic)
    {
        OnIncomingDamage?.Invoke(ref dmg, null);
        if (dmg <= 0f || !CanBeDamaged) return;

        CurrentHP = Mathf.Max(MinHP, CurrentHP - dmg);
        if (CurrentHP <= MinHP) Die();
    }

    /* ---------- Heal ---------- */
    public void UpdateHP()
    {
        
    }

    public void ReceiveHP(float heal) =>
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + heal);

    /* ---------- DOT ---------- */
    public void ApplyDot(float dps, float dur) => StartCoroutine(DotRoutine(dps, dur));
    public void RefreshDot(float dur)           => ApplyDot(0, dur);

    IEnumerator DotRoutine(float dps, float dur)
    {
        IsDotActive = true;
        float t = 0f;
        while (t < dur)
        {
            ReceiveDamage(dps / _dotTickRate, SkillDamageType.DOT);
            yield return new WaitForSeconds(1f / _dotTickRate);
            t += 1f / _dotTickRate;
        }
        IsDotActive = false;
    }

    void Die() { OnEnemyDead?.Invoke(transform); Destroy(gameObject); }
}