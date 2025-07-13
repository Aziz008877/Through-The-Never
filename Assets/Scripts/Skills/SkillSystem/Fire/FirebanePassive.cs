using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class FirebanePassive : PassiveSkillBehaviour, ISkillModifier
{
    [Header("Penalties & repeat")]
    [SerializeField] private float _nerfPercent = 0.40f;
    [SerializeField] private float _repeatDelay = 0.15f;
    private readonly Dictionary<ActiveSkillBehaviour, System.Action<float>> _handlers = new();
    public override void EnablePassive()
    {
        PlayerContext.SkillModifierHub.Register(this);
        
        if (PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Special) is { } s)
            Hook(s);
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        PlayerContext.SkillModifierHub.Unregister(this);
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;

        foreach (var kv in _handlers)
            kv.Key.OnCooldownStarted -= kv.Value;

        _handlers.Clear();
    }
    
    public float Evaluate(SkillKey key, float baseValue)
    {
        if (key.Stat is SkillStat.Damage or SkillStat.Radius or SkillStat.Duration)
            return baseValue * (1f - _nerfPercent);

        return baseValue;
    }
    
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Special) Hook(beh);
    }

    private void Hook(ActiveSkillBehaviour skill)
    {
        if (skill == null || _handlers.ContainsKey(skill)) return;

        System.Action<float> h = _ => RepeatCast(skill);
        _handlers.Add(skill, h);
        skill.OnCooldownStarted += h;
    }

    private void RepeatCast(ActiveSkillBehaviour skill)
    {
        if (!enabled || skill == null) return;
        PlayerContext.StartCoroutine(DuplicateRoutine(skill));
    }

    private IEnumerator DuplicateRoutine(ActiveSkillBehaviour skill)
    {
        yield return new WaitForSeconds(_repeatDelay);
        
        if (_handlers.TryGetValue(skill, out var h))
            skill.OnCooldownStarted -= h;
        
        float savedCd = skill.RemainingCooldown;
        skill.ReduceCooldownByPercent(1f);

        skill.TryCast();
        skill.SetCooldown(savedCd);
        skill.OnCooldownStarted += h;
    }
}
