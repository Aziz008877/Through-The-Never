using UnityEngine;
public class WildfirePassive : PassiveSkillBehaviour
{
    private PlayerDashSkill _dash;

    public override void EnablePassive()
    {
        Attach(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Dash));
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;
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
            _dash.OnDashStarted += FireSeekingFireball;
        }
    }

    private void Detach()
    {
        if (_dash == null) return;
        _dash.OnDashStarted -= FireSeekingFireball;
        _dash = null;
    }
    
    private void FireSeekingFireball(Vector3 startPos)
    {
        if (PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Basic) is not FireballSkill fb)
            return;
        
        fb.SetHomingProjectiles(true);
        
        fb.TryCast();

        fb.SetHomingProjectiles(false);
    }
}