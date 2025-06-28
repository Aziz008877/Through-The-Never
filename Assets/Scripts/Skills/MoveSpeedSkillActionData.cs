using UnityEngine;

[CreateAssetMenu(fileName = "MoveSpeedSkill", menuName = "SkillActions/MoveSpeedSkill")]
public class MoveSpeedSkillActionData : SkillActionData
{
    public override void Activate(PlayerSkillHandler handler)
    {
        handler.PlayerMove.UpgradeMS();
    }
}
