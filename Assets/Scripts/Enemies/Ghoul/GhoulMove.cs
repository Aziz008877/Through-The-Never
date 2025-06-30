using UnityEngine;

public class GhoulMove : BaseEnemyMove
{
    private GhoulAttackHandler _ghoulAttackHandler;

    private float _lastMeleeTime = -Mathf.Infinity;
    private float _lastRangedTime = -Mathf.Infinity;
    private float _lastSummonTime = -Mathf.Infinity;

    private bool _isAttacking = false;

    protected void Awake()
    {
        _ghoulAttackHandler = GetComponent<GhoulAttackHandler>();
    }

    private void Update()
    {
        if (_target == null) return;

        if (_isAttacking)
        {
            StopChasing();
            return;
        }

        float distance = Vector3.Distance(transform.position, _target.position);

        // Призыв скелетов по кулдауну
        if (Time.time >= _lastSummonTime + _ghoulAttackHandler.SummonCooldown)
        {
            PerformSummonSkeletons();
            return;
        }

        // Мили атака
        if (distance <= _ghoulAttackHandler.MeleeDistance && Time.time >= _lastMeleeTime + _ghoulAttackHandler.MeleeCooldown)
        {
            PerformMeleeAttack();
            return;
        }

        // Рендж атака
        if (distance > _ghoulAttackHandler.MeleeDistance && Time.time >= _lastRangedTime + _ghoulAttackHandler.RangeCooldown)
        {
            PerformRangedAttack();
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

        Invoke(nameof(EndAttack), _ghoulAttackHandler.StopDurationAfterAttack);
    }

    private void PerformRangedAttack()
    {
        _isAttacking = true;
        StopChasing();
        _lastRangedTime = Time.time;

        _enemyAnimation.PlayRangedAttack();

        Invoke(nameof(EndAttack), _ghoulAttackHandler.StopDurationAfterAttack);
    }

    private void PerformSummonSkeletons()
    {
        _isAttacking = true;
        StopChasing();
        _lastSummonTime = Time.time;
        _enemyAnimation.PlaySummonedAttack();
        _ghoulAttackHandler.SpawnSkeletons();

        Invoke(nameof(EndAttack), _ghoulAttackHandler.StopDurationAfterSummon);
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
