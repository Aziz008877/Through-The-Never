using UnityEngine;

public static class PlayerDeathReporter
{
    public static void ReportPlayerKilledBy(Transform killer)
    {
        if (!killer) return;

        EnemyKind kind = EnemyKind.Melee;
        int baseTier = 1;

        var melee  = killer.GetComponentInParent<MeleeMobAttack>();
        var ranged = killer.GetComponentInParent<RangedMobAttack>();

        if (melee)  { kind = EnemyKind.Melee;  baseTier = melee.GetTierOrDefault(1); }
        if (ranged) { kind = EnemyKind.Ranged; baseTier = ranged.GetTierOrDefault(1); }

        var svc = NemesisRuntime.Svc;
        if (svc == null)
        {
            Debug.LogWarning("[Nemesis] Svc is null. NemesisRuntime present?");
            return;
        }

        svc.Promote(kind, baseTier);
#if UNITY_EDITOR
        Debug.Log($"[Nemesis] Promote by killer: kind={kind}, baseTier={baseTier}");
#endif
    }
}

public static class EnemyDeathReporter
{
    public static void ReportEnemyDied(Transform enemyRoot)
    {
        
    }
}
