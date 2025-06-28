using UnityEngine;

public abstract class BaseEnemyAttack : MonoBehaviour
{
    [Header("General Attack Settings")]
    [SerializeField] protected float _damage = 10f;
    [SerializeField] protected float _meleeDistance = 2f;
    [SerializeField] protected float _rangeDistance = 6f;
    [SerializeField] protected float _meleeCooldown = 2f;
    [SerializeField] protected float _rangeCooldown = 4f;
    [SerializeField] protected float _stopDurationAfterAttack = 1f;

    protected Transform _target;

    public float MeleeDistance => _meleeDistance;
    public float RangeDistance => _rangeDistance;
    public float MeleeCooldown => _meleeCooldown;
    public float RangeCooldown => _rangeCooldown;
    public float StopDurationAfterAttack => _stopDurationAfterAttack;

    public virtual void PrepareAttack(Transform target)
    {
        _target = target;
    }

    // animation event
    public virtual void TryApplyDamage()
    {
        if (_target == null) return;

        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance <= _meleeDistance)
        {
            _target.GetComponent<PlayerHP>()?.ReceiveDamage(_damage);
        }
    }
    
    public virtual void PerformRangeAttack()
    {
        Debug.Log($"{name} использует базовую дальнюю атаку (нет реализации)");
    }
}