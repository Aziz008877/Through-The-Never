using UnityEngine;
using UnityEngine.Events;

public class SkeletonHandler : MonoBehaviour
{
    [SerializeField] private BaseEnemyHP[] _allSkeletons;
    [SerializeField] private GhoulMove _ghoulMove;
    [SerializeField] private Transform _player;
    [SerializeField] private UnityEvent _onAllSkeletonsDestroyed;
    private int _killCount;
    private void Awake()
    {
        foreach (var skeleton in _allSkeletons)
        {
            skeleton.OnEnemyDead += ReceiveEnemyDeadState;
            skeleton.GetComponent<SkeletonAttack>().ReceiveTargetEnemy(_player);
            skeleton.GetComponent<SkeletonMove>().ReceiveTargetEnemy(_player);
        }
        
        _ghoulMove.ReceiveTargetEnemy(_player);
        _ghoulMove.GetComponent<GhoulAttackHandler>().ReceiveTargetEnemy(_player);
    }

    private void ReceiveEnemyDeadState(Transform deadPosition)
    {
        _killCount++;

        if (_killCount >= _allSkeletons.Length)
        {
            _onAllSkeletonsDestroyed?.Invoke();
        }
    }

    private void OnDestroy()
    {
        foreach (var skeleton in _allSkeletons)
        {
            if (skeleton != null)
            {
                skeleton.OnEnemyDead -= ReceiveEnemyDeadState;
            }
        }
    }
}
