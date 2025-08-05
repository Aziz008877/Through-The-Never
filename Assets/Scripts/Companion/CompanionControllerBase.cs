using UnityEngine;
using Zenject;
using System.Collections.Generic;

[RequireComponent(typeof(CompanionContext), typeof(CompanionMove))]
public abstract class CompanionControllerBase : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private float _followRadius = 6f;
    [SerializeField] private float _stopDist = 1.4f;
    [SerializeField] private float _idleDelay = 2f;

    [Header("Enemy Search")]
    [SerializeField] private float _searchRadius = 12f;
    [SerializeField] private LayerMask _enemyMask = ~0;

    [Header("Combat State")]
    [SerializeField] private float _combatGraceTime = 4f;

    [Header("Skill Definitions")]
    public SkillDefinition DefensiveSkill;
    public SkillDefinition SpecialSkill;
    public SkillDefinition DashSkill;
    public SkillDefinition BasicSkill;
    public List<SkillDefinition> PassiveSkills = new();

    protected CompanionContext Ctx;
    private CompanionMove _move;

    [Inject] private SkillRuntimeFactory _factory;
    [Inject] private PlayerContext _player;

    protected ActiveSkillBehaviour _def, _spec, _dash, _basic;
    private readonly List<PassiveSkillBehaviour> _passives = new();

    [SerializeField] private bool _combatStarted;
    private float _combatTimer;
    private Vector3 _lastShot;
    private float _lastShotTime = -999f;
    private float _nextCast;

    protected virtual void Awake()
    {
        Ctx = GetComponent<CompanionContext>();
        _move = GetComponent<CompanionMove>();

        _def = _factory.Spawn(DefensiveSkill, Ctx, transform) as ActiveSkillBehaviour;
        _spec = _factory.Spawn(SpecialSkill, Ctx, transform) as ActiveSkillBehaviour;
        _dash = _factory.Spawn(DashSkill, Ctx, transform) as ActiveSkillBehaviour;
        _basic = _factory.Spawn(BasicSkill, Ctx, transform) as ActiveSkillBehaviour;

        foreach (var pd in PassiveSkills)
        {
            if (_factory.Spawn(pd, Ctx, transform) is PassiveSkillBehaviour p)
            {
                p.EnablePassive();
                _passives.Add(p);
            }
        }

        PlayerBasicAttackEvents.OnBasicAttack += SavePlayerShot;
    }

    private void OnDestroy()
    {
        PlayerBasicAttackEvents.OnBasicAttack -= SavePlayerShot;
        foreach (var p in _passives) p.DisablePassive();
    }

    private void Update()
    {
        //UpdateCombatState();

        if (_combatStarted) CombatLoop();
        else FollowPlayer();
    }

    private void UpdateCombatState()
    {
        bool playerShotRecently = PlayerActive;
        bool enemyNearby = NearestEnemy() != null;

        if (playerShotRecently || enemyNearby)
        {
            _combatStarted = true;
            _combatTimer = 0f;
        }
        else if (_combatStarted)
        {
            _combatTimer += Time.deltaTime;
            if (_combatTimer >= _combatGraceTime)
                _combatStarted = false;
        }
    }

    private void FollowPlayer()
    {
        float dist = Vector3.Distance(transform.position, _player.transform.position);
        if (dist > _followRadius) _move.MoveTo(_player.transform.position);
        else _move.Stop();
    }

    private void SavePlayerShot(Vector3 pt)
    {
        _lastShot = pt;
        _lastShotTime = Time.time;
    }

    protected bool PlayerActive => Time.time - _lastShotTime <= _idleDelay;
    protected Vector3 LastShot => _lastShot;

    protected Transform NearestEnemy()
    {
        Transform best = null;
        float min = float.MaxValue;

        foreach (var c in Physics.OverlapSphere(transform.position, _searchRadius, _enemyMask))
        {
            float d = (c.transform.position - transform.position).sqrMagnitude;
            if (d < min) { min = d; best = c.transform; }
        }

        return best;
    }

    protected virtual void CombatLoop()
    {
        if (Time.time < _nextCast) return;

        if (_def && _def.IsReady && Ctx.Hp.CurrentHP / Ctx.Hp.MaxHP < GetDefenceThreshold())
        {
            _def.TryCast();
            SetCd();
            return;
        }

        Transform t = PlayerActive ? null : NearestEnemy();

        if (_spec && _spec.IsReady)
        {
            if (t) _spec.transform.LookAt(t);
            else _spec.transform.forward = (LastShot - Ctx.CastPivot.position).normalized;

            _spec.TryCast();
            SetCd();
            return;
        }

        if (_dash && _dash.IsReady && t)
        {
            _dash.TryCastAtTarget(t);
            SetCd();
            return;
        }

        if (_basic && _basic.IsReady)
        {
            if (PlayerActive)
            {
                Vector3 dir = (LastShot - Ctx.CastPivot.position).normalized;
                transform.forward = dir;
                _basic.TryCastAtPoint(LastShot);
            }
            else if (t)
            {
                _basic.TryCastAtTarget(t);
            }
            SetCd();
        }
    }

    private void SetCd() => _nextCast = Time.time + GetGlobalCooldown();

    protected virtual float GetDefenceThreshold() => 0.3f;
    protected virtual float GetGlobalCooldown() => 0.25f;
}
