using UnityEngine;

public class SkeletonMove : BaseEnemyMove
{
    private float _lastMeleeTime = -Mathf.Infinity;
    private bool _isAttacking = false;
    private void Update()
    {
        if (_target == null) return;
        
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
}