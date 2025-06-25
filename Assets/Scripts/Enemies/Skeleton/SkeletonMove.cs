using UnityEngine;
public class SkeletonMove : BaseEnemyMove
{
    [SerializeField] private float _attackDistance = 2f;
    [SerializeField] private float _attackCooldown = 1.5f;
    private float _lastAttackTime;
    private void Update()
    {
        if (_target == null) return;

        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance > _attackDistance)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_target.position);
        }
        else
        {
            _agent.isStopped = true;

            if (Time.time >= _lastAttackTime + _attackCooldown)
            {
                Attack();
                _lastAttackTime = Time.time;
            }
        }
    }

    private void Attack()
    {
        _enemyAttack.PrepareAttack(_target);
        _enemyAnimation.PlayMeleeAttack();
    }
}
