public sealed class CryomaniacPassive : PassiveSkillBehaviour
{
    private PlayerDashSkill _dash;

    public override void EnablePassive()
    {
        Attach(Context.SkillManager.GetActive(SkillSlot.Dash));
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        Detach();
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) Attach(beh);
    }

    private void Attach(ActiveSkillBehaviour beh)
    {
        Detach();

        if (beh && beh.TryGetComponent(out PlayerDashSkill dash))
        {
            _dash = dash;
            //_dash.OnDashEnded += ResetAllCooldowns;
        }
    }

    private void Detach()
    {
        if (_dash == null) return;
        //_dash.OnDashKill -= ResetAllCooldowns;
        _dash = null;
    }

    private void ResetAllCooldowns()
    {
        foreach (var kv in Context.SkillManager.Actives)
            kv.Value.ReduceCooldownByPercent(1f);
    }
}