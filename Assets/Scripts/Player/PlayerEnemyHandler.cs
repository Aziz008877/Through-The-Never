using System;
using UnityEngine;

public class PlayerEnemyHandler : MonoBehaviour, IEnemyHandler
{
    [SerializeField] private BaseEnemyHP[] _baseEnemyHp;
    public event Action<Transform> OnEnemyKilled;

    private void Start()
    {
        foreach (var enemyHp in _baseEnemyHp)
        {
            RegisterEnemy(enemyHp);
        }
    }

    public void RegisterEnemy(IDamageable dmg)
    {
        dmg.OnEnemyDead += HandleEnemyDead;
    }

    private void UnregisterEnemy(IDamageable dmg)
    {
        dmg.OnEnemyDead -= HandleEnemyDead;
    }

    private void HandleEnemyDead(Transform enemy)
    {
        OnEnemyKilled?.Invoke(enemy);
    }
}
