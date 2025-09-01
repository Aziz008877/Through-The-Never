using UnityEngine;
using UnityEngine.AI;

public class RangedMobBrain : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _player;

    [Header("Ranges")]
    [SerializeField] private float _meleeRange = 1.9f;

    [Header("Flow")]
    [SerializeField] private float _postMin = 0.35f;
    [SerializeField] private float _postMax = 0.7f;
    [SerializeField] private float _meleeWindup = 0.15f;
    [SerializeField] private float _meleeRepeat = 0.7f;
    [SerializeField] private float _rangedRepeat = 0.6f;
    [SerializeField] private float _thinkInterval = 0.12f;

    private RangedMobAttack _atk;
    private BaseEnemyAnimation _anim;
    private BaseEnemyMove _move;
    private NavMeshAgent _agent;

    private float _postTimer, _meleeTimer, _rangedTimer, _thinkTimer, _meleeHold;
    private bool _busy;

    private void Awake()
    {
        _atk   = GetComponent<RangedMobAttack>();
        _anim  = GetComponent<BaseEnemyAnimation>();
        _move  = GetComponent<BaseEnemyMove>();
        _agent = GetComponent<NavMeshAgent>();
        _atk.OnCastEnded += OnCastEnded;
    }

    private void Start()
    {
        if (_player)
        {
            _move.ReceiveTargetEnemy(_player);
            _atk.SetTarget(_player);
        }
    }

    private void Update()
    {
        if (_postTimer  > 0f) _postTimer  -= Time.deltaTime;
        if (_meleeTimer > 0f) _meleeTimer -= Time.deltaTime;
        if (_rangedTimer> 0f) _rangedTimer-= Time.deltaTime;

        if (_thinkTimer > 0f) { _thinkTimer -= Time.deltaTime; return; }
        _thinkTimer = _thinkInterval;

        if (!_player) return;

        if (!_atk.IsManuallyCasting) _move.SetMoveState(true);
        if (_busy || _postTimer > 0f || _atk.IsManuallyCasting) return;

        float dist  = Vector3.Distance(transform.position, _player.position);
        bool close  = dist <= _meleeRange;

        _meleeHold = close ? (_meleeHold + Time.deltaTime) : 0f;

        if (_atk.Tier == RangedMobTier.Tier3_Purple && _atk.Buff_Available() && !close)
        { Cast("Buff"); return; }

        if (close && _atk.Tier >= RangedMobTier.Tier2_Orange)
        {
            if (_meleeTimer <= 0f && _meleeHold >= _meleeWindup)
            { Cast("Melee"); return; }
            else
            { return; }
        }

        if (_rangedTimer <= 0f) { Cast("Ranged"); return; }
    }

    private void Cast(string skill)
    {
        _busy = true;
        _atk.BeginManualCast(skill);
        switch (skill)
        {
            case "Melee":  _anim.PlayMeleeAttack();  break;
            case "Ranged": _anim.PlayRangedAttack(); break;
            case "Buff":   _anim.PlaySpecial01();    break;
        }
    }

    private void OnCastEnded(string skill, bool interrupted)
    {
        _busy = false;
        _postTimer = Random.Range(_postMin, _postMax);
        if (interrupted) return;

        if (skill == "Melee")  _meleeTimer  = _meleeRepeat;
        if (skill == "Ranged") _rangedTimer = _rangedRepeat;
    }
}
