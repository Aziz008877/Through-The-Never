using UnityEngine;
using System.Collections.Generic;
public abstract class ActorContext : MonoBehaviour
{
    [field: SerializeField] public Transform ActorPosition { get; private set; }
    [field: SerializeField] public Transform CastPivot { get; private set; }
    [field: SerializeField] public SkillModifierHub SkillModifierHub { get; set; }

    public Camera MainCamera;
    public Renderer[] PlayerMeshes;
    public abstract IActorHp Hp { get; }
    public abstract IActorMove Move { get; }
    public abstract IActorAnim Animator { get; }
    public abstract IActorState State { get; }
    public virtual  ISkillManager SkillManager => null;
    public virtual  IEnemyHandler EnemyHandler => null;
    public float CritChance { get; private set; } = 0f;
    public float CritMultiplier { get; set; } = 2f;
    public bool SolarFlareCharge   { get; set; }
    public bool DeepfrostWaterMode { get; set; }
    public GameObject FireballModel;
    readonly List<IDamageContextModifier> _ctxMods = new();
    readonly List<IOnDamageDealtContextModifier> _dealCtxMods = new();
    public void RegisterContextModifier(IDamageContextModifier m) => _ctxMods.Add(m);
    public void UnregisterContextModifier(IDamageContextModifier m) => _ctxMods.Remove(m);
    public void RegisterOnDamageDealtContextModifier(IOnDamageDealtContextModifier m) => _dealCtxMods.Add(m);
    public void UnregisterOnDamageDealtContextModifier(IOnDamageDealtContextModifier m) => _dealCtxMods.Remove(m);
    public void ApplyDamageContextModifiers(ref DamageContext ctx)
    {
        for (int i = 0; i < _ctxMods.Count; i++) _ctxMods[i].Apply(ref ctx);
    }

    public void FireOnDamageDealtContext(in DamageContext ctx)
    {
        for (int i = 0; i < _dealCtxMods.Count; i++) _dealCtxMods[i].OnDamageDealt(ctx);
    }

    public void AddCritChance(float delta)     => CritChance     = Mathf.Max(0f, CritChance + delta);
    public void AddCritMultiplier(float delta) => CritMultiplier += delta;
}