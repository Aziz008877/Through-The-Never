using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerSkillManager : MonoBehaviour, ISkillManager
{
    [SerializeField] private Transform _skillRoot;
    [SerializeField] private AudioSource _skillAudioSource;
    readonly Dictionary<SkillSlot, ActiveSkillBehaviour> _actives = new();
    public IReadOnlyDictionary<SkillSlot, ActiveSkillBehaviour> Actives => _actives;
    public event Action<SkillSlot, ActiveSkillBehaviour> ActiveRegistered;

    [Inject] private PlayerContext _context;
    [Inject] private PlayerInput _input;
    [Inject] private SkillRuntimeFactory _factory;
    [Inject] private PlayerAnimator _playerAnimator;
    private readonly List<SkillDefinition> _chosenSkills = new();
    public IReadOnlyList<SkillDefinition>  ChosenSkills => _chosenSkills;
    private bool _basicLocked;
    private SkillSelectionSaver _selectionSaver;
    public static Action<SkillSlot> OnSkillPerformed;
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
    
    private void CastBasic()
    {
        if (_basicLocked) return;

        if (Cast(SkillSlot.Basic, "temp"))
        {
            Debug.Log("YES");
            _playerAnimator.CastBasics();
        }
            
    }
    private void CastDefense() => Cast(SkillSlot.Defense);
    private void CastSpecial() => Cast(SkillSlot.Special);
    private void CastDash() => Cast(SkillSlot.Dash);

    private void Cast(SkillSlot slot)
    {
        if (_actives.TryGetValue(slot, out var a) && a != null)
        {
            bool wasReady = a.IsReady;
            a.TryCast();

            if (wasReady && !a.IsReady) 
                PlaySkillSound(a);
        }

        OnSkillPerformed?.Invoke(slot);
    }
    
    private bool Cast(SkillSlot slot, string basic)
    {
        if (_actives.TryGetValue(slot, out var a) && a != null)
        {
            if (!a.IsReady) return false;
            bool wasReady = a.IsReady;
            Debug.Log(wasReady);
            Debug.Log(slot);
            a.TryCast();
            bool success = wasReady && !a.IsReady;
            Debug.Log(success);
            if (success) PlaySkillSound(a);
            return success;
        }

        return false;
    }
    private void PlaySkillSound(ActiveSkillBehaviour skill)
    {
        if (skill?.Definition == null) return;

        var clip = skill.Definition.SkillSound;

        if (clip == null) return;

        _skillAudioSource.PlayOneShot(clip);
    }
    
    public void SetBasicLocked(bool state) => _basicLocked = state;
    
    public ActiveSkillBehaviour GetActive(SkillSlot slot)
    {
        _actives.TryGetValue(slot, out var beh);
        return beh;
    }
}
