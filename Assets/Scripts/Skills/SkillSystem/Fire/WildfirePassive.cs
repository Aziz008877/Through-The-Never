using UnityEngine;
public class WildfirePassive : PassiveSkillBehaviour
{
    private PlayerDashSkill _attachedDash;

    public override void EnablePassive()
    {
        TryAttach(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Dash));
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        Detach();
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) TryAttach(beh);
    }

    private void TryAttach(ActiveSkillBehaviour dashSkill)
    {
        Detach();

        if (dashSkill != null && dashSkill.TryGetComponent<PlayerDashSkill>(out var dash))
        {
            dash.OnDashStarted += FireBasicSkill;
            _attachedDash = dash;
        }
    }

    private void Detach()
    {
        if (_attachedDash != null)
        {
            _attachedDash.OnDashStarted -= FireBasicSkill;
            _attachedDash = null;
        }
    }

    private void FireBasicSkill(Vector3 startPosition)
    {
        var basic = PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Basic);
        if (basic == null) return;
        
        basic.TryCast();
    }
}