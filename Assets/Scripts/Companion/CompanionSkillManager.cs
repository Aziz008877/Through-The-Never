using System;
using System.Collections.Generic;
using UnityEngine;
public class CompanionSkillManager : MonoBehaviour, ISkillManager
{
    private readonly Dictionary<SkillSlot, ActiveSkillBehaviour> _actives = new();
    public IReadOnlyDictionary<SkillSlot, ActiveSkillBehaviour> Actives => _actives;
    public event Action<SkillSlot, ActiveSkillBehaviour> ActiveRegistered;

    public void AddSkill(SkillSlot slot, ActiveSkillBehaviour behaviour)
    {
        if (_actives.ContainsKey(slot)) return;

        _actives[slot] = behaviour;
        ActiveRegistered?.Invoke(slot, behaviour);
    }

    public void AddPassive(PassiveSkillBehaviour behaviour)
    {
        behaviour.EnablePassive();
    }

    public ActiveSkillBehaviour GetActive(SkillSlot slot)
    {
        _actives.TryGetValue(slot, out var beh);
        return beh;
    }
}