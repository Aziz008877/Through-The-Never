using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class CompanionMove : MonoBehaviour, IActorMove
{
    public Vector3 LastMoveDirection { get; private set; }
    public event Action<Vector3> OnMove;

    private NavMeshAgent _agent;
    private bool _hasPendingUpdateRotation;
    private bool _pendingUpdateRotationValue;

    private void Awake()
    {
        EnsureAgent();
        _agent.updateRotation = false;
    }

    private void OnEnable()
    {
        EnsureAgent();

        if (_hasPendingUpdateRotation)
        {
            _agent.updateRotation = _pendingUpdateRotationValue;
            _hasPendingUpdateRotation = false;
        }
    }

    private void EnsureAgent()
    {
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null)
            {
                Debug.LogError("[CompanionMove] NavMeshAgent not found.");
                return;
            }
        }
    }

    public void SetAgentRotation(bool enabled)
    {
        EnsureAgent();
        if (_agent != null)
        {
            _agent.updateRotation = enabled;
        }
        else
        {
            _hasPendingUpdateRotation = true;
            _pendingUpdateRotationValue = enabled;
        }
    }

    public void MoveTo(Vector3 pos)
    {
        EnsureAgent();
        if (_agent == null) return;
        
        if (!_agent.isOnNavMesh) return;

        _agent.SetDestination(pos);
    }

    public void Stop()
    {
        EnsureAgent();
        if (_agent == null) return;

        if (_agent.isOnNavMesh) _agent.ResetPath();
        _agent.velocity = Vector3.zero;
    }

    private void Update()
    {
        if (_agent == null) return;

        Vector3 vel = _agent.velocity;
        vel.y = 0f;

        LastMoveDirection = vel;
        OnMove?.Invoke(vel);
    }

    public bool Reached(float dist)
    {
        if (_agent == null) return false;
        return !_agent.pathPending && _agent.remainingDistance <= dist;
    }

    public void RotateTowardsMouse(float customDuration = -1) => throw new NotImplementedException();
    public void SetSpeedMultiplier(float multiplier)          => throw new NotImplementedException();
}
