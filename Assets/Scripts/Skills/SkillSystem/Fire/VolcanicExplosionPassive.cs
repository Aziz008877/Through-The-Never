using UnityEngine;
public sealed class VolcanicExplosionPassive : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] private float _radiusBonusPercent = 0.30f;
    private float RadiusMultiplier => 1f + Mathf.Max(0f, _radiusBonusPercent);

    public override void EnablePassive()
    {
        PlayerContext.SkillModifierHub.Register(this);
    }

    public override void DisablePassive()
    {
        PlayerContext.SkillModifierHub.Unregister(this);
    }
    
    public float Evaluate(SkillKey key, float baseValue)
    {
        if (key.Stat == SkillStat.Radius && key.Slot != SkillSlot.Passive)
            return baseValue * RadiusMultiplier;
        return baseValue;
    }
}