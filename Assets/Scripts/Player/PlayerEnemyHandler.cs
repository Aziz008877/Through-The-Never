using System;
using UnityEngine;

public class PlayerEnemyHandler : MonoBehaviour, IEnemyHandler
{
    [field: SerializeField] public BaseEnemyHP[] Enemies { get; set; }
    public event Action<Transform> OnEnemyKilled;

    private void Start()
    {
        foreach (var enemyHp in Enemies)
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
