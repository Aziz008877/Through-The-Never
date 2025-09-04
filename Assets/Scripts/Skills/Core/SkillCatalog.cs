using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill Catalog")]
public class SkillCatalog : ScriptableObject
{
    [Serializable]
    public struct SchoolBundle
    {
        public MagicSchool School;

        [Header("Starter set (granted instantly)")]
        public SkillDefinition Basic;
        public SkillDefinition StarterDash;
        public SkillDefinition Innate;

        [Header("Pools for chests")]
        public SkillDefinition[] Specials;
        public SkillDefinition[] Defenses;
        public SkillDefinition[] Dash;
        public SkillDefinition[] Passives;
    }

    [SerializeField] private SchoolBundle[] _schools;

    public bool TryGetBundle(MagicSchool school, out SchoolBundle bundle)
    {
        for (int i = 0; i < _schools.Length; i++)
        {
            if (_schools[i].School.Equals(school))
            {
                bundle = _schools[i];
                return true;
            }
        }

        bundle = default;
        return false;
    }
}