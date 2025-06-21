using UnityEngine;

public class SkillUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] _skillsVisuals;

    private void ShowUISkill(int id)
    {
        _skillsVisuals[id].SetActive(true);
    }
}
