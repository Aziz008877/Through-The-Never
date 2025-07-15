using UnityEngine;

public sealed class SunstrikePassive : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [SerializeField] private float _bonusPercent   = 0.25f;   // +25 %
    [SerializeField] private float _igniteDuration = 5f;

    float BonusMul => 1f + Mathf.Clamp01(_bonusPercent);

    public override void EnablePassive()  =>
        PlayerContext.RegisterOnDamageDealtModifier(this);

    public override void DisablePassive() =>
        PlayerContext.UnregisterOnDamageDealtModifier(this);

    // IOnDamageDealtModifier
    public void OnDamageDealt(IDamageable target,
        float damage,
        SkillDamageType type,
        PlayerContext ctx)
    {
        if (target is not IDotReceivable { IsDotActive: true } dot) return;

        // 1) наносим «усиленный» урон
        float boosted = damage * BonusMul;
        target.ReceiveDamage(boosted, type);
        ctx.FireOnDamageDealt(target, boosted, type);   // триггерим цепочку дальше

        // 2) продлеваем Ignite
        dot.RefreshDot(_igniteDuration);

#if UNITY_EDITOR
        Debug.Log($"[Sunstrike] +{boosted:0.##} bonus dmg", ctx);
#endif
    }
}