using UnityEngine;
using Zenject;
public class SkillSceneInitializer : MonoBehaviour
{
    [SerializeField] private SkillSelectionSaver _skillSelectionSaver;
    [SerializeField] private SkillActionData[] _skillActions;
    [Inject] private PlayerSkillHandler _playerSkillHandler;

    private void Start()
    {
        if (_skillSelectionSaver.SelectedSkillID != -1)
        {
            int id = _skillSelectionSaver.SelectedSkillID;
            _skillActions[id].Activate(_playerSkillHandler);
            _skillSelectionSaver.ClearSelection();
        }
    }
}