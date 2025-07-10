using System;
using UnityEngine;

public abstract  class SkillBehaviour : MonoBehaviour
{
    public SkillDefinition Definition { get; private set; }
    protected PlayerContext PlayerContext { get; private set; }

    public virtual void Inject(SkillDefinition definition, PlayerContext context)
    {
        Definition = definition;
        PlayerContext = context;
    }
}

public abstract class ActiveSkillBehaviour : SkillBehaviour
{
    protected float Cooldown => PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Cooldown), Definition.Cooldown);
    protected float Damage => PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Damage), Definition.Damage);
    protected float Radius => PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Radius), Definition.Raduis);
    protected float Duration => PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Duration), Definition.Duration);

    public  bool  IsReady => _cooldownTimer <= 0f;
    public  float RemainingCooldown => _cooldownTimer;

    protected float _cooldownTimer;

    public event Action<float> OnCooldownStarted;

    public abstract void TryCast();
    
    public void ReduceCooldownByPercent(float percent01)
    {
        percent01 = Mathf.Clamp01(percent01);
        if (percent01 <= 0f || IsReady) return;
        _cooldownTimer -= _cooldownTimer * percent01;
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
