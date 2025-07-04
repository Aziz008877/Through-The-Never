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
    public bool IsReady => _cooldownTimer <= 0f;
    public float RemainingCooldown => _cooldownTimer;
    public float TotalCooldown => Definition.Cooldown;
    public abstract void TryCast();
    protected float _cooldownTimer;
    public event Action<float> OnCooldownStarted;
    
    private void Update()
    {
        if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;
    }
    
    protected void StartCooldown()
    {
        _cooldownTimer = Definition.Cooldown;
        OnCooldownStarted?.Invoke(_cooldownTimer);
    }
}

public abstract class PassiveSkillBehaviour : SkillBehaviour
{
    public abstract void EnablePassive();
    public abstract void DisablePassive();
}
