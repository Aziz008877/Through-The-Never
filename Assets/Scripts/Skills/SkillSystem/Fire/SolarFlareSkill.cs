using UnityEngine;

public class SolarFlareSkill : ActiveSkillBehaviour
{
    public override void TryCast()
    {
        if (!IsReady) return;
        if (PlayerContext.SolarFlareCharge) return;

        base.TryCast();
        PlayerContext.SolarFlareCharge = true;

        StartCooldown();
    }
}