using UnityEngine;
using Zenject;
using System.Collections.Generic;

public abstract class ActorContext : MonoBehaviour
{
    [field: SerializeField] public Transform ActorPosition { get; private set; }
    [field: SerializeField] public Transform CastPivot     { get; private set; }
    [field: SerializeField] public SkillModifierHub SkillModifierHub { get; set; }
    public Camera MainCamera;
    public virtual ISkillManager SkillManager => null;
    public Renderer[] PlayerMeshes;
    public GameObject FireballModel;
    public abstract IActorHp    Hp       { get; }
    public abstract IActorMove  Move     { get; }
    public abstract IActorAnim  Animator { get; }
    public abstract IActorState State    { get; }
    public float CritChance { get; private set; } = 0;
    public float CritMultiplier { get; set; } = 2f;
    public bool SolarFlareCharge { get; set; }
    readonly List<IDamageModifier> _dmgMods = new();
    readonly List<IOnDamageDealtModifier> _dealMods = new();
    public virtual IEnemyHandler EnemyHandler => null;

    public void RegisterModifier(IDamageModifier m)   => _dmgMods.Add(m);
    public void UnregisterModifier(IDamageModifier m) => _dmgMods.Remove(m);

    public void ApplyDamageModifiers(ref float dmg, ref SkillDamageType type)
    {
        foreach (var m in _dmgMods) m.Apply(ref dmg, ref type);
    }

    public void FireOnDamageDealt(IDamageable t, float d, SkillDamageType tp)
    {
        foreach (var m in _dealMods) m.OnDamageDealt(t, d, tp, this);
    }

    public void RegisterOnDamageDealtModifier(IOnDamageDealtModifier m)   => _dealMods.Add(m);
    public void UnregisterOnDamageDealtModifier(IOnDamageDealtModifier m) => _dealMods.Remove(m);
    
    public void AddCritChance(float delta)
    {
        CritChance = Mathf.Max(0, CritChance + delta);
    }
    
    public void AddCritMultiplier(float delta)
    {
        CritMultiplier += delta;
    }
}