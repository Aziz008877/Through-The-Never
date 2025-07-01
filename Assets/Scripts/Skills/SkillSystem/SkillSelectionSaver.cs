using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Selection Saver")]
public class SkillSelectionSaver : ScriptableObject
{
    [SerializeField] private List<SkillDefinition> _chosen = new();
    [SerializeField] private bool _hasSchool;
    [SerializeField] private MagicSchool _school;
    public bool HasHasSchool => _hasSchool;
    public MagicSchool School => _school;
    public void AddSkill(SkillDefinition def) => _chosen.Add(def);
    public List<SkillDefinition> GetChosenSkills() => new(_chosen);
    public bool TryChooseSchool(MagicSchool school)
    {
        if (_hasSchool) return false;
        _hasSchool = true;
        _school = school;
        return true;
    }

    public void Clear()
    {
        _chosen.Clear();
        _hasSchool = false;
    }
}