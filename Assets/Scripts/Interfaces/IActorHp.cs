using System;
using System.Collections.Generic;
using UnityEngine;


public delegate void IncomingDamageHandler(ref float damage, IDamageable source);

public interface IActorHp
{
    float CurrentHP { get; set; }
    float MinHP     { get; set; }
    float MaxHP     { get; set; }
    void UpdateHP();
    void  ReceiveHP(float amount);
    void ReceiveDamage(float damageValue, IDamageable source);

    event IncomingDamageHandler OnIncomingDamage;

    void SetCanBeDamagedState(bool state);

    void Revive(float percent);

    public void AddMaxHP(float amount, bool healToFull = true);
    public void RemoveMaxHP(float amount);

    Action OnActorDead { get; set; }
    public Action<float, IDamageable> OnActorReceivedDamage { get; set; }
}

public interface IActorMove
{
    Vector3 LastMoveDirection { get; }
    public void RotateTowardsMouse(float customDuration = -1f);
    public void SetSpeedMultiplier(float multiplier);
}
public interface IActorAnim  { void Dash(); }
public interface IActorState { void ChangePlayerState(bool active); }

public interface ISkillManager
{
    event Action<SkillSlot, ActiveSkillBehaviour> ActiveRegistered;
    ActiveSkillBehaviour GetActive(SkillSlot slot);
    IReadOnlyDictionary<SkillSlot, ActiveSkillBehaviour> Actives { get; }
}

public interface IEnemyHandler
{
    public BaseEnemyHP[] Enemies { get; set; }
    event Action<Transform> OnEnemyKilled;
    event Action<IDamageable> EnemyRegistered;
    void RegisterEnemy(IDamageable dmg);
}
