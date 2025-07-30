using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerSkillManager : MonoBehaviour
{
    [SerializeField] private Transform _skillRoot;
    readonly Dictionary<SkillSlot, ActiveSkillBehaviour> _actives = new();
    public IReadOnlyDictionary<SkillSlot, ActiveSkillBehaviour> Actives => _actives;
    public event Action<SkillSlot, ActiveSkillBehaviour> ActiveRegistered;

    [Inject] private PlayerContext _context;
    [Inject] private PlayerInput _input;
    [Inject] private SkillRuntimeFactory _factory;
    
    private readonly List<SkillDefinition> _chosenSkills = new();
    public IReadOnlyList<SkillDefinition>  ChosenSkills => _chosenSkills;
    
    private void OnEnable()
    {
        _input.OnBasicSkillPressed += CastBasic;
        _input.OnDefensiveSkillPressed += CastDefense;
        _input.OnSpecialSkillPressed += CastSpecial;
        _input.OnDashPressed += CastDash;
    }
    private void OnDisable()
    {
        _input.OnBasicSkillPressed -= CastBasic;
        _input.OnDefensiveSkillPressed -= CastDefense;
        _input.OnSpecialSkillPressed -= CastSpecial;
        _input.OnDashPressed -= CastDash;
    }

    public void AddSkills(List<SkillDefinition> defs)
    {
        Build(defs);
        
        /*foreach (var def in defs)
        {
            Debug.Log(def.DisplayName);
        }*/
    }
    
    public void Build(List<SkillDefinition> skillDefinitions)
    {
        foreach (SkillDefinition definition in skillDefinitions)
        {
            if (!_chosenSkills.Contains(definition)) _chosenSkills.Add(definition);
            
            if (definition.Kind == SkillKind.Active && !_actives.ContainsKey(definition.Slot))
            {
                ActiveSkillBehaviour behaviour = _factory.Spawn(definition, _context, _skillRoot) as ActiveSkillBehaviour;

                if (behaviour != null)
                {
                    _actives[definition.Slot] = behaviour;
                    ActiveRegistered?.Invoke(definition.Slot, behaviour);
                }
                continue;
            }
            
            if (definition.Kind == SkillKind.Passive)
            {
                PassiveSkillBehaviour passive = _factory.Spawn(definition, _context, _skillRoot) as PassiveSkillBehaviour;

                if (passive != null) passive.EnablePassive();
            }
        }
    }
    
    private void CastBasic() => Cast(SkillSlot.Basic);
    private void CastDefense() => Cast(SkillSlot.Defense);
    private void CastSpecial() => Cast(SkillSlot.Special);
    private void CastDash() => Cast(SkillSlot.Dash);

    private void Cast(SkillSlot slot)
    {
        if (_actives.TryGetValue(slot, out var a)) a.TryCast();

        //Debug.Log(a.Definition.DisplayName);
    }
    
    public ActiveSkillBehaviour GetActive(SkillSlot slot)
    {
        _actives.TryGetValue(slot, out var beh);
        return beh;
    }
}
