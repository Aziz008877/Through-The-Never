using System;
using UnityEngine;

public interface IDamageable
{
    float CurrentHP { get; set; }
    float MinHP { get; set; }
    float MaxHP { get; set; }
    bool CanBeDamaged { get; set; }
    Action<Transform> OnEnemyDead { get; set; }
    void ReceiveDamage(float damageValue, SkillDamageType type);
}
