using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyHandler : MonoBehaviour
{
    [SerializeField] private List<BaseEnemyMove> _enemies;
    [Inject] private PlayerContext _playerContext;
    private void Start()
    {
        foreach (var enemy in _enemies)
        {
            enemy.ReceiveTargetEnemy(_playerContext.PlayerPosition);
        }
    }
}
