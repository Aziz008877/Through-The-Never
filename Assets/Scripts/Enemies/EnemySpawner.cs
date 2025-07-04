using UnityEngine;
using Zenject;
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private BaseEnemyHP _prefab;
    [Inject] private PlayerEnemyHandler _playerEnemyHandler;

    public void Spawn(Vector3 pos)
    {
        var enemy = Instantiate(_prefab, pos, Quaternion.identity);
        _playerEnemyHandler.RegisterEnemy(enemy);
    }
}
