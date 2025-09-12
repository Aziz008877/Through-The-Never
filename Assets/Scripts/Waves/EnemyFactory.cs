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

        if (hasMelee)
        {
            int tHuman = Mathf.Clamp(tier, 1, 4);
            melee.SetTier((MeleeMobTier)tHuman);
            if (hp.TryGetComponent(out MeleeMobMove move)) move.RecalculateBaseSpeed();
        }
        else if (hasRanged)
        {
            int tHuman = Mathf.Clamp(tier, 1, 3);
            ranged.SetTier((RangedMobTier)tHuman);
        }

        if (hp.TryGetComponent(out EnemyMaterialApplier vis)) vis.Refresh();

        return hp;
    }
}
