using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemyMove : MonoBehaviour
{
    [SerializeField] protected float _rotationSpeed;
    protected NavMeshAgent _agent;
    protected BaseEnemyAnimation _enemyAnimation;
    protected BaseEnemyAttack _enemyAttack;
    protected Transform _target;
    protected bool _canChase = true;
    public virtual void ReceiveTargetEnemy(Transform target)
    {
        _target = target;
    }
    
    protected virtual void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _enemyAnimation = GetComponent<BaseEnemyAnimation>();
        _enemyAttack = GetComponent<BaseEnemyAttack>();
        
        _agent.updateRotation = false;
    }

    protected bool IsTargetInRange(float range)
    {
        return Vector3.Distance(transform.position, _target.position) <= range;
    }

    protected virtual void ChaseTarget()
    {
        _agent.isStopped = false;
        _agent.SetDestination(_target.position);
        _enemyAnimation.ChangeMoveState(true);
        
        RotateTowardsMovementDirection();
    }

    protected virtual void StopChasing()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _enemyAnimation.ChangeMoveState(false);
    }

    private void RotateTowardsMovementDirection()
    {
        if (_agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 lookDir = _agent.velocity.normalized;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
    }
}