using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill Pool")]
public class SkillPoolSO : ScriptableObject
{
    [SerializeField] private List<SkillDefinition> _allDefinitions = new();

    public List<SkillDefinition> GetBySlot(SkillSlot slot)
    {
        return _allDefinitions.FindAll(d => d.Slot == slot);
    }
}