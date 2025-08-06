using UnityEngine;

public sealed class TripleStrikePassive : PassiveSkillBehaviour
{
    [SerializeField] private int _extraProjectiles = 1;

    public override void EnablePassive()
    {
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
        AttachIfFireball(Context.SkillManager.GetActive(SkillSlot.Basic));
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        if (Context.SkillManager.GetActive(SkillSlot.Basic) is FireballSkill fb)
            fb.SetExtraProjectiles(0);
    }
    
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour behaviour)
    {
        if (slot == SkillSlot.Basic) AttachIfFireball(behaviour);
    }

    private void AttachIfFireball(ActiveSkillBehaviour behaviour)
    {
        if (behaviour is FireballSkill fb)
        {
            fb.SetExtraProjectiles(_extraProjectiles);
        }
            
    }
}