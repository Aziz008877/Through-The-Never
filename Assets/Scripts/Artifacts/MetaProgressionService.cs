using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class MetaSaveData
{
    public int Coins = 0;
    public Dictionary<int, int> Levels = new();
}

public class MetaProgressionService : MonoBehaviour
{
    [Serializable]
    private class MetaSaveDTO
    {
        public int Coins;
        public List<int> Keys = new();
        public List<int> Values = new();
    }

    [SerializeField] private ArtifactProgressConfig _config;
    [SerializeField] private int _startingCoins = 0;

    public static MetaProgressionService Instance { get; private set; }
    public event Action OnChanged;

    private MetaSaveData _data;
    private string SavePath => Path.Combine(Application.persistentDataPath, "meta_progress.json");

    private void Awake()
    {
        //Time.timeScale = 4;
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public int GetLevel(ArtifactId id)
    {
        if (_data.Levels != null && _data.Levels.TryGetValue((int)id, out int lvl))
            return Mathf.Max(0, lvl);
        return 0;
    }

    public int GetMaxLevel() => _config ? _config.MaxLevel : 5;

    public int GetUpgradeCost(ArtifactId id)
    {
        int cur = GetLevel(id);
        if (cur >= GetMaxLevel()) return 0;
        return _config ? _config.GetUpgradeCost(cur) : 0;
    }

    public bool CanUpgrade(ArtifactId id)
    {
        int cur = GetLevel(id);
        if (cur >= GetMaxLevel()) return false;
        int cost = GetUpgradeCost(id);
        return _data.Coins >= cost;
    }

    public bool Upgrade(ArtifactId id)
    {
        if (!CanUpgrade(id)) return false;
        int cur  = GetLevel(id);
        int cost = GetUpgradeCost(id);
        _data.Coins -= cost;
        _data.Levels[(int)id] = cur + 1;
        Save();
        OnChanged?.Invoke();
        return true;
    }

    public void AddCoins(int delta)
    {
        _data.Coins = Mathf.Max(0, _data.Coins + delta);
        Save();
        OnChanged?.Invoke();
    }

    public int Coins => _data.Coins;

    public bool IsSchoolUnlocked(MagicSchool school)
    {
        return school switch
        {
            MagicSchool.Fire => GetLevel(ArtifactId.PhoenixFeather) > 0,
            MagicSchool.Ice  => GetLevel(ArtifactId.MoonShard)      > 0,
            _ => false
        };
    }

    private void Load()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                var json = File.ReadAllText(SavePath);
                var dto  = JsonUtility.FromJson<MetaSaveDTO>(json);

                _data = new MetaSaveData { Coins = dto.Coins, Levels = new Dictionary<int, int>() };
                for (int i = 0; i < dto.Keys.Count && i < dto.Values.Count; i++)
                    _data.Levels[dto.Keys[i]] = dto.Values[i];
            }
            else
            {
                _data = new MetaSaveData { Coins = _startingCoins, Levels = new Dictionary<int, int>() };
                Save(); // создадим файл сразу
            }
        }
        catch
        {
            _data = new MetaSaveData { Coins = _startingCoins, Levels = new Dictionary<int, int>() };
        }
    }

    private void Save()
    {
        try
        {
            var dto = new MetaSaveDTO { Coins = _data.Coins };
            if (_data.Levels != null)
            {
                foreach (var kv in _data.Levels)
                {
                    dto.Keys.Add(kv.Key);
                    dto.Values.Add(kv.Value);
                }
            }

            var json = JsonUtility.ToJson(dto);
            File.WriteAllText(SavePath, json);
        }
        catch { /* ignore */ }
    }

    public float PhoenixDotPercent    => _config ? _config.GetPhoenixDotPercent(GetLevel(ArtifactId.PhoenixFeather)) : 0.25f;
    public float PhoenixDotDuration   => _config ? _config.GetPhoenixDotDuration(GetLevel(ArtifactId.PhoenixFeather)) : 3f;

    public float MoonSlowMovePerStack => _config ? _config.GetMoonSlowMovePerStack(GetLevel(ArtifactId.MoonShard)) : 0.08f;
    public float MoonDmgRedPerStack   => _config ? _config.GetMoonDmgRedPerStack(GetLevel(ArtifactId.MoonShard))   : 0.06f;
    public int   MoonMaxStacks        => _config ? _config.GetMoonMaxStacks(GetLevel(ArtifactId.MoonShard))        : 5;
    public float MoonAttackSpeedSlowPerStack => _config ? _config.GetMoonAttackSpeedSlowPerStack(GetLevel(ArtifactId.MoonShard)) : 0f;
    public float HornMaxHPBonus => _config ? _config.GetHornMaxHPBonus(GetLevel(ArtifactId.CernunnosHorn)) : 0f;

}
