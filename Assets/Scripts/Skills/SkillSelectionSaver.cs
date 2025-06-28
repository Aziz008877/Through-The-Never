using UnityEngine;

[CreateAssetMenu(fileName = "SkillSelectionSaver", menuName = "SkillSystem/SkillSelectionSaver")]
public class SkillSelectionSaver : ScriptableObject
{
    public int SelectedSkillID = -1;

    public void SaveSelection(int id)
    {
        SelectedSkillID = id;
    }

    public void ClearSelection()
    {
        SelectedSkillID = -1;
    }
}