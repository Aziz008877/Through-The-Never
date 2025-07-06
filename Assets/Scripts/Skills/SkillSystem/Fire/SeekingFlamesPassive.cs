
public sealed class SeekingFlamesPassive : PassiveSkillBehaviour
{
    public override void EnablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
        TryAttach(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Basic));
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;
        if (PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Basic) is FireballSkill fb)
            fb.SetHomingProjectiles(false);
    }

    void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Basic) TryAttach(beh);
    }

    void TryAttach(ActiveSkillBehaviour beh)
    {
        if (beh is FireballSkill fb) fb.SetHomingProjectiles(true);
    }
}