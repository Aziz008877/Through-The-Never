using UnityEngine;

public sealed class LavapoolPassive : PassiveSkillBehaviour
{
    [SerializeField] private LavaPoolArea _lavaPrefab;
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private float _dps = 8f;
    [SerializeField] private float _radius = 2.2f;

    public override void EnablePassive()
    {
        PlayerContext.PlayerEnemyHandler.OnEnemyKilled += SpawnLavapool;
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerEnemyHandler.OnEnemyKilled -= SpawnLavapool;
    }

    private void SpawnLavapool(Transform deadEnemy)
    {
        var pool = Instantiate(_lavaPrefab, deadEnemy.position, Quaternion.identity);
        pool.Init(_dps, _radius, _lifeTime, PlayerContext);
    }
}