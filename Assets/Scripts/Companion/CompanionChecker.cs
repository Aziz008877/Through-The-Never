using System;
using UnityEngine;

public class CompanionChecker : MonoBehaviour
{
    [SerializeField] private SkillSelectionSaver _skillSelectionSaver;
    [SerializeField] private GameObject _companion;
    private void Start()
    {
        if (_skillSelectionSaver.CompanionEnabled)
        {
            _companion.SetActive(true);
        }
    }
}
