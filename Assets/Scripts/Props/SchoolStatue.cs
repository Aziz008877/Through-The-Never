using System;
using System.Collections.Generic;
using UnityEngine;
public class SchoolStatue : MonoBehaviour, IInteractable
{
    [SerializeField] private MagicSchool _school;
    [SerializeField] private SkillDefinition _starterBasic;
    [SerializeField] private InnateDefinition _starterInnate;
    [SerializeField] private SkillDefinition _starterDash;
    [SerializeField] private SkillDefinition _currentTestSpecial;
    [SerializeField] private SkillDefinition _currentTestPassive;
    [SerializeField] private SkillSelectionSaver _saver;
    [SerializeField] private ParticleSystem _chooseVFX;
    [field: SerializeField] public Transform InteractionUI { get; set; }
    [field: SerializeField] public bool CanInteract { get; set; }
    public Action<MagicSchool> OnMagicSchoolSelected;

    public void PerformAction(GameObject player)
    {
        if (!CanInteract) return;
        if (!_saver.TryChooseSchool(_school)) return;
        
        _saver.AddSkill(_starterInnate);
        _saver.AddSkill(_starterBasic);
        _saver.AddSkill(_starterDash);
        _saver.AddSkill(_currentTestSpecial);
        _saver.AddSkill(_currentTestPassive);
        
        player.GetComponent<PlayerSkillManager>().AddSkills(_saver.GetChosenSkills());

        //_chooseVFX.Play();
        CanInteract = false;
        //InteractionUI.gameObject.SetActive(false);
        //GetComponent<Collider>().enabled = false;
        OnMagicSchoolSelected?.Invoke(_school);
    }
}