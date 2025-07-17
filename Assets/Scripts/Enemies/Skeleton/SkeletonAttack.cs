using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class SkeletonAttack : BaseEnemyAttack, IBlindable
{
    private float _blindTimer;
    private float _missChance;
    private float _slowPercent;
    private float _dps;

    private NavMeshAgent _agent;
    private BaseEnemyHP _hp; 
    private float _origSpeed;
    private Coroutine _blindRoutine;
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _hp = GetComponent<BaseEnemyHP>();
        _origSpeed = _agent.speed;
    }
    
    public void ApplyBlind(float duration, float missChance, float slowPercent, float dps)
    {
        _missChance  = missChance;
        _slowPercent = slowPercent;
        _dps = dps;
        
        _blindTimer = Mathf.Max(_blindTimer, duration);

        _blindRoutine ??= StartCoroutine(BlindTick());
    }

    public bool IsBlinded() => _blindTimer > 0f;
    public float CurrentMissChance => IsBlinded() ? _missChance : 0f;
    private IEnumerator BlindTick()
    {
        _agent.speed = _origSpeed * (1f - _slowPercent);

        while (_blindTimer > 0f)
        {
            float dt = Time.deltaTime;
            _blindTimer -= dt;

            if (_dps > 0f && _hp != null)
                _hp.ReceiveDamage(_dps * dt, SkillDamageType.Basic);

            yield return null;
        }
        
        _agent.speed = _origSpeed;
        _blindRoutine = null;
    }
}