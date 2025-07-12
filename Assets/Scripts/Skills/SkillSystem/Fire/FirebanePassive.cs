using System.Collections;
using UnityEngine;
public sealed class FirebanePassive : PassiveSkillBehaviour, ISkillModifier
{
    [Header("Penalties & repeat")]
    [SerializeField] private float _nerfPercent = 0.40f;
    [SerializeField] private float _repeatDelay = 0.15f;
    public override void EnablePassive()
    {
        PlayerContext.SkillModifierHub.Register(this);
        foreach (var kv in PlayerContext.PlayerSkillManager.Actives)
            Hook(kv.Value);

        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        PlayerContext.SkillModifierHub.Unregister(this);
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;

        foreach (var kv in PlayerContext.PlayerSkillManager.Actives)
            Unhook(kv.Value);
    }
    
    public float Evaluate(SkillKey key, float val)
    {
        if (key.Stat is SkillStat.Damage or SkillStat.Radius or SkillStat.Duration)
            return val * (1f - _nerfPercent);

        return val;
    }
    
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh) => Hook(beh);

    private void Hook(ActiveSkillBehaviour beh)
    {
        beh.OnCooldownStarted += cd => RepeatCast(beh);
    }

    private void Unhook(ActiveSkillBehaviour beh)
    {
        beh.OnCooldownStarted -= cd => RepeatCast(beh);
    }
    
    private void RepeatCast(ActiveSkillBehaviour skill)
    {
        if (skill == null) return;

        PlayerContext.StartCoroutine(DuplicateRoutine(skill));
    }

    private IEnumerator DuplicateRoutine(ActiveSkillBehaviour skill)
    {
        yield return new WaitForSeconds(_repeatDelay);
        
        skill.ReduceCooldownByPercent(1f);
        skill.TryCast();
        skill.ReduceCooldownByPercent(-1f);
        skill.OnCooldownStarted?.Invoke(skill.RemainingCooldown);
    }
}
