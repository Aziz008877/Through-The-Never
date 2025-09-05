using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct OfferProgress
{
    public bool Initialized;
    public int StageIndex;
    public int CycleIndex;
    public MagicSchool School;
}

[CreateAssetMenu(menuName = "Skills/Selection Saver")]
public class SkillSelectionSaver : ScriptableObject
{
    [Header("Chosen (полученные игроком)")]
    [SerializeField] private List<SkillDefinition> _chosen = new();
    public IReadOnlyList<SkillDefinition> Chosen => _chosen;

    [Header("Taken (все, что уже предлагалось/взято)")]
    [SerializeField] private List<SkillDefinition> _takenList = new();
    public IReadOnlyList<SkillDefinition> TakenList => _takenList;

    [Header("School")]
    [SerializeField] private bool _hasSchool;
    [SerializeField] private MagicSchool _school;
    public bool HasSchool => _hasSchool;
    public MagicSchool School => _school;

    [Header("Chest offer progress")]
    [SerializeField] private OfferProgress _progress;
    public OfferProgress Progress
    {
        get => _progress;
        set => _progress = value;
    }

    [Header("Companion")]
    [SerializeField] private bool _companionEnabled;
    public bool CompanionEnabled
    {
        get => _companionEnabled;
        set => _companionEnabled = value;
    }

    // --- API ---

    public bool TryChooseSchool(MagicSchool school)
    {
        if (_hasSchool) return false;
        _hasSchool = true;
        _school = school;
        return true;
    }

    public void AddSkill(SkillDefinition def)
    {
        if (!def) return;
        if (!_chosen.Contains(def)) _chosen.Add(def);
        if (!_takenList.Contains(def)) _takenList.Add(def);
    }

    public void MarkTakenOnly(SkillDefinition def)
    {
        if (!def) return;
        if (!_takenList.Contains(def)) _takenList.Add(def);
    }

    public void Clear()
    {
        _chosen.Clear();
        _takenList.Clear();
        _hasSchool = false;
        _school = MagicSchool.Neutral;
        _progress = default;
        _companionEnabled = false;
    }

    public void ResetRun(MagicSchool school)
    {
        _chosen.Clear();
        _takenList.Clear();
        _progress = new OfferProgress
        {
            Initialized = false,
            StageIndex = 0,
            CycleIndex = 0,
            School = school
        };
        _hasSchool = true;
        _school = school;
        _companionEnabled = false;
    }

    public List<SkillDefinition> GetChosenSkills() => new(_chosen);

    // ---------- ГАРАНТИРОВАННЫЙ СБРОС ПРИ ЗАПУСКЕ ----------
    // Срабатывает и в билде, и в редакторе при входе в Play.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoClearOnBoot()
    {
        // Найдём все экземпляры ассета, которые попали в память,
        // и подчистим ТОЛЬКО runtime-состояние (диск не трогаем).
        var all = Resources.FindObjectsOfTypeAll<SkillSelectionSaver>();
        foreach (var saver in all)
        {
            saver.Clear();
        }
    }

#if UNITY_EDITOR
    // ---------- УТИЛИТЫ ДЛЯ EDITOR, чтобы не утащить мусор в билд ----------

    // Показывает предупреждение, если asset содержит данные.
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if ((_chosen != null && _chosen.Count > 0) ||
                (_takenList != null && _takenList.Count > 0) ||
                _hasSchool ||
                _companionEnabled ||
                _progress.Initialized ||
                _progress.StageIndex != 0 ||
                _progress.CycleIndex != 0 ||
                _progress.School != MagicSchool.Neutral)
            {
                Debug.LogWarning($"[SkillSelectionSaver] Asset \"{name}\" содержит runtime-данные. " +
                                 $"Рекомендуется очистить через контекстное меню: Skills/Selection Saver -> Clear (Editor).", this);
            }
        }
    }

    [ContextMenu("Clear (Editor)")]
    private void EditorClear()
    {
        Clear();
        UnityEditor.EditorUtility.SetDirty(this); // сохраняем чистое состояние в asset
    }
#endif
}
