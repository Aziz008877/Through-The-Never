using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class BaseEnemyMove : MonoBehaviour
{
    [Header("General")]
    [SerializeField] protected float _rotationSpeed = 720f;
    [SerializeField] protected MoveBehaviour _behaviour = MoveBehaviour.ChaseIfInRange;
    [SerializeField] protected float _chaseRange = 12f;
    [SerializeField] protected float _stopDistance = 1.6f;

    [Header("Patrol")]
    [SerializeField] protected Transform[] _patrolPoints;
    [SerializeField] protected float _wanderPause = 1.2f;
    
    protected NavMeshAgent _agent;
    protected BaseEnemyAnimation _enemyAnimation;
    protected BaseEnemyAttack _enemyAttack;
    protected Transform _target;
    protected bool _isMoving;
    protected int _patrolIndex;
    protected float _pauseTimer;
    protected bool _canMove = true;
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
        _agent.stoppingDistance = _stopDistance;
    }

    public void StopChasing()
    {
        _canMove = false;
    }

    protected virtual void Update()
    {
        if (!_canMove) return;
        
        switch (_behaviour)
        {
            case MoveBehaviour.Idle:
                IdleLogic();
                break;

            case MoveBehaviour.Patrol:
                PatrolLogic();
                break;

            case MoveBehaviour.ChaseAlways:
                ChaseLogic();
                break;

            case MoveBehaviour.ChaseIfInRange:
                if (_target != null && InChaseRange())
                    ChaseLogic();
                else
                    PatrolLogic();
                break;
        }

        RotateTowardsMovement();
    }

    protected virtual void IdleLogic()
    {
        if (_isMoving) StopMoving();
    }

    protected virtual void PatrolLogic()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0)
        {
            IdleLogic();
            return;
        }

        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _pauseTimer += Time.deltaTime;
            if (_pauseTimer < _wanderPause)
            {
                StopMoving();
                return;
            }

            _pauseTimer = 0f;
            _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
            MoveTo(_patrolPoints[_patrolIndex].position);
        }
        else if (!_isMoving)
        {
            MoveTo(_patrolPoints[_patrolIndex].position);
        }
    }

    protected virtual void ChaseLogic()
    {
        if (_target == null)
        {
            IdleLogic();
            return;
        }
        
        _enemyAttack.SetTarget(_target);
        
        if (_enemyAttack.ShouldStopMovement())
        {
            StopMoving();
            return;
        }
        
        float sqrDist = (transform.position - _target.position).sqrMagnitude;
        float stopSqr = _stopDistance * _stopDistance;

        if (sqrDist <= stopSqr)
        {
            StopMoving();
        }
        else
        {
            MoveTo(_target.position);
        }
    }
    
    protected bool InChaseRange()
    {
        if (_target == null) return false;
        return (transform.position - _target.position).sqrMagnitude <= _chaseRange * _chaseRange;
    }

    protected void MoveTo(Vector3 pos)
    {
        if (_agent.isOnNavMesh)
            _agent.SetDestination(pos);

        _agent.isStopped = false;
        _isMoving = true;
        _enemyAnimation.SetMove(true);
    }

    protected void StopMoving()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _isMoving = false;
        _enemyAnimation.SetMove(false);
    }

    void RotateTowardsMovement()
    {
        Vector3 lookDir;

        if (_isMoving && _agent.velocity.sqrMagnitude > 0.01f)
        {
            lookDir = _agent.velocity;
        }
        else if (_target != null && !_isMoving)
        {
            lookDir = _target.position - transform.position;
        }
        else
            return;

        lookDir.y = 0f;
        if (lookDir.sqrMagnitude < 0.01f) return;

        Quaternion want = Quaternion.LookRotation(lookDir.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, want, _rotationSpeed * Time.deltaTime);
    }

    protected enum MoveBehaviour
    {
        Idle,
        Patrol,
        ChaseAlways,
        ChaseIfInRange
    }
}
