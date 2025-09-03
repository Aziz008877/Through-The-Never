using UnityEngine;
using UnityEngine.AI;

public class MeleeMobBrain : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _player;

    [Header("Ranges")]
    [SerializeField] private float _meleeRange = 2.0f;

    [Header("Flow")]
    [SerializeField] private float _postMin = 0.4f;
    [SerializeField] private float _postMax = 0.8f;
    [SerializeField] private float _meleeWindup = 0.2f;
    [SerializeField] private float _meleeRepeatDelay = 0.7f;
    [SerializeField] private float _thinkInterval = 0.12f;
    [SerializeField] private float _stationaryEps = 0.05f;

    private MeleeMobAttack _attack;
    private BaseEnemyAnimation _anim;
    private BaseEnemyMove _move;
    private NavMeshAgent _agent;

    private float _postTimer, _meleeTimer, _thinkTimer, _meleeHold;
    private bool _busy;
    public void ReceivePlayer(Transform player)
    {
        _player = player;
    }
    
    private void Awake()
    {
        _attack = GetComponent<MeleeMobAttack>();
        _anim   = GetComponent<BaseEnemyAnimation>();
        _move   = GetComponent<BaseEnemyMove>();
        _agent  = GetComponent<NavMeshAgent>();
        _attack.OnCastEnded += OnCastEnded;
    }

    private void Start()
    {
        if (_player)
        {
            _move.ReceiveTargetEnemy(_player);
            _attack.SetTarget(_player);
        }
    }

    private void Update()
    {
        if (_postTimer > 0f)  _postTimer  -= Time.deltaTime;
        if (_meleeTimer > 0f) _meleeTimer -= Time.deltaTime;

        if (_thinkTimer > 0f) { _thinkTimer -= Time.deltaTime; return; }
        _thinkTimer = _thinkInterval;

        if (!_player) return;

        if (!_attack.IsManuallyCasting) _move.SetMoveState(true);
        if (_busy || _postTimer > 0f || _attack.IsManuallyCasting) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        bool far = dist > _attack.GetFarDistance();
        bool stand = AgentStationaryNearTarget();

        if (!stand || dist > _meleeRange) _meleeHold = 0f; else _meleeHold += Time.deltaTime;

        if (far)
        {
            if (_attack.Tier == MeleeMobTier.Tier4_Red && _attack.Toss_Available()) { Cast("Toss"); return; }
            return;
        }

        if (_attack.Tier >= MeleeMobTier.Tier3_Green && _attack.Roar_Available()) { Cast("Roar"); return; }
        if (_attack.Tier >= MeleeMobTier.Tier2_Cyan && _attack.Heavy_Available() && stand) { Cast("Heavy"); return; }
        if (dist <= _meleeRange && _meleeTimer <= 0f && _meleeHold >= _meleeWindup && stand) { Cast("Melee"); return; }
    }

    private bool AgentStationaryNearTarget()
    {
        if (_agent == null || !_agent.enabled || !_agent.isOnNavMesh) return true;
        if (_agent.pathPending) return false;
        if (_agent.remainingDistance > _agent.stoppingDistance + 0.05f) return false;
        if (_agent.velocity.sqrMagnitude > _stationaryEps * _stationaryEps) return false;
        return true;
    }

    private void Cast(string skill)
    {
        _busy = true;
        _attack.BeginManualCast(skill);
        switch (skill)
        {
            case "Melee": _anim.PlayMeleeAttack();  break;
            case "Heavy": _anim.PlaySpecial01();    break;
            case "Roar":  _anim.PlayRangedAttack(); break;
            case "Toss":  _anim.PlaySpecial02();    break;
        }
    }

    private void OnCastEnded(string skill, bool interrupted)
    {
        _busy = false;
        _postTimer = Random.Range(_postMin, _postMax);
        if (interrupted) return;
        if (skill == "Melee") _meleeTimer = _meleeRepeatDelay;
    }
}
