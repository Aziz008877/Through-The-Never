using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "Innate/Progress Saver")]
public class InnateProgressSaver : ScriptableObject
{
    [SerializeField] private List<InnateDefinition> _innateDefinitions = new();
    [SerializeField] private List<int> _currentLevels = new();
    [SerializeField] private int _storedEssence;

    private const string SaveFileName = "innate_progress.json";

    private void OnValidate()
    {
        while (_currentLevels.Count < _innateDefinitions.Count)
            _currentLevels.Add(0);
    }

    public int Essence => _storedEssence;
    public void AddEssence(int amount)
    {
        _storedEssence += amount;
        Save();
    }

    public int GetLevel(MagicSchool school)
    {
        int index = _innateDefinitions.FindIndex(innateDefinition => innateDefinition.School == school);
        return index >= 0 ? _currentLevels[index] : 0;
    }

    public bool TryUpgrade(MagicSchool school)
    {
        int index = _innateDefinitions.FindIndex(innateDefinition => innateDefinition.School == school);
        if (index < 0) return false;

        InnateDefinition def = _innateDefinitions[index];
        int level = _currentLevels[index];
        if (level >= def.levels.Length - 1) return false;

        int cost = def.levels[level + 1].cost;
        if (_storedEssence < cost) return false;

        _storedEssence -= cost;
        _currentLevels[index] = level + 1;
        Save();
        return true;
    }

    public void ResetProgress()
    {
        for (int i = 0; i < _currentLevels.Count; i++)
            _currentLevels[i] = 0;

        _storedEssence = 0;
        Save();
    }

    [System.Serializable]
    private class SaveData { public List<int> levels; public int essence; }

    private void OnEnable()
    {
        while (_currentLevels.Count < _innateDefinitions.Count)
            _currentLevels.Add(0);

        Load();
    }

    public void Save()
    {
        var data = new SaveData { levels = _currentLevels, essence = _storedEssence };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, SaveFileName), json);
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<SaveData>(json);

        _currentLevels = data.levels;
        _storedEssence = data.essence;

        while (_currentLevels.Count < _innateDefinitions.Count)
            _currentLevels.Add(0);
    }
}
