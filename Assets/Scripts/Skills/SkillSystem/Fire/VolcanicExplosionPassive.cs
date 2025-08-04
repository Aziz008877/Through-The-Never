using UnityEngine;

public sealed class VolcanicExplosionPassive : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] [Range(0f, 2f)]
    private float _radiusBonusPercent = 0.30f;          // +30 %

    private float RadiusMult => 1f + Mathf.Max(0f, _radiusBonusPercent);

    /* ───────── lifecycle ───────── */
    public override void EnablePassive()
    {
        Context.SkillModifierHub.Register(this);
        Debug.Log($"<color=orange>[Volcanic Explosion]</color> enabled " +
                  $"(+{_radiusBonusPercent:P0} radius)");
    }

    public override void DisablePassive()
    {
        Context.SkillModifierHub.Unregister(this);
        Debug.Log("<color=orange>[Volcanic Explosion]</color> disabled");
    }

    /* ───────── ISkillModifier ───────── */
    public float Evaluate(SkillKey key, float value)
    {
        // применяем ТОЛЬКО к активным AOE (например Special, BasicCast и т.п.)
        if (key.Stat != SkillStat.Radius || key.Slot == SkillSlot.Passive)
            return value;

        float boosted = value * RadiusMult;
        Debug.Log($"<color=orange>[Volcanic Explosion]</color> {key.Slot} " +
                  $"radius {value:F1} → {boosted:F1}");
        return boosted;
    }
}