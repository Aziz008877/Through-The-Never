using UnityEngine;
public class BerserkersRagePassive : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] private float _maxBonusPercent = 0.50f;
    private float MaxMultiplier => 1f + Mathf.Max(0f, _maxBonusPercent);

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
        if (key.Stat != SkillStat.Damage || key.Slot == SkillSlot.Passive)
            return baseValue;

        float hpFrac = Mathf.Clamp01(PlayerContext.PlayerHp.CurrentHP / PlayerContext.PlayerHp.MaxHP);
        float mul = Mathf.Lerp(MaxMultiplier, 1f, hpFrac);
        return baseValue * mul;
    }
}