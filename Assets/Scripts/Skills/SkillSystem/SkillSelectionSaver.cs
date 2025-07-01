using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Selection Saver")]
public class SkillSelectionSaver : ScriptableObject
{
    [SerializeField] private List<SkillDefinition> _chosen = new();

    public void Add(SkillDefinition definition) => _chosen.Add(definition);
    public List<SkillDefinition> GetChosenSkills() => new(_chosen);
    public void Clear() => _chosen.Clear();
}