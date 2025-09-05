using UnityEngine;

[CreateAssetMenu(menuName = "Meta/Artifact Progress Config")]
public class ArtifactProgressConfig : ScriptableObject
{
    [Header("Common")]
    [SerializeField] private int _maxLevel = 5;
    [SerializeField] private int[] _upgradeCosts; 
    public int MaxLevel => _maxLevel;
    public int GetUpgradeCost(int level) 
    {
        if (_upgradeCosts == null || _upgradeCosts.Length == 0) return 0;
        int idx = Mathf.Clamp(level, 0, _upgradeCosts.Length - 1);
        return _upgradeCosts[idx];
    }

    [Header("Phoenix Feather")]
    [Tooltip("Абсолютные значения по уровням: index=0 — база до апгрейдов.")]
    [SerializeField] private float[] _phoenix_DotPercentByLevel = { 0.25f, 0.30f, 0.35f, 0.40f, 0.45f, 0.50f };
    [SerializeField] private float[] _phoenix_DotDurationByLevel = { 3f, 3.5f, 4f, 4.5f, 5f, 5.5f };
    [Header("Cernunnos Horn")]
    [SerializeField] private float[] _horn_MaxHPBonusByLevel = { 0f, 25f, 50f, 80f, 120f, 170f };

    [Header("Moon Shard")]
    [SerializeField] private float[] _moon_SlowMovePerStackByLevel = { 0.08f, 0.09f, 0.10f, 0.11f, 0.12f, 0.13f };
    [SerializeField] private float[] _moon_DmgRedPerStackByLevel  = { 0.06f, 0.07f, 0.08f, 0.09f, 0.10f, 0.11f };
    [SerializeField] private int[]   _moon_MaxStacksByLevel        = { 5, 6, 7, 8, 9, 10 };
    [Tooltip("Опционально: если где-то применяешь замедление атаки врагам.")]
    [SerializeField] private float[] _moon_AttackSpeedSlowPerStackByLevel = { 0f, 0.02f, 0.04f, 0.06f, 0.08f, 0.10f };
    
    public float GetPhoenixDotPercent(int level)   => GetByLevel(_phoenix_DotPercentByLevel, level);
    public float GetPhoenixDotDuration(int level)  => GetByLevel(_phoenix_DotDurationByLevel, level);
    public float GetHornMaxHPBonus(int level) => GetByLevel(_horn_MaxHPBonusByLevel, level);
    public float GetMoonSlowMovePerStack(int level)   => GetByLevel(_moon_SlowMovePerStackByLevel, level);
    public float GetMoonDmgRedPerStack(int level)     => GetByLevel(_moon_DmgRedPerStackByLevel,  level);
    public int   GetMoonMaxStacks(int level)          => (int)Mathf.Round(GetByLevelInt(_moon_MaxStacksByLevel, level));
    public float GetMoonAttackSpeedSlowPerStack(int level)=> GetByLevel(_moon_AttackSpeedSlowPerStackByLevel, level);

    private static float GetByLevel(float[] arr, int level)
    {
        if (arr == null || arr.Length == 0) return 0f;
        int idx = Mathf.Clamp(level, 0, arr.Length - 1);
        return arr[idx];
    }
    private static int GetByLevelInt(int[] arr, int level)
    {
        if (arr == null || arr.Length == 0) return 0;
        int idx = Mathf.Clamp(level, 0, arr.Length - 1);
        return arr[idx];
    }
}
public enum ArtifactId
{
    PhoenixFeather = 0,
    MoonShard = 1,
    CernunnosHorn  = 2
}