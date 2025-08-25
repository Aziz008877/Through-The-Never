using UnityEngine;

public static class DamageApplyExtensions
{
    public static void ApplyNow(this ActiveSkillBehaviour skill, IDamageable target, float amount, SkillDamageType type, Vector3 hitPoint = default, Vector3 hitNormal = default)
    {
        if (skill == null || target == null) return;
        var ctx = skill.BuildDamage(amount, type, hitPoint, hitNormal, skill.gameObject);
        ctx.Target = target;
        target.ReceiveDamage(ctx);
        skill.Context.FireOnDamageDealtContext(ctx);
    }

    public static void ApplyDot(this ActiveSkillBehaviour skill, IDamageable target, float dps, float duration, float tickRate = 1f, Vector3 hitPoint = default, Vector3 hitNormal = default)
    {
        if (skill == null || target == null) return;
        var ctx = skill.BuildDot(dps, duration, tickRate, hitPoint, hitNormal, skill.gameObject);
        ctx.Target = target;
        target.ReceiveDamage(ctx);
        skill.Context.FireOnDamageDealtContext(ctx);
    }
}