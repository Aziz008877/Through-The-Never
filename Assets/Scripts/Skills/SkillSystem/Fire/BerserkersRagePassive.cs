using UnityEngine;
public class BerserkersRagePassive : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] private float _maxBonusPercent = 0.50f;
    private float MaxMultiplier => 1f + Mathf.Max(0f, _maxBonusPercent);

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
        if (key.Stat != SkillStat.Damage || key.Slot == SkillSlot.Passive)
            return baseValue;

        float hpFrac = Mathf.Clamp01(Context.Hp.CurrentHP / Context.Hp.MaxHP);
        float mul = Mathf.Lerp(MaxMultiplier, 1f, hpFrac);
        return baseValue * mul;
    }
}