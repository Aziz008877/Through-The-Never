/*using UnityEngine;

public class SunstrikePassive : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [SerializeField] private float _bonusPercent   = 0.25f; // +25 %
    [SerializeField] private float _igniteDuration = 5f;    // новая длит.

    float BonusMul => 1f + Mathf.Clamp01(_bonusPercent);

    public override void EnablePassive()  => PlayerContext.RegisterOnDamageDealtModifier(this);
    public override void DisablePassive() => PlayerContext.UnregisterOnDamageDealtModifier(this);

    /* IOnDamageDealtModifier ------------------------------------------------#1#
    public void OnDamageDealt(IDamageable target,
        ref float   damage,
        ref SkillDamageType type,
        PlayerContext ctx)
    {
        //   1. Проверяем, есть ли активный Ignite
        if (target is IDotReceivable dot && dot.IsDotActive)
        {
            // 2. Увеличиваем текущий удар
            damage *= BonusMul;

            // 3. Продлеваем / обновляем Ignite до 5 сек
            dot.RefreshDot(_igniteDuration);
        }
    }
}*/