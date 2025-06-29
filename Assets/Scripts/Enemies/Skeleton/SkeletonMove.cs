using System;
using UnityEngine;

public class SkeletonMove : BaseEnemyMove
{
    private SkeletonHP _skeletonHp;
    private float _lastMeleeTime = -Mathf.Infinity;
    private bool _isAttacking = false;
    private void Awake()
    {
        _skeletonHp = GetComponent<SkeletonHP>();
        _skeletonHp.OnEnemyDead += Die;
    }

    private void Die(Transform obj)
    {
        _canChase = false;
        StopChasing();
    }

    private void Update()
    {
        if (_target == null || !_canChase) return;
        
        if (_isAttacking)
        {
            StopChasing();
            return;
        }

        float distance = Vector3.Distance(transform.position, _target.position);
        
        if (distance <= _enemyAttack.MeleeDistance && Time.time >= _lastMeleeTime + _enemyAttack.MeleeCooldown)
        {
            PerformMeleeAttack();
            return;
        }
        
        ChaseTarget();
    }

    private void PerformMeleeAttack()
    {
        _isAttacking = true;
        StopChasing();
        _lastMeleeTime = Time.time;

        _enemyAnimation.PlayMeleeAttack();

        Invoke(nameof(EndAttack), _enemyAttack.StopDurationAfterAttack);
    }

    private void EndAttack()
    {
        _isAttacking = false;
    }

    protected override void ChaseTarget()
    {
        if (_isAttacking) return;
        base.ChaseTarget();
    }

    private void OnDestroy()
    {
        _skeletonHp.OnEnemyDead -= Die;
    }
}