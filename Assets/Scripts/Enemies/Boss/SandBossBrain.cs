using UnityEngine;
using UnityEngine.AI;

public class SandBossBrain : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _player;

    [Header("Cooldowns (sec)")]
    [SerializeField] private float _dashCd = 5f;
    [SerializeField] private float _shatterCd = 10f;
    [SerializeField] private float _tossCd = 10f;

    [Header("Ranges")]
    [SerializeField] private float _meleeRange = 2.0f;

    [Header("Flow Control")]
    [SerializeField] private float _postCastDelayMin = 0.5f;
    [SerializeField] private float _postCastDelayMax = 1.0f;
    [SerializeField] private float _minMeleeRepeatDelay = 0.8f;
    [SerializeField] private float _meleeWindup = 0.25f;
    [SerializeField] private float _thinkInterval = 0.12f;
    [SerializeField] private float _stationarySpeedEps = 0.05f;

    [Header("Chase → Force Ranged")]
    [SerializeField] private float _chaseTossDelay = 1.0f; 

    private SandBossAttack _attack;
    private BaseEnemyAnimation _anim;
    private BaseEnemyMove _move;
    private IStunnable _stun;
    private NavMeshAgent _agent;

    private float _dashTimer, _shatterTimer, _tossTimer;
    private float _postTimer, _meleeTimer, _thinkTimer, _meleeHoldTimer;
    private float _chaseTimer; // накапливаем, пока бежим и не в мили-дистанции
    private bool _busy;

    private void Awake()
    {
        _attack = GetComponent<SandBossAttack>();
        _anim   = GetComponent<BaseEnemyAnimation>();
        _move   = GetComponent<BaseEnemyMove>();
        _stun   = GetComponent<IStunnable>();
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
        if (_dashTimer    > 0f) _dashTimer    -= Time.deltaTime;
        if (_shatterTimer > 0f) _shatterTimer -= Time.deltaTime;
        if (_tossTimer    > 0f) _tossTimer    -= Time.deltaTime;
        if (_postTimer    > 0f) _postTimer    -= Time.deltaTime;
        if (_meleeTimer   > 0f) _meleeTimer   -= Time.deltaTime;

        if (_thinkTimer > 0f) { _thinkTimer -= Time.deltaTime; return; }
        _thinkTimer = _thinkInterval;

        if (_stun != null && _stun.IsStunned) return;

        if (!_attack.IsManuallyCasting) _move.SetMoveState(true);
        if (_busy || _postTimer > 0f || _attack.IsManuallyCasting) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        bool inMelee = dist <= _meleeRange;
        bool far = dist > _attack.FarDistance;
        bool agentReady = AgentStationaryNearTarget();

        // Копим время погони, если агент движется и мы вне мили-дистанции
        bool isChasing = !agentReady && !inMelee;
        _chaseTimer = isChasing ? (_chaseTimer + Time.deltaTime) : 0f;

        // Сбрасываем "удержание" мили, если мы далеко или ещё не остановились у цели
        if (!agentReady || !inMelee) _meleeHoldTimer = 0f;
        else _meleeHoldTimer += Time.deltaTime;

        // --- ДАЛЕКАЯ ДИСТАНЦИЯ: Dash/Toss как раньше ---
        if (far)
        {
            var (min, max) = _attack.DashRange;
            if (_dashTimer <= 0f && dist >= min && dist <= max) { TryCast(SandBossSkill.Dash); return; }
            if (_tossTimer <= 0f) { TryCast(SandBossSkill.Toss); return; }
            return;
        }

        // --- «Серая зона» между meleeRange и FarDistance ---
        // Если давно бежим и не можем в мили — форсим Toss.
        if (!inMelee && _chaseTimer >= _chaseTossDelay && _tossTimer <= 0f)
        {
            TryCast(SandBossSkill.Toss);
            return;
        }

        // --- БЛИЗКО: мили при выполнении условий ---
        if (inMelee && _meleeTimer <= 0f && _meleeHoldTimer >= _meleeWindup && agentReady)
        {
            TryCast(SandBossSkill.Melee);
            return;
        }

        // Shatter по-прежнему только когда мы остановились у цели
        if (_shatterTimer <= 0f && agentReady)
        {
            TryCast(SandBossSkill.Shatter);
            return;
        }
    }

    private bool AgentStationaryNearTarget()
    {
        if (_agent == null || !_agent.enabled || !_agent.isOnNavMesh) return true;
        if (_agent.pathPending) return false;
        if (_agent.remainingDistance > _agent.stoppingDistance + 0.05f) return false;
        if (_agent.velocity.sqrMagnitude > _stationarySpeedEps * _stationarySpeedEps) return false;
        return true;
    }

    private void TryCast(SandBossSkill skill)
    {
        _busy = true;
        _attack.BeginManualCast(skill);
        switch (skill)
        {
            case SandBossSkill.Melee:   _anim.PlayMeleeAttack();  break;
            case SandBossSkill.Dash:    _anim.PlayRangedAttack(); break;
            case SandBossSkill.Shatter: _anim.PlaySpecial01();    break;
            case SandBossSkill.Toss:    _anim.PlaySpecial02();    break;
        }
    }

    private void OnCastEnded(SandBossSkill skill, bool interrupted)
    {
        _busy = false;
        _postTimer = Random.Range(_postCastDelayMin, _postCastDelayMax);
        if (interrupted) return;

        switch (skill)
        {
            case SandBossSkill.Dash:    _dashTimer    = _dashCd;    break;
            case SandBossSkill.Shatter: _shatterTimer = _shatterCd; break;
            case SandBossSkill.Toss:    _tossTimer    = _tossCd;    break;
            case SandBossSkill.Melee:   _meleeTimer   = _minMeleeRepeatDelay; break;
        }
    }
}
