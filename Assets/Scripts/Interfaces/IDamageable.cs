using System;
using UnityEngine;

public interface IDamageable
{
    float CurrentHP { get; set; }
    float MinHP { get; set; }
    float MaxHP { get; set; }
    bool  CanBeDamaged { get; set; }
    void ReceiveDamage(in DamageContext ctx);
    event Action<Transform> OnEnemyDead;
}
