using UnityEngine;
public sealed class SunstrikePassive : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [SerializeField] private float _bonusPercent   = 0.25f;
    [SerializeField] private float _igniteDuration = 5f;
    private float BonusMul => 1f + Mathf.Clamp01(_bonusPercent);

    public override void EnablePassive()
    {
        PlayerContext.RegisterOnDamageDealtModifier(this);
    }

    public override void DisablePassive()
    {
        PlayerContext.UnregisterOnDamageDealtModifier(this);
    }
    
    public void OnDamageDealt(IDamageable target, float damage, SkillDamageType type, PlayerContext ctx)
    {
        if (target is not IDotReceivable {IsDotActive: true} dot) return;
        
        float extra = damage * _bonusPercent;
        target.ReceiveDamage(extra, type);
        
        ctx.FireOnDamageDealt(target, extra, type);
        
        dot.RefreshDot(_igniteDuration);
    }
}