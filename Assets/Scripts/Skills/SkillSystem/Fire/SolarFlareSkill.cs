using UnityEngine;

public class SolarFlareSkill : ActiveSkillBehaviour
{
    public override void TryCast()
    {
        if (!IsReady) return;
        if (Context.SolarFlareCharge) return;

        base.TryCast();
        Context.SolarFlareCharge = true;

        StartCooldown();
    }
}