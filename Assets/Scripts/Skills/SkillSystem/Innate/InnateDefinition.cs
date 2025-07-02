using UnityEngine;

[CreateAssetMenu(menuName = "Innate/Definition")]
public class InnateDefinition : SkillDefinition
{
    public LevelData[] levels;

    [System.Serializable]
    public class LevelData
    {
        public int cost;
        public float value;
        public string description;
    }
}