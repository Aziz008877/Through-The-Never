using UnityEngine;

public class FireInnateSkill : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [SerializeField] private float _dotPercent = 0.25f;
    [SerializeField] private float _dotDuration = 3f;
    public override void EnablePassive()
    {
        PlayerContext.RegisterOnDamageDealtModifier(this);
    }
    public override void DisablePassive()
    {
        PlayerContext.UnregisterOnDamageDealtModifier(this);
    }
    public void OnDamageDealt(IDamageable target, float damage, SkillDamageType type, PlayerContext context)
    {
        if (type == SkillDamageType.Basic)
        {
            if (target is IDotReceivable dot)
                dot.ApplyDot(damage * _dotPercent, _dotDuration);
        }
    }
}