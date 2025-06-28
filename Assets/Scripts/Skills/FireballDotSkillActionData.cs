using UnityEngine;

[CreateAssetMenu(fileName = "FireballDotSkillActionData", menuName = "SkillActions/FireballDotSkill")]
public class FireballDotSkillActionData : SkillActionData
{
    public override void Activate(PlayerSkillHandler handler)
    {
        //handler.FireballSkill.SetDotMode(_dotDamagePerTick, _dotDuration, _dotTickRate);
        handler.FireballSkill.SetDotMode();
    }
}