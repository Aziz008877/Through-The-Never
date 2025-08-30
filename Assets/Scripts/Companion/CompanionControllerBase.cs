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
    [SerializeField] private float _playerHitPickRadius = 2f;
    [SerializeField] private float _retargetInterval = 0.25f;

    [Header("Combat State")]
    [SerializeField] private float _combatGraceTime = 4f;
    [SerializeField] private float _dashMinDist = 4f;

    [Header("Leash")]
    [SerializeField] private float _tetherOutRadius = 8f;
    [SerializeField] private float _tetherInRadius = 6f;
    [SerializeField] private float _followStopFromPlayer = 1.6f;

    [Header("Aiming")]
    [SerializeField] private Transform _aimRoot;
    [SerializeField] private float _aimHold = 0.25f;

    [Header("Skill Definitions")]
    public SkillDefinition DefensiveSkill;
    public SkillDefinition SpecialSkill;
    public SkillDefinition DashSkill;
    public SkillDefinition BasicSkill;
    public List<SkillDefinition> PassiveSkills = new();
    [SerializeField] private CompanionSkillManager _skillManager;

    protected CompanionContext Ctx;
    private CompanionMove _move;

    [Inject] private SkillRuntimeFactory _factory;
    [Inject] private PlayerContext _player;

    protected ActiveSkillBehaviour _def, _spec, _dash, _basic;
    private readonly List<PassiveSkillBehaviour> _passives = new();

    private Transform _target;
    private Transform _lastPlayerTarget;
    private bool _combatStarted;
    private float _combatTimer;
    private Vector3 _lastShot;
    private float _lastShotTime = -999f;
    private float _nextCast;
    private float _lastRetarget;

    private bool _leashing;
    private float _holdFacingUntil;
    private Transform _holdFacingTarget;

    protected virtual void Awake()
    {
        Ctx = GetComponent<CompanionContext>();
        _move = GetComponent<CompanionMove>();

        _def = _factory.Spawn(DefensiveSkill, Ctx, transform) as ActiveSkillBehaviour;
        _spec = _factory.Spawn(SpecialSkill, Ctx, transform) as ActiveSkillBehaviour;
        _dash = _factory.Spawn(DashSkill, Ctx, transform) as ActiveSkillBehaviour;
        _basic = _factory.Spawn(BasicSkill, Ctx, transform) as ActiveSkillBehaviour;

        _skillManager.AddSkill(SkillSlot.Defense, _def);
        _skillManager.AddSkill(SkillSlot.Special, _spec);
        _skillManager.AddSkill(SkillSlot.Dash, _dash);
        _skillManager.AddSkill(SkillSlot.Basic, _basic);

        if (_aimRoot == null) _aimRoot = transform;
        _move.SetAgentRotation(false);

        PlayerBasicAttackEvents.OnBasicAttack += SavePlayerShot;
    }

    protected virtual void Start()
    {
        foreach (var pd in PassiveSkills)
        {
            if (_factory.Spawn(pd, Ctx, transform) is PassiveSkillBehaviour p)
            {
                _passives.Add(p);
                _skillManager.AddPassive(p);
            }
        }
    }

    private void OnDestroy()
    {
        PlayerBasicAttackEvents.OnBasicAttack -= SavePlayerShot;
        foreach (var p in _passives) p.DisablePassive();
    }

    private void Update()
    {
        UpdateCombatState();

        if (_combatStarted)
        {
            if (ShouldLeashToPlayer())
            {
                FaceTarget();
                MoveTowardPlayerDuringCombat();
                CombatLoop();
            }
            else
            {
                RetargetIfNeeded();
                FaceTarget();
                MoveToTarget();
                CombatLoop();
            }
        }
        else
        {
            FollowPlayer();
        }
    }

    private void LateUpdate()
    {
        if (_target == null || !IsEnemyAlive(_target))
        {
            ClearAimLock();
            Vector3 mv = _move.LastMoveDirection;
            mv.y = 0f;
            if (mv.sqrMagnitude > 0.0001f)
            {
                ForceFace(_aimRoot.position + mv);
                return;
            }
            
            Vector3 toPlayer = _player.transform.position - _aimRoot.position;
            toPlayer.y = 0f;
            if (toPlayer.sqrMagnitude > 0.0001f) ForceFace(_aimRoot.position + toPlayer);
            return;
        }
        
        if (_leashing || Time.time < _holdFacingUntil)
        {
            ForceFace(_holdFacingTarget ? _holdFacingTarget.position : _target.position);
            return;
        }

        if (_move.LastMoveDirection.sqrMagnitude < 0.001f)
            ForceFace(_target.position);
    }

    private void UpdateCombatState()
    {
        if (_target != null && !IsEnemyAlive(_target))
        {
            _target = null;
            ClearAimLock();
        }

        if (PlayerActive)
        {
            if (_lastPlayerTarget == null || !IsEnemyAlive(_lastPlayerTarget))
                _lastPlayerTarget = FindEnemyNearPoint(_lastShot, _playerHitPickRadius);

            _target = IsEnemyAlive(_lastPlayerTarget) ? _lastPlayerTarget : null;
        }
        else
        {
            if (_target == null) _target = NearestEnemy();
        }

        if (_target != null)
        {
            _combatStarted = true;
            _combatTimer = 0f;
        }
        else if (_combatStarted)
        {
            _combatTimer += Time.deltaTime;
            if (_combatTimer >= _combatGraceTime)
            {
                _combatStarted = false;
                _target = null;
                ClearAimLock();
            }
        }
    }

    private void RetargetIfNeeded()
    {
        if (Time.time - _lastRetarget < _retargetInterval) return;
        _lastRetarget = Time.time;

        if (!IsEnemyAlive(_target))
        {
            _target = PlayerActive
                ? (IsEnemyAlive(_lastPlayerTarget) ? _lastPlayerTarget : FindEnemyNearPoint(_lastShot, _playerHitPickRadius))
                : NearestEnemy();

            if (_target == null) ClearAimLock();
        }
    }

    private bool ShouldLeashToPlayer()
    {
        float d = Vector3.Distance(transform.position, _player.transform.position);
        if (!_leashing && d > _tetherOutRadius) _leashing = true;
        else if (_leashing && d < _tetherInRadius) _leashing = false;
        return _leashing;
    }

    private void FollowPlayer()
    {
        float dist = Vector3.Distance(transform.position, _player.transform.position);
        if (dist > _followRadius) _move.MoveTo(_player.transform.position);
        else _move.Stop();
    }

    private void MoveTowardPlayerDuringCombat()
    {
        float dist = Vector3.Distance(transform.position, _player.transform.position);
        if (dist > _followStopFromPlayer) _move.MoveTo(_player.transform.position);
        else _move.Stop();
    }

    private void MoveToTarget()
    {
        if (_target == null) { _move.Stop(); return; }

        float distToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        if (distToPlayer > _tetherOutRadius) { MoveTowardPlayerDuringCombat(); return; }

        float distToTarget = Vector3.Distance(transform.position, _target.position);
        if (distToTarget > _stopDist) _move.MoveTo(_target.position);
        else _move.Stop();
    }

    private void FaceTarget()
    {
        if (_target == null) return;
        if (!IsEnemyAlive(_target)) return;
        Vector3 p = _target.position;
        p.y = transform.position.y;
        Vector3 dir = p - transform.position;
        if (dir.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(dir);
    }

    private void ForceFace(Vector3 lookPoint)
    {
        Vector3 p = lookPoint; p.y = _aimRoot.position.y;
        Vector3 dir = p - _aimRoot.position;
        if (dir.sqrMagnitude < 0.0001f) return;
        _aimRoot.rotation = Quaternion.LookRotation(dir);
    }

    private void ClearAimLock()
    {
        _holdFacingUntil = 0f;
        _holdFacingTarget = null;
    }

    private void SavePlayerShot(Vector3 pt)
    {
        _lastShot = pt;
        _lastShotTime = Time.time;
        _lastPlayerTarget = FindEnemyNearPoint(pt, _playerHitPickRadius);
        if (_lastPlayerTarget != null) _target = _lastPlayerTarget;
    }

    protected bool PlayerActive => Time.time - _lastShotTime <= _idleDelay;

    protected Transform FindEnemyNearPoint(Vector3 point, float radius)
    {
        Transform best = null; float min = float.MaxValue;
        foreach (var c in Physics.OverlapSphere(point, radius))
        {
            if (!c.TryGetComponent(out BaseEnemyHP _)) continue;
            float d = (c.transform.position - point).sqrMagnitude;
            if (d < min) { min = d; best = c.transform; }
        }
        return best;
    }

    protected virtual void CombatLoop()
    {
        if (Time.time < _nextCast || _target == null) return;

        Vector3 aimPoint = _target ? _target.position : transform.position;

        if (_def && _def.IsReady && Ctx.Hp.CurrentHP / Ctx.Hp.MaxHP < GetDefenceThreshold())
        {
            _holdFacingTarget = _target; _holdFacingUntil = Time.time + _aimHold;
            _def.TryCastByDefinition(_target);
            SetCd();
            return;
        }

        if (_spec && _spec.IsReady)
        {
            _holdFacingTarget = _target; _holdFacingUntil = Time.time + _aimHold;
            _spec.TryCastByDefinition(_target);
            SetCd();
            return;
        }

        if (_dash && _dash.IsReady)
        {
            float dist = Vector3.Distance(transform.position, _target.position);
            if (dist >= _dashMinDist)
            {
                _holdFacingTarget = _target; _holdFacingUntil = Time.time + _aimHold;
                _dash.TryCastByDefinition(_target);
                SetCd();
                return;
            }
        }

        if (_basic && _basic.IsReady)
        {
            _holdFacingTarget = _target; _holdFacingUntil = Time.time + _aimHold;
            _basic.TryCastByDefinition(_target);
            SetCd();
        }
    }

    protected Transform NearestEnemy()
    {
        Transform best = null; float min = float.MaxValue;
        foreach (var c in Physics.OverlapSphere(transform.position, _searchRadius))
        {
            if (!c.TryGetComponent(out BaseEnemyHP _)) continue;
            float d = (c.transform.position - transform.position).sqrMagnitude;
            if (d < min) { min = d; best = c.transform; }
        }
        return best;
    }

    private static bool IsEnemyAlive(Transform t)
    {
        if (t == null) return false;
        if (!t.gameObject.activeInHierarchy) return false;
        return t.TryGetComponent(out BaseEnemyHP _);
    }

    private void SetCd() => _nextCast = Time.time + GetGlobalCooldown();
    protected virtual float GetDefenceThreshold() => 0.3f;
    protected virtual float GetGlobalCooldown() => 0.25f;
}
