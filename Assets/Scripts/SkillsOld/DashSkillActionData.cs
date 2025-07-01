using UnityEngine;

[CreateAssetMenu(fileName = "DashSkillActionData", menuName = "SkillActions/DashSkill")]
public class DashSkillActionData : SkillActionData
{
    public int SkillIDForUI = 0;

    public override void Activate(PlayerSkillHandler handler)
    {
        handler.PlayerDash.gameObject.SetActive(true);
        handler.SkillUIHandler.ShowUISkill(SkillIDForUI);
    }
}