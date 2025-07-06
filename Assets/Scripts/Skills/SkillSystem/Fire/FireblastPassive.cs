
public sealed class FireblastPassive : PassiveSkillBehaviour
{
    public override void EnablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActive;
        TryAttach(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Basic));
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActive;
        TryDetach(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Basic));
    }

    private void OnActive(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Basic) TryAttach(beh);
    }
    
    private void TryAttach(ActiveSkillBehaviour beh)
    {
        if (beh is FireballSkill fireballSkill) fireballSkill.SetSmallExplosion(true);
    }

    private void TryDetach(ActiveSkillBehaviour beh)
    {
        if (beh is FireballSkill fireballSkill) fireballSkill.SetSmallExplosion(false);
    }
}