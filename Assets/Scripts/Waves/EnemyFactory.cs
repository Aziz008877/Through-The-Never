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
    [SerializeField] private ParticleSystem _spawnCircle;

    public void SetTarget(Transform target) => _playerTarget = target;

    public BaseEnemyHP Spawn(EnemyKind kind, int tier, Vector3 pos, Quaternion rot)
    {
        if (!_catalog.TryGet(kind, out var prefab))
        {
            Debug.LogError($"[EnemyFactory] Prefab not found in catalog for kind={kind}");
            return null;
        }

        var hp = Instantiate(prefab, pos, rot);
        
        if (_spawnCircle)
        {
            _spawnCircle.Stop();
            _spawnCircle.transform.position = new Vector3(pos.x, 0.05f, pos.z);
            _spawnCircle.Play();
        }
        
        if (_playerTarget)
        {
            var move = hp.GetComponent<BaseEnemyMove>();
            if (move)
            {
                if (move.TryGetComponent(out MeleeMobBrain meleeMobBrain))
                    meleeMobBrain.ReceivePlayer(_playerTarget);
                if (move.TryGetComponent(out RangedMobBrain rangedMobBrain))
                    rangedMobBrain.ReceivePlayer(_playerTarget);

                move.ReceiveTargetEnemy(_playerTarget);
            }
        }
        
        bool hasMelee  = hp.TryGetComponent(out MeleeMobAttack melee);
        bool hasRanged = hp.TryGetComponent(out RangedMobAttack ranged);

        if (kind == EnemyKind.Ranged && !hasRanged)
            Debug.LogError($"[EnemyFactory] Catalog mismatch: kind=Ranged, prefab '{hp.name}' doesn't have RangedMobAttack.");

        if (kind == EnemyKind.Melee && !hasMelee)
            Debug.LogError($"[EnemyFactory] Catalog mismatch: kind=Melee, prefab '{hp.name}' doesn't have MeleeMobAttack.");

        if (hasMelee)
        {
            int tHuman = Mathf.Max(1, tier);
            int tEnum  = Mathf.Clamp(tHuman - 1, 0, 3);
            Debug.Log($"[Factory] kind={kind} requestedTier(int)={tier} prefab='{hp.name}'");
            melee.SetTier((MeleeMobTier)tEnum);
            if (hp.TryGetComponent(out MeleeMobMove move)) move.RecalculateBaseSpeed();
        }
        else if (hasRanged)
        {
            int tHuman = Mathf.Max(1, tier);
            int tEnum  = Mathf.Clamp(tHuman - 1, 0, 2);
            Debug.Log($"[Factory] kind={kind} requestedTier(int)={tier} prefab='{hp.name}'");
            ranged.SetTier((RangedMobTier)tEnum);
        }

        if (hp.TryGetComponent(out EnemyMaterialApplier vis)) vis.Refresh();

        return hp;
    }
}
