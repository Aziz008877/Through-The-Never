using UnityEngine;

public class GhoulMove : BaseEnemyMove
{
    [Header("Attack Settings")]
    [SerializeField] private float _meleeDistance = 2f;
    [SerializeField] private float _rangedCooldown = 4f;
    [SerializeField] private float _meleeCooldown = 2f;
    [SerializeField] private float _stopDurationAfterAttack = 1f;
    private float _lastMeleeTime = -Mathf.Infinity;
    private float _lastRangedTime = -Mathf.Infinity;
    private bool _isAttacking = false;
    private void Update()
    {
        if (_target == null) return;
        float distance = Vector3.Distance(transform.position, _target.position);

        if (_isAttacking)
        {
            StopChasing();
            return;
        }

        if (distance <= _meleeDistance && Time.time >= _lastMeleeTime + _meleeCooldown)
        {
            _lastMeleeTime = Time.time;
            PerformMeleeAttack();
            return;
        }

        if (distance > _meleeDistance && Time.time >= _lastRangedTime + _rangedCooldown)
        {
            _lastRangedTime = Time.time;
            PerformRangedAttack();
            return;
        }
        
        ChaseTarget();
    }

    private void PerformMeleeAttack()
    {
        _isAttacking = true;
        StopChasing();
        _enemyAnimation.PlayMeleeAttack();
        // _enemyAttack.PrepareAttack(_target);

        Invoke(nameof(EndAttack), _stopDurationAfterAttack);
    }

    private void PerformRangedAttack()
    {
        _isAttacking = true;
        StopChasing();
        _enemyAnimation.PlayRangedAttack();
        // _enemyAttack.PrepareRangedAttack(_target);

        Invoke(nameof(EndAttack), _stopDurationAfterAttack);
    }

    private void EndAttack()
    {
        _isAttacking = false;
    }

    protected override void ChaseTarget()
    {
        if (_isAttacking) return;

        _agent.isStopped = false;
        _agent.SetDestination(_target.position);
        _enemyAnimation.ChangeMoveState(true);
    }

    protected override void StopChasing()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _enemyAnimation.ChangeMoveState(false);
    }
}
