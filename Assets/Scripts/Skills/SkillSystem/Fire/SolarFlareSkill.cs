
using UnityEngine;

public class SolarFlareSkill : ActiveSkillBehaviour
{
    public override void TryCast()
    {
        if (!IsReady) return;
        if (PlayerContext.SolarFlareCharge) return;

        PlayerContext.SolarFlareCharge = true;
        Debug.Log(PlayerContext.SolarFlareCharge);
        StartCooldown();
    }
}