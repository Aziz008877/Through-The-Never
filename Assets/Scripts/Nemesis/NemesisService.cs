using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class NemesisService
{
    private readonly NemesisConfig _cfg;
    private readonly Dictionary<string, int> _levels = new(StringComparer.Ordinal);
    public string LastKillerId { get; private set; }
    public event Action<string,int> OnLevelChanged;

    public NemesisService(NemesisConfig cfg)
    {
        _cfg = cfg;
        var s = NemesisStorage.Load();
        for (int i = 0; i < s.Ids.Count && i < s.Levels.Count; i++)
            _levels[s.Ids[i]] = Mathf.Max(0, s.Levels[i]);
        LastKillerId = s.LastKillerId;
    }

    private void Persist()
    {
        var s = new NemesisState();
        foreach (var kv in _levels)
        {
            s.Ids.Add(kv.Key);
            s.Levels.Add(kv.Value);
        }
        s.LastKillerId = LastKillerId;
        NemesisStorage.Save(s);
    }

    public int GetLevel(string npcId) => (npcId != null && _levels.TryGetValue(npcId, out var lvl)) ? lvl : 0;
    public bool IsNemesis(string npcId) => GetLevel(npcId) > 0;

    public void Promote(string npcId)
    {
        if (string.IsNullOrEmpty(npcId)) return;
        int cur = GetLevel(npcId);
        int next = Mathf.Clamp(cur + 1, 1, _cfg.MaxLevel);
        if (next != cur)
        {
            _levels[npcId] = next;
            LastKillerId = npcId;
            Persist();
            OnLevelChanged?.Invoke(npcId, next);
        }
        else
        {
            LastKillerId = npcId;
            Persist();
        }
    }

    public void Clear(string npcId)
    {
        if (string.IsNullOrEmpty(npcId)) return;
        if (_levels.Remove(npcId))
        {
            Persist();
            OnLevelChanged?.Invoke(npcId, 0);
        }
    }

    public float HpMul(string npcId)    => 1f + GetLevel(npcId) * Mathf.Max(0f, _cfg.HpPerLevel);
    public float DamageMul(string npcId)=> 1f + GetLevel(npcId) * Mathf.Max(0f, _cfg.DamagePerLevel);
}
