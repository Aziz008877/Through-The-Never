using UnityEngine;

public class JetfirePassive : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] private float _speedMultiplier = 1.5f;
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
        if (key.Slot == SkillSlot.Dash && key.Stat == SkillStat.Cooldown)
        {
            return currentValue / _speedMultiplier;
        }
            
        return currentValue;
    }
}
