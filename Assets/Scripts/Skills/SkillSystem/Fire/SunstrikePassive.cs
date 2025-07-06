using UnityEngine;

public sealed class SunstrikePassive : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] private float _damageBonusPercent = 0.25f;
    private float DamageMultiplier => 1f + Mathf.Max(0f, _damageBonusPercent);
    public override void EnablePassive()
    {
        PlayerContext.SkillModifierHub.Register(this);
    }

    public override void DisablePassive()
    {
        PlayerContext.SkillModifierHub.Unregister(this);
    }
    public float Evaluate(SkillKey key, float currentValue)
    {
        if (key.Stat == SkillStat.Damage && key.Slot != SkillSlot.Passive)
            return currentValue * DamageMultiplier;
        
        return currentValue;
    }
}