using UnityEngine;
public sealed class NemesisService
{
    private readonly NemesisConfig _cfg;

    private bool _active;
    private EnemyKind _kind;
    private int _baseTier;
    private int _level;

    public NemesisService(NemesisConfig cfg)
    {
        _cfg = cfg ? cfg : ScriptableObject.CreateInstance<NemesisConfig>();
        var s = NemesisStorage.Load();
        _active = s.Active;
        _kind = TryParseKind(s.Kind, out var k) ? k : EnemyKind.Melee;
        _baseTier = s.BaseTier <= 0 ? 1 : s.BaseTier;
        _level = Mathf.Clamp(s.Level, 0, _cfg.MaxLevel);
#if UNITY_EDITOR
        Debug.Log($"[Nemesis] Loaded: {s}");
#endif
    }

    private static bool TryParseKind(string v, out EnemyKind kind)
    {
        kind = EnemyKind.Melee;
        if (string.IsNullOrEmpty(v)) return false;
        return System.Enum.TryParse(v, out kind);
    }

    private void Persist()
    {
        NemesisStorage.Save(new NemesisState
        {
            Active = _active,
            Kind = _kind.ToString(),
            BaseTier = _baseTier,
            Level = _level
        });
    }

    public bool HasActive => _active;

    public void Promote(EnemyKind kind, int baseTier)
    {
        if (!_active)
        {
            _active   = true;
            _kind     = kind;
            _baseTier = Mathf.Max(1, baseTier);
            _level    = 1;
        }
        else if (_kind == kind)
        {
            _level    = Mathf.Clamp(_level + 1, 1, _cfg.MaxLevel);
            _baseTier = Mathf.Max(_baseTier, baseTier);
        }
        else
        {
            _active   = true;
            _kind     = kind;
            _baseTier = Mathf.Max(1, baseTier);
            _level    = 1;
        }
        Persist();
#if UNITY_EDITOR
        Debug.Log($"[Nemesis] Promote -> Active={_active}, Kind={_kind}, BaseTier={_baseTier}, Level={_level}");
#endif
    }

    public void ClearActive()
    {
        _active = false;
        _level  = 0;
        Persist();
#if UNITY_EDITOR
        Debug.Log("[Nemesis] Cleared");
#endif
    }

    public bool TryGetActive(out EnemyKind kind, out int baseTier, out int level)
    {
        kind = _kind; baseTier = _baseTier; level = _level;
        return _active && _level > 0;
    }
    
    public int ComputeTargetTier(int meleeMax, int rangedMax, out int overflow)
    {
        int maxTier     = (_kind == EnemyKind.Melee) ? meleeMax : rangedMax;
        int desiredTier = _baseTier + (_level - 1);
        overflow        = Mathf.Max(0, desiredTier - maxTier);
        return Mathf.Clamp(desiredTier, 1, maxTier);
    }

    public float HpResidualMult(int overflowLevels)  => 1f + Mathf.Max(0, overflowLevels) * Mathf.Max(0f, _cfg.HpPerLevel);
    public float DmgResidualMult(int overflowLevels) => 1f + Mathf.Max(0, overflowLevels) * Mathf.Max(0f, _cfg.DamagePerLevel);
    public float MsResidualMult(int overflowLevels)  => 1f + Mathf.Max(0, overflowLevels) * Mathf.Max(0f, _cfg.MoveSpeedPerLevel);
}
