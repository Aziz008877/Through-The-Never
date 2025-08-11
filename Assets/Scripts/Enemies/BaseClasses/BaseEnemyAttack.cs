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
    private BaseEnemyHP _baseEnemyHp;
    private float _meleeTimer, _rangedTimer, _lockTimer;

    private IFrostbiteReceivable _frost;

    public bool IsCasting { get; private set; }
    public event Action<float> OnAttackStarted;

    private void Awake()
    {
        _baseEnemyHp = GetComponent<BaseEnemyHP>();
        _anim = GetComponent<BaseEnemyAnimation>();
        TryGetComponent<IFrostbiteReceivable>(out _frost);
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

    public void HandleMeleeHit() { if (IsCasting) DealMeleeHit(); }
    public void HandleRangedFire() { if (IsCasting) DealRangedShot(); }

    public void MeleeEndEvent() { if (IsCasting) EndCast(ref _meleeTimer,  _meleeCd); }
    public void RangedEndEvent() { if (IsCasting) EndCast(ref _rangedTimer, _rangedCd); }

    protected virtual void DealMeleeHit()
    {
        if (!TryGetValidTarget(_meleeDist, out var player, out var enemy)) return;
        float dmg = _meleeDamage;
        if (_frost != null && _frost.IsFrostActive) dmg *= _frost.OutgoingDamageMul;
        ApplyDamage(player, enemy, dmg);
    }

    protected virtual void DealRangedShot()
    {
        if (!TryGetValidTarget(_rangedDist, out var player, out var enemy)) return;
        float dmg = _rangedDamage;
        if (_frost != null && _frost.IsFrostActive) dmg *= _frost.OutgoingDamageMul;
        ApplyDamage(player, enemy, dmg);
    }

    private bool TryGetValidTarget(float maxDist, out PlayerHP player, out BaseEnemyHP enemy)
    {
        player = null; enemy = null;
        if (_target == null) return false;
        if ((transform.position - _target.position).sqrMagnitude > maxDist * maxDist) return false;

        player = _target.GetComponent<PlayerHP>();
        enemy  = player ? null : _target.GetComponent<BaseEnemyHP>();
        if (player == null && enemy == null) return false;

        if (TryGetComponent<IBlindable>(out var blind) && blind.IsBlinded() && Random.value < blind.CurrentMissChance)
            return false;

        return true;
    }

    private void ApplyDamage(PlayerHP player, BaseEnemyHP enemy, float dmg)
    {
        if (player) player.ReceiveDamage(dmg, _baseEnemyHp);
        else enemy.ReceiveDamage(dmg, SkillDamageType.Basic);
    }

    private void TickTimers()
    {
        if (_meleeTimer > 0f) _meleeTimer -= Time.deltaTime;
        if (_rangedTimer > 0f) _rangedTimer -= Time.deltaTime;
        if (_lockTimer > 0f) _lockTimer -= Time.deltaTime;
    }

    private void BeginCast()
    {
        IsCasting = true;
        _lockTimer = 999f;
        OnAttackStarted?.Invoke(_stopAfterCast);
    }

    private void EndCast(ref float cdTimer, float cdValue)
    {
        IsCasting = false;
        cdTimer = cdValue;
        _lockTimer = _stopAfterCast;
    }
}
