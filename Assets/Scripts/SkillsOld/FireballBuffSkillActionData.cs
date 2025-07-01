using UnityEngine;

[CreateAssetMenu(fileName = "FireballBuffSkillActionData", menuName = "SkillActions/FireballBuffSkill")]
public class FireballBuffSkillActionData : SkillActionData
{
    public float buffPercent = 20f;

    public override void Activate(PlayerSkillHandler handler)
    {
        handler.FireballSkill.BuffDamage(buffPercent);
    }
}