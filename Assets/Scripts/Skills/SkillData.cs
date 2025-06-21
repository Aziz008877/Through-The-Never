using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public Sprite SkillImage;
    public string SkillName;
    public string SkillDescription;
}
