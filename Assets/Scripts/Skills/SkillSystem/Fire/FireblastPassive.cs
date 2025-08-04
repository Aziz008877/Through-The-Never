
public sealed class FireblastPassive : PassiveSkillBehaviour
{
    public override void EnablePassive()
    {
        Context.SkillManager.ActiveRegistered += OnActive;
        TryAttach(Context.SkillManager.GetActive(SkillSlot.Basic));
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActive;
        TryDetach(Context.SkillManager.GetActive(SkillSlot.Basic));
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