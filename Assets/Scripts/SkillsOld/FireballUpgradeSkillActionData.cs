using UnityEngine;

[CreateAssetMenu(fileName = "FireballUpgradeSkillActionData", menuName = "SkillActions/FireballUpgradeSkill")]
public class FireballUpgradeSkillActionData : SkillActionData
{
    public override void Activate(PlayerSkillHandler handler)
    {
        handler.FireballSkill.UpdateSkill();
    }
}