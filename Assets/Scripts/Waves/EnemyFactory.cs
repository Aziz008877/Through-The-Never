using UnityEngine;

public interface IEnemyFactory
{
    BaseEnemyHP Spawn(EnemyKind kind, int tier, Vector3 pos, Quaternion rot);
    void SetTarget(Transform target);
}

public sealed class EnemyFactory : MonoBehaviour, IEnemyFactory
{
    [SerializeField] private EnemyCatalog _catalog;
    [SerializeField] private Transform _playerTarget;

    public void SetTarget(Transform target) => _playerTarget = target;

    public BaseEnemyHP Spawn(EnemyKind kind, int tier, Vector3 pos, Quaternion rot)
    {
        if (!_catalog.TryGet(kind, out var prefab))
        {
            Debug.LogError($"[EnemyFactory] Prefab not found in catalog for kind={kind}");
            return null;
        }

        var hp = Instantiate(prefab, pos, rot);

        bool hasMelee  = hp.TryGetComponent(out MeleeMobAttack melee);
        bool hasRanged = hp.TryGetComponent(out RangedMobAttack ranged);
        
        if (kind == EnemyKind.Ranged && !hasRanged)
            Debug.LogError($"[EnemyFactory] Catalog mismatch: kind=Ranged, prefab '{hp.name}' doesn't have RangedMobAttack.");
        if (kind == EnemyKind.Melee && !hasMelee)
            Debug.LogError($"[EnemyFactory] Catalog mismatch: kind=Melee, prefab '{hp.name}' doesn't have MeleeMobAttack.");
        
        if (_playerTarget)
        {
            var move = hp.GetComponent<BaseEnemyMove>();

            if (move.TryGetComponent(out MeleeMobBrain meleeMobBrain))
            {
                meleeMobBrain.ReceivePlayer(_playerTarget);
            }

            if (move.TryGetComponent(out RangedMobBrain rangedMobBrain))
            {
                rangedMobBrain.ReceivePlayer(_playerTarget);
            }
            move.ReceiveTargetEnemy(_playerTarget);
        }

        if (hasMelee)
        {
            int t = tier <= 0 ? 1 : tier;
            //melee.SetTier((MeleeMobTier)Mathf.Clamp(t, 1, 4));
            melee.SetTier(MeleeMobTier.Tier4_Red);
            Debug.Log($"[EnemyFactory] Melee '{hp.name}' tier={t}");
            if (hp.TryGetComponent(out MeleeMobMove move)) move.RecalculateBaseSpeed();
        }
        else if (hasRanged)
        {
            int t = tier <= 0 ? 1 : tier;
            //ranged.SetTier((RangedMobTier)Mathf.Clamp(t, 1, 3));
            ranged.SetTier(RangedMobTier.Tier3_Purple);
            Debug.Log($"[EnemyFactory] Ranged '{hp.name}' tier={t}");
        }

        if (hp.TryGetComponent(out EnemyMaterialApplier vis)) vis.Refresh();
        return hp;
    }
}