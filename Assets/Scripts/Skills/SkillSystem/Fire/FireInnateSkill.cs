using UnityEngine;

public class FireInnateSkill : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [SerializeField] private float _dotPercent = 0.25f;
    [SerializeField] private float _dotDuration = 3f;
    public override void EnablePassive()
    {
        Context.RegisterOnDamageDealtModifier(this);
    }
    public override void DisablePassive()
    {
        Context.UnregisterOnDamageDealtModifier(this);
    }
    public void OnDamageDealt(IDamageable target, float damage, SkillDamageType type, ActorContext context)
    {
        if (type == SkillDamageType.Basic)
        {
            if (target is IDotReceivable dot)
                dot.ApplyDot(damage * _dotPercent, _dotDuration);
        }
    }
}