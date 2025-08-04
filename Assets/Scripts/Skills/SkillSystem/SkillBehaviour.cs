using System;
using UnityEngine;

public abstract class SkillBehaviour : MonoBehaviour
{
    public SkillDefinition Definition { get; private set; }
    protected ActorContext Context { get; private set; }
    protected PlayerContext PlayerCtx    => Context as PlayerContext;
    protected CompanionContext CompanionCtx => Context as CompanionContext;

    public virtual void Inject(SkillDefinition definition, ActorContext context)
    {
        Definition = definition;
        Context = context;
    }
}

public abstract class ActiveSkillBehaviour : SkillBehaviour
{
    protected float Cooldown => Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Cooldown), Definition.Cooldown);
    protected float Damage => Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Damage), Definition.Damage);
    protected float Radius => Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Radius), Definition.Raduis);
    protected float Duration => Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Duration), Definition.Duration);

    public  bool  IsReady => _cooldownTimer <= 0f;
    public  float RemainingCooldown => _cooldownTimer;

    protected float _cooldownTimer;

    public Action<float> OnCooldownStarted;
    public Action<float> OnSkillActivated;

    public virtual void TryCast()
    {
        OnSkillActivated?.Invoke(Duration);
    }
    
    public void ReduceCooldownByPercent(float percent01)
    {
        percent01 = Mathf.Clamp01(percent01);
        if (percent01 <= 0f || IsReady) return;
        _cooldownTimer -= _cooldownTimer * percent01;
    }

    public void SetCooldown(float seconds)
    {
        _cooldownTimer = Mathf.Max(0f, seconds);
    }
    
    protected virtual void Update()
    {
        if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;
    }

    protected void StartCooldown()
    {
        _cooldownTimer = Cooldown;
        OnCooldownStarted?.Invoke(_cooldownTimer);
    }
}


public abstract class PassiveSkillBehaviour : SkillBehaviour
{
    public abstract void EnablePassive();
    public abstract void DisablePassive();
}
