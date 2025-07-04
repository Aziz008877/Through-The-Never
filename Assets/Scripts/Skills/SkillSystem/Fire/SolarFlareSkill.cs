
public class SolarFlareSkill : ActiveSkillBehaviour
{
    public override void TryCast()
    {
        if (!IsReady) return;                  // идёт КД
        if (PlayerContext.SolarFlareCharge) return; // заряд уже есть

        PlayerContext.SolarFlareCharge = true; // зачаровали следующую базу
        StartCooldown();
    }
}