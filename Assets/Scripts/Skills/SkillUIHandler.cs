using UnityEngine;

public class SkillUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] _skillsVisuals;

    public void ShowUISkill(int id)
    {
        _skillsVisuals[id].SetActive(true);
    }
}
