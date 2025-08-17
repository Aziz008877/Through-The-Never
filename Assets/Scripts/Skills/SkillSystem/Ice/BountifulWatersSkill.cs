using UnityEngine;
public class BountifulWatersSkill : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] private float _cooldownMultiplier = 0.8f;
    [SerializeField] private float _minCooldown = 0.15f;
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
        if (key.Stat != SkillStat.Cooldown || key.Slot == SkillSlot.Passive)
            return baseValue;

        float result = Mathf.Max(baseValue * _cooldownMultiplier, _minCooldown);
        //Debug.Log($"[BountifulWaters] {key.Slot} cd: {baseValue:F2} → {result:F2}");
        return result;
    }

}
