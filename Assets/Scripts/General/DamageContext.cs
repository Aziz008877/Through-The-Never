using System;
using UnityEngine;
public struct DamageContext
{
    public ActorContext Attacker;
    public IDamageable  Target;

    public SkillBehaviour SkillBehaviour;
    public SkillDefinition SkillDef;
    public SkillSlot Slot;
    public SkillDamageType Type;

    public float Damage;
    public bool IsCrit;
    public float CritMultiplier;

    public Vector3 HitPoint;
    public Vector3 HitNormal;
    public GameObject SourceGO;

    public bool  HasDot;
    public float DotDps;
    public float DotDuration;
    public float DotTickRate;

    public bool IsLethalPrediction;
}