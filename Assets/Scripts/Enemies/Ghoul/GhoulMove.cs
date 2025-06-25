using UnityEngine;

public class GhoulMove : BaseEnemyMove
{
    [Header("Attack Settings")]
    [SerializeField] private float _meleeDistance = 2f;
    [SerializeField] private float _rangedDistance = 6f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _rangedAttackChance = 0.5f;
    private float _lastAttackTime;
    private void Update()
    {
        if (_target == null) return;

        float distance = Vector3.Distance(transform.position, _target.position);
        
        if (distance > _rangedDistance)
        {
            ChaseTarget();
            return;
        }

        StopChasing();

        if (Time.time < _lastAttackTime + _attackCooldown)
            return;
        
        if (distance <= _meleeDistance)
        {
            PerformMeleeAttack();
        }
        else
        {
            if (Random.value < _rangedAttackChance)
                PerformRangedAttack();
            else
                PerformMeleeAttack();
        }

        _lastAttackTime = Time.time;
    }

    private void PerformMeleeAttack()
    {
        _enemyAnimation.PlayMeleeAttack();
        //_enemyAttack.PrepareAttack(_target);
    }

    private void PerformRangedAttack()
    {
        _enemyAnimation.PlayRangedAttack();
        //_enemyAttack.PrepareRangedAttack(_target); // или FireballAttack
    }
}