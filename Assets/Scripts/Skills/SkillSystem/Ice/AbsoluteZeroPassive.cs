using UnityEngine;

public sealed class AbsoluteZeroPassive : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] private float _damageMultiplier   = 2f;
    [SerializeField] private float _cooldownMultiplier = 1.5f;

    public override void EnablePassive()
    {
        Context.SkillModifierHub.Register(this);
    }

    public override void DisablePassive()
    {
        Context.SkillModifierHub.Unregister(this);
    }

    public float Evaluate(SkillKey key, float baseValue)
    {
        if (key.Slot != SkillSlot.Special) return baseValue;

        float result = baseValue;

        if (key.Stat == SkillStat.Damage)
        {
            result = baseValue * _damageMultiplier;
            Debug.Log($"[AbsoluteZero] {key.Slot} {key.Stat}: {baseValue:F2} → {result:F2} (x{_damageMultiplier})");
        }
        else if (key.Stat == SkillStat.Cooldown)
        {
            result = baseValue * _cooldownMultiplier;
            Debug.Log($"[AbsoluteZero] {key.Slot} {key.Stat}: {baseValue:F2} → {result:F2} (x{_cooldownMultiplier})");
        }

        return result;
    }
}