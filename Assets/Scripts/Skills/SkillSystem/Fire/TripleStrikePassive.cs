using UnityEngine;
public sealed class TripleStrikePassive : PassiveSkillBehaviour
{
    [SerializeField] private int _sideProjectiles = 1;
    public override void EnablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
        
        TryAttach(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Basic));
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;

        if (PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Basic) is FireballSkill fb)
            fb.SetExtraProjectiles(0);
    }
    
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Basic) TryAttach(beh);
    }

    private void TryAttach(ActiveSkillBehaviour beh)
    {
        if (beh is FireballSkill fb)
            fb.SetExtraProjectiles(_sideProjectiles);
    }
}
