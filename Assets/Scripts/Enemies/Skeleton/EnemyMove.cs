using UnityEngine;
using UnityEngine.AI;
public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _attackDistance = 2f;
    [SerializeField] private float _attackCooldown = 1.5f;
    private NavMeshAgent _agent;
    private BaseEnemyAnimation _enemyAnimation;
    private float _lastAttackTime;
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _enemyAnimation = GetComponent<BaseEnemyAnimation>();
    }

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
        _enemyAnimation.AttackAnimation();
    }

    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
    }
}
