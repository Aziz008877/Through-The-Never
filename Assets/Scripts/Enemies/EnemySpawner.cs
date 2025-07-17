using UnityEngine;
using Zenject;
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private BaseEnemyHP _prefab;
    [Inject] private PlayerEnemyHandler _playerEnemyHandler;
    [Inject] private PlayerContext _playerContext;
    public void Spawn(Vector3 pos)
    {
        var enemy = Instantiate(_prefab, pos, Quaternion.identity);
        enemy.GetComponent<BaseEnemyMove>().ReceiveTargetEnemy(_playerContext.PlayerPosition);
        _playerEnemyHandler.RegisterEnemy(enemy);
    }
}
