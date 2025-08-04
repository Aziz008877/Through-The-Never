
public sealed class SeekingFlamesPassive : PassiveSkillBehaviour
{
    public override void EnablePassive()
    {
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
        TryAttach(Context.SkillManager.GetActive(SkillSlot.Basic));
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        if (Context.SkillManager.GetActive(SkillSlot.Basic) is FireballSkill fb)
            fb.SetHomingProjectiles(false);
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Basic) TryAttach(beh);
    }

    private void TryAttach(ActiveSkillBehaviour beh)
    {
        if (beh is FireballSkill fb) fb.SetHomingProjectiles(true);
    }
}