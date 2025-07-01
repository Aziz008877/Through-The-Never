using UnityEngine;

[CreateAssetMenu(fileName = "FireBeamSkill", menuName = "SkillActions/FireBeamSkill")]
public class FireBeamSkillActionData : SkillActionData
{
    public int SkillIDForUI = 1;
    public override void Activate(PlayerSkillHandler handler)
    {
        handler.FirebeamSkill.gameObject.SetActive(true);
        handler.SkillUIHandler.ShowUISkill(SkillIDForUI);
    }
}
