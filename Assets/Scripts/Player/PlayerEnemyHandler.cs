using System;
using UnityEngine;

public class PlayerEnemyHandler : MonoBehaviour
{
    [SerializeField] private TrainingEnemy _baseEnemyHp;
    public event Action<Transform> OnEnemyKilled;

    private void Start()
    {
        RegisterEnemy(_baseEnemyHp);
    }

    public void RegisterEnemy(IDamageable dmg)
    {
        dmg.OnEnemyDead += HandleEnemyDead;
        //UnregisterEnemy(dmg);
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
