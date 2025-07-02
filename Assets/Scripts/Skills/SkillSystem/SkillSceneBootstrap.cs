using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class SkillSceneBootstrap : MonoBehaviour
{
    [SerializeField] private SkillSelectionSaver _saver;
    [Inject] private PlayerSkillManager _skillManager;
    [Inject] private PlayerContext _context;
    private void Start()
    {
        List<SkillDefinition> list = _saver.GetChosenSkills();
        _skillManager.Build(list);
        _saver.Clear();
    }

    private void OnDestroy()
    {
        _saver.Clear();
    }
}