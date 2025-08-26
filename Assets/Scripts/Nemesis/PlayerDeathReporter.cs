using UnityEngine;

public static class PlayerDeathReporter
{
    public static void ReportPlayerKilledBy(Transform killer)
    {
        if (!killer) return;
        var target = killer.GetComponent<NemesisTarget>();
        if (target == null) return;
        NemesisRuntime.Svc?.Promote(target.NpcId);
    }
}

public static class EnemyDeathReporter
{
    public static void ReportEnemyDied(Transform enemyRoot)
    {
        if (!enemyRoot) return;
        var nem = enemyRoot.GetComponent<NemesisTarget>();
        if (nem != null)
            nem.OnDied();
    }
}
