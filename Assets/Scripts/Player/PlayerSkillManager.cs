using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class PlayerSkillManager : MonoBehaviour
{
    [SerializeField] private Transform _skillRoot;
    private readonly Dictionary<SkillSlot, ActiveSkillBehaviour> _actives = new();
    [Inject] private PlayerContext _context;
    [Inject] private PlayerInput _input;
    [Inject] private SkillRuntimeFactory _factory;
    private void OnEnable()
    {
        _input.OnBasicSkillPressed += () => Cast(SkillSlot.Basic);
        _input.OnDefensiveSkillPressed += () => Cast(SkillSlot.Defense);
        _input.OnSpecialSkillPressed += () => Cast(SkillSlot.Special);
        _input.OnDashPressed += () => Cast(SkillSlot.Dash);
    }
    
    public void AddSkills(List<SkillDefinition> newDefs)
    {
        Build(newDefs);
    }

    public void Build(List<SkillDefinition> skillsDefinitions)
    {
        foreach (var skillDefinition in skillsDefinitions)
        {
            var behaviour = _factory.Spawn(skillDefinition, _context, _skillRoot);

            if (skillDefinition.Kind == SkillKind.Active && behaviour is ActiveSkillBehaviour activeSkillBehaviour)
                _actives[skillDefinition.Slot] = activeSkillBehaviour;

            if (skillDefinition.Kind == SkillKind.Passive && behaviour is PassiveSkillBehaviour passiveSkillBehaviour)
                passiveSkillBehaviour.Enable();
        }
    }

    private void Cast(SkillSlot slot)
    {
        if (_actives.TryGetValue(slot, out var activeSkill))
        {
            activeSkill.TryCast();
        }
    }


    private void OnDestroy()
    {
        _input.OnBasicSkillPressed -= () => Cast(SkillSlot.Basic);
        _input.OnDefensiveSkillPressed -= () => Cast(SkillSlot.Defense);
        _input.OnSpecialSkillPressed -= () => Cast(SkillSlot.Special);
        _input.OnDashPressed -= () => Cast(SkillSlot.Dash);
    }
}
