using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct OfferProgress
{
    public bool Initialized;
    public int StageIndex;
    public int CycleIndex;
    public MagicSchool School;
}

[CreateAssetMenu(menuName = "Skills/Selection Saver")]
public class SkillSelectionSaver : ScriptableObject
{
    [Header("Chosen (полученные игроком)")]
    [SerializeField] private List<SkillDefinition> _chosen = new();
    public IReadOnlyList<SkillDefinition> Chosen => _chosen;

    [Header("Taken (все, что уже предлагалось/взято)")]
    [SerializeField] private List<SkillDefinition> _takenList = new();
    public IReadOnlyList<SkillDefinition> TakenList => _takenList;

    [Header("School")]
    [SerializeField] private bool _hasSchool;
    [SerializeField] private MagicSchool _school;
    public bool HasSchool => _hasSchool;
    public MagicSchool School => _school;

    [Header("Chest offer progress")]
    [SerializeField] private OfferProgress _progress;
    public OfferProgress Progress
    {
        get => _progress;
        set => _progress = value;
    }

    [Header("Companion")]
    [SerializeField] private bool _companionEnabled;        // надо ли включать компаньона в сценах
    public bool CompanionEnabled
    {
        get => _companionEnabled;
        set => _companionEnabled = value;
    }

    // --- API ---

    public bool TryChooseSchool(MagicSchool school)
    {
        if (_hasSchool) return false;
        _hasSchool = true;
        _school = school;
        return true;
    }

    public void AddSkill(SkillDefinition def)
    {
        if (!def) return;
        if (!_chosen.Contains(def)) _chosen.Add(def);
        if (!_takenList.Contains(def)) _takenList.Add(def);
    }

    public void MarkTakenOnly(SkillDefinition def)
    {
        if (!def) return;
        if (!_takenList.Contains(def)) _takenList.Add(def);
    }

    public void Clear()
    {
        _chosen.Clear();
        _takenList.Clear();
        _hasSchool = false;
        _school = default;
        _progress = default;
        _companionEnabled = false;
    }

    public void ResetRun(MagicSchool school)
    {
        _chosen.Clear();
        _takenList.Clear();
        _progress = new OfferProgress
        {
            Initialized = false,
            StageIndex = 0,
            CycleIndex = 0,
            School = school
        };
        _hasSchool = true;
        _school = school;
        _companionEnabled = false;
    }

    public List<SkillDefinition> GetChosenSkills() => new(_chosen);
}
