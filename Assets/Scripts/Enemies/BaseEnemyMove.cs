using UnityEngine;
using UnityEngine.AI;
public class BaseEnemyMove : MonoBehaviour
{
    [SerializeField] protected Transform _target;
    protected NavMeshAgent _agent;
    protected BaseEnemyAnimation _enemyAnimation;
    protected BaseEnemyAttack _enemyAttack;
    protected virtual void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _enemyAnimation = GetComponent<BaseEnemyAnimation>();
        _enemyAttack = GetComponent<BaseEnemyAttack>();
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
    }

    protected virtual void StopChasing()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _enemyAnimation.ChangeMoveState(false);
    }
}
