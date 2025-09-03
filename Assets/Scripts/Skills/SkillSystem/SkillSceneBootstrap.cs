using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SkillSceneBootstrap : MonoBehaviour
{
    [SerializeField] private SkillSelectionSaver _skillSelectionSaver;
    [Inject] private PlayerSkillManager _skillManager;

    private void Start()
    {
        List<SkillDefinition> skillDefinitions = _skillSelectionSaver.GetChosenSkills();
        _skillManager.Build(skillDefinitions);
        //_skillSelectionSaver.Clear();
    }

    private void OnDestroy()
    {
        //_skillSelectionSaver.Clear();
    }
}