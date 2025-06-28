using UnityEngine;

[CreateAssetMenu(fileName = "FireBeamSkill", menuName = "SkillActions/FireBeamSkill")]
public class FireBeamSkillActionData : SkillActionData
{
    public override void Activate(PlayerSkillHandler handler)
    {
        handler.FirebeamSkill.gameObject.SetActive(true);
    }
}
