/*using UnityEngine;

public sealed class SunstrikePassive : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [SerializeField] float _bonusPercent   = 0.25f;   // +25 %
    [SerializeField] float _igniteDuration = 5f;      // новая длительность

    float BonusMul => 1f + Mathf.Clamp01(_bonusPercent);

    public override void EnablePassive()
    {
        PlayerContext.RegisterOnDamageDealtModifier(this);
    }

    public override void DisablePassive()
    {
        PlayerContext.UnregisterOnDamageDealtModifier(this);
    }

    /* ------------ IOnDamageDealtModifier -------------- #1#
    public void OnDamageDealt(IDamageable target,
        ref float dmg,
        ref SkillDamageType type,
        PlayerContext ctx)
    {
        if (target is IDotReceivable dot && dot.IsDotActive)
        {
            // 1) усиливаем текущий удар
            dmg *= BonusMul;

            // 2) продлеваем Ignite до 5 сек
            dot.RefreshDot(_igniteDuration);
        }
    }
}*/