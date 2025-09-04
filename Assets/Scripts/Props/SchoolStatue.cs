using System;
using System.Collections.Generic;
using UnityEngine;

public class SchoolStatue : MonoBehaviour, IInteractable
{
    [SerializeField] private MagicSchool _school;
    [SerializeField] private SkillDefinition _starterBasic;
    [SerializeField] private InnateDefinition _starterInnate;
    [SerializeField] private SkillDefinition _starterDash;
    [SerializeField] private SkillSelectionSaver _saver;
    [SerializeField] private ChestOfferDirector _chestOfferDirector;
    [SerializeField] private ParticleSystem _chooseVFX;

    [field: SerializeField] public Transform InteractionUI { get; set; }
    [field: SerializeField] public bool CanInteract { get; set; } = true;

    public Action<MagicSchool> OnMagicSchoolSelected;

    public void PerformAction(GameObject player)
    {
        if (!CanInteract) return;
        if (!_saver.TryChooseSchool(_school)) return;

        var starters = new List<SkillDefinition>(3);
        if (_starterInnate) starters.Add(_starterInnate);
        if (_starterBasic)  starters.Add(_starterBasic);
        if (_starterDash)   starters.Add(_starterDash);

        if (_starterInnate) _saver.AddSkill(_starterInnate);
        if (_starterBasic)  _saver.AddSkill(_starterBasic);
        if (_starterDash)   _saver.AddSkill(_starterDash);
        
        var psm = player.GetComponent<PlayerSkillManager>();
        psm.AddSkills(starters);
        
        _chestOfferDirector.SetSchool(_school);
        _chestOfferDirector.InitializeAndGrantInitial();

        if (_chooseVFX) _chooseVFX.Play();
        CanInteract = false;
        OnMagicSchoolSelected?.Invoke(_school);
    }
}