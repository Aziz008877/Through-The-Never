using System;
using UnityEngine;
using UnityEngine.Events;

public class SkeletonHandler : MonoBehaviour
{
    [SerializeField] private BaseEnemyHP[] _allSkeletons;
    [SerializeField] private UnityEvent _onAllSkeletonsDestroyed;
    private int _killCount;
    private void Awake()
    {
        foreach (var skeleton in _allSkeletons)
        {
            skeleton.OnEnemyDead += ReceiveEnemyDeadState;
        }
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
