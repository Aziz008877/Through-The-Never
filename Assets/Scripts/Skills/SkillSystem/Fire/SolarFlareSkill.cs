
public class SolarFlareSkill : ActiveSkillBehaviour
{
    public override void TryCast()
    {
        if (!IsReady) return;
        if (PlayerContext.SolarFlareCharge) return;

        PlayerContext.SolarFlareCharge = true;
        StartCooldown();
    }
}