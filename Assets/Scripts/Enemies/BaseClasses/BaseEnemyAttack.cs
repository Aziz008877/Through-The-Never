using UnityEngine;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BaseEnemyAnimation))]
public abstract class BaseEnemyAttack : MonoBehaviour
{
    [Header("Melee")]
    [SerializeField] private bool  _useMelee    = true;
    [SerializeField] private float _meleeDamage = 10f;
    [SerializeField] private float _meleeDist   = 2f;
    [SerializeField] private float _meleeCd     = 2f;

    [Header("Ranged")]
    [SerializeField] private bool  _useRanged    = true;
    [SerializeField] private float _rangedDamage = 8f;
    [SerializeField] private float _rangedDist   = 6f;
    [SerializeField] private float _rangedCd     = 4f;

    [Header("Behaviour")]
    [SerializeField] private float _stopAfterCast = .8f;

    private Transform _target;
    private BaseEnemyAnimation _anim;

    private ActorContext _attackerCtx;              // контекст врага-атакера
    private IFrostbiteReceivable _frost;            // модификатор исходящего урона
    private IBlindable _blindSelf;                  // промах при ослеплении

    private float _meleeTimer, _rangedTimer, _lockTimer;

    public bool IsCasting { get; private set; }
    public event Action<float> OnAttackStarted;

    private void Awake()
    {
        _anim = GetComponent<BaseEnemyAnimation>();
        _attackerCtx = GetComponent<ActorContext>() ?? GetComponentInParent<ActorContext>();
        TryGetComponent(out _frost);
        TryGetComponent(out _blindSelf);
    }

    private void Update()
    {
        if (_target == null) return;

        TickTimers();
        if (_lockTimer > 0f) return;

        float dist = Vector3.Distance(transform.position, _target.position);

        if (_useMelee && _meleeTimer <= 0f && dist <= _meleeDist)
        {
            BeginCast(); _anim.PlayMeleeAttack(); return;
        }

        if (_useRanged && _rangedTimer <= 0f && dist <= _rangedDist)
        {
            BeginCast(); _anim.PlayRangedAttack();
        }
    }

    public void SetTarget(Transform t) => _target = t;
    public bool ShouldStopMovement() => _lockTimer > 0f;

    public void HandleMeleeHit()   { if (IsCasting) DealMeleeHit(); }
    public void HandleRangedFire() { if (IsCasting) DealRangedShot(); }

    public void MeleeEndEvent()  { if (IsCasting) EndCast(ref _meleeTimer,  _meleeCd); }
    public void RangedEndEvent() { if (IsCasting) EndCast(ref _rangedTimer, _rangedCd); }

    protected virtual void DealMeleeHit()
    {
        if (!TryGetValidTarget(_meleeDist, out var target)) return;
        float dmg = _meleeDamage;
        if (_frost != null && _frost.IsFrostActive) dmg *= _frost.OutgoingDamageMul;
        ApplyDamage(target, dmg, SkillDamageType.Basic);
    }

    protected virtual void DealRangedShot()
    {
        if (!TryGetValidTarget(_rangedDist, out var target)) return;
        float dmg = _rangedDamage;
        if (_frost != null && _frost.IsFrostActive) dmg *= _frost.OutgoingDamageMul;
        ApplyDamage(target, dmg, SkillDamageType.Basic);
    }

    private bool TryGetValidTarget(float maxDist, out IDamageable target)
    {
        target = null;
        if (_target == null) return false;
        if ((transform.position - _target.position).sqrMagnitude > maxDist * maxDist) return false;

        if (!_target.TryGetComponent(out target)) return false;

        if (_blindSelf != null && _blindSelf.IsBlinded() && Random.value < _blindSelf.CurrentMissChance)
            return false;

        return true;
    }

    private void ApplyDamage(IDamageable target, float dmg, SkillDamageType type)
    {
        var ctx = new DamageContext
        {
            Attacker       = _attackerCtx,
            Target         = target,
            SkillBehaviour = null,
            SkillDef       = null,
            Slot           = SkillSlot.Undefined,
            Type           = type,
            Damage         = dmg,
            IsCrit         = false,
            CritMultiplier = 1f,
            SourceGO       = gameObject
        };

        _attackerCtx?.ApplyDamageContextModifiers(ref ctx);
        target.ReceiveDamage(ctx); // события разойдутся внутри цели
    }

    private void TickTimers()
    {
        if (_meleeTimer  > 0f) _meleeTimer  -= Time.deltaTime;
        if (_rangedTimer > 0f) _rangedTimer -= Time.deltaTime;
        if (_lockTimer   > 0f) _lockTimer   -= Time.deltaTime;
    }

    private void BeginCast()
    {
        IsCasting = true;
        _lockTimer = 999f;                  // блок до конца анимации
        OnAttackStarted?.Invoke(_stopAfterCast);
    }

    private void EndCast(ref float cdTimer, float cdValue)
    {
        IsCasting = false;
        cdTimer = cdValue;
        _lockTimer = _stopAfterCast;       // короткий стоп после каста
    }
}
