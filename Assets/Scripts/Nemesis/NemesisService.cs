using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class NemesisService
{
    private readonly NemesisConfig _cfg;
    private readonly INemesisStorage _storage;
    private readonly Dictionary<string, int> _levels; // npcId -> level
    public event Action<string, int> OnLevelChanged;   // npcId, newLevel
    public NemesisService(NemesisConfig cfg, INemesisStorage storage)
    {
        _cfg = cfg;
        _storage = storage;
        _levels = storage.LoadAll() ?? new Dictionary<string, int>(StringComparer.Ordinal);
    }

    public int GetLevel(string npcId)
    {
        return npcId != null && _levels.TryGetValue(npcId, out var lvl) ? lvl : 0;
    }

    public bool IsNemesis(string npcId) => GetLevel(npcId) > 0;

    public void Promote(string npcId)
    {
        if (string.IsNullOrEmpty(npcId)) return;
        var cur = GetLevel(npcId);
        var next = Mathf.Clamp(cur + 1, 1, _cfg.MaxLevel);
        if (next != cur)
        {
            _levels[npcId] = next;
            _storage.SaveAll(_levels);
            OnLevelChanged?.Invoke(npcId, next);
        }
    }

    public void Clear(string npcId)
    {
        if (string.IsNullOrEmpty(npcId)) return;
        if (_levels.Remove(npcId))
        {
            _storage.SaveAll(_levels);
            OnLevelChanged?.Invoke(npcId, 0);
        }
    }

    public float GetHpMultiplier(string npcId)
    {
        int lvl = GetLevel(npcId);
        return 1f + lvl * Mathf.Max(0f, _cfg.HpPerLevel);
    }

    public float GetDamageMultiplier(string npcId)
    {
        int lvl = GetLevel(npcId);
        return 1f + lvl * Mathf.Max(0f, _cfg.DamagePerLevel);
    }
}