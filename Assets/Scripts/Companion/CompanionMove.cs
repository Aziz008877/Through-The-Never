using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class CompanionMove : MonoBehaviour, IActorMove
{
    public Vector3 LastMoveDirection { get; private set; }
    public event Action<Vector3> OnMove;
    private NavMeshAgent _agent;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
    }
    public void SetAgentRotation(bool enabled) => _agent.updateRotation = enabled;

    public void MoveTo(Vector3 pos)
    {
        if (!_agent.isOnNavMesh) return;
        _agent.SetDestination(pos);
    }

    public void Stop()
    {
        if (_agent.isOnNavMesh) _agent.ResetPath();
        _agent.velocity = Vector3.zero;
    }

    private void Update()
    {
        Vector3 vel = _agent.velocity; vel.y = 0f;
        LastMoveDirection = vel;
        OnMove?.Invoke(vel);
    }

    public bool Reached(float dist) =>
        !_agent.pathPending && _agent.remainingDistance <= dist;

    public void RotateTowardsMouse(float customDuration = -1) => throw new NotImplementedException();
    public void SetSpeedMultiplier(float multiplier) => throw new NotImplementedException();
}