using System;
using UnityEngine;

public class SkeletonHandler : MonoBehaviour
{
    [SerializeField] private BaseEnemyHP[] _allSkeletons;

    private void Awake()
    {
        foreach (var skeleton in _allSkeletons)
        {
            skeleton.OnEnemyDead += ReceiveEnemyDeadState;
        }
    }

    private void ReceiveEnemyDeadState(Transform deadPosition)
    {
        throw new NotImplementedException();
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
