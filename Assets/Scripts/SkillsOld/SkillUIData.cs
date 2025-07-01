using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Objects/SkillUIData")]
public class SkillUIData : ScriptableObject
{
    public Sprite SkillImage;
    public string SkillName;
    public string SkillDescription;
}