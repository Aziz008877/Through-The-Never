using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public enum RangedMobTier { Tier1_Green = 1, Tier2_Orange = 2, Tier3_Purple = 3 }
public class RangedMobAttack : BaseEnemyAttack
{
    [Header("Tier")]
    [SerializeField] private RangedMobTier _tier = RangedMobTier.Tier1_Green;
    [Header("General")]
    [SerializeField] private Transform _ownerRoot;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private float _farDistance = 10f;
    [SerializeField] private float _meleeRange = 1.9f;

    [Header("Damages")]
    [SerializeField] private float _meleeLow = 8f;
    [SerializeField] private float _meleeAvg = 12f;
    [SerializeField] private float _rangedLow = 8f;
    [SerializeField] private float _rangedAvg = 12f;

    [Header("Projectile")]
    [SerializeField] private Transform _projPrefab;
    [SerializeField] private float _projSpeedLow = 10f;
    [SerializeField] private float _projSpeedAvg = 16f;
    [SerializeField] private float _projSpeedHigh = 22f;
    [SerializeField] private float _projHitRadius = 0.8f;
    [SerializeField] private float _projMaxDistance = 20f;

    [Header("Buff (Spell1)")]
    [SerializeField] private float _buffDuration = 5f;
    [SerializeField] private float _buffCd = 15f;
    [SerializeField] private float _healAmount = 15f;

    private BaseEnemyMove _move;
    private IDamageable _selfAsSource;
    private Transform _targetTr;

    private bool _isCasting;
    private bool _buffActive;
    private float _buffTimeLeft;
    private float _buffCdTimer;

    public bool IsManuallyCasting => _isCasting;
    public float FarDistance => _farDistance;
    public RangedMobTier Tier => _tier;

    public event Action<string, bool> OnCastEnded;

    private void Start()
    {
        _move = GetComponent<BaseEnemyMove>();
        TryGetComponent(out _selfAsSource);
    }
    public void SetTier(RangedMobTier tier)
    {
        _tier = tier;
        GetComponent<EnemyMaterialApplier>().Refresh();
    }
    private void Update()
    {
        if (_buffCdTimer > 0f) _buffCdTimer -= Time.deltaTime;
        if (_buffTimeLeft > 0f)
        {
            _buffTimeLeft -= Time.deltaTime;
            if (_buffTimeLeft <= 0f) _buffActive = false;
        }
    }

    public new void SetTarget(Transform t) { _targetTr = t; base.SetTarget(t); }

    public void BeginManualCast(string _) { if (_isCasting) return; _isCasting = true; _move.SetMoveState(false); }
    public void EndManualCast(string skill, bool interrupted)
    {
        if (!_isCasting) return;
        _isCasting = false; _move.SetMoveState(true);
        if (!interrupted && skill == "Buff") _buffCdTimer = _buffCd;
        OnCastEnded?.Invoke(skill, interrupted);
    }
    public void InterruptCast() { if (!_isCasting) return; StopAllCoroutines(); EndManualCast("Any", true); }
    public void Melee_Hit()
    {
        float dmg = (_tier >= RangedMobTier.Tier3_Purple ? _meleeAvg : (_tier >= RangedMobTier.Tier2_Orange ? _meleeLow : _meleeLow));
        Vector3 center = transform.position + transform.forward * Mathf.Max(0.6f, _meleeRange * 0.5f);
        var set = new HashSet<ActorContext>();
        foreach (var c in Physics.OverlapSphere(center, _meleeRange * 0.6f))
        {
            var ac = c.GetComponentInParent<ActorContext>();
            if (ac != null) set.Add(ac);
        }
        foreach (var ac in set) ac.Hp?.ReceiveDamage(dmg, _selfAsSource);
    }
    public void Melee_End() => EndManualCast("Melee", false);
    public void Ranged_Fire() { StartCoroutine(FireRoutine()); }
    private IEnumerator FireRoutine()
    {
        LaunchOne();
        if (_tier == RangedMobTier.Tier3_Purple && _buffActive) { yield return new WaitForSeconds(0.06f); LaunchOne(); }
    }
    private void LaunchOne()
    {
        if (_projPrefab == null) return;

        Vector3 start = _muzzle ? _muzzle.position : transform.position + Vector3.up * 1.2f;
        Quaternion rot = Quaternion.LookRotation(transform.forward, Vector3.up);

        float spd = _projSpeedLow;
        if      (_tier == RangedMobTier.Tier2_Orange) spd = _projSpeedAvg;
        else if (_tier == RangedMobTier.Tier3_Purple) spd = _projSpeedHigh;

        float dmg = (_tier == RangedMobTier.Tier3_Purple ? _rangedAvg : _rangedLow);

        var projectile = Instantiate(_projPrefab, start, rot);

        var mover = projectile.gameObject.GetComponent<EnemyProjectile>();
        mover.Init(spd, _projMaxDistance, dmg, _selfAsSource);
    }
    public void Ranged_End() => EndManualCast("Ranged", false);
    public bool Buff_Available() => _tier == RangedMobTier.Tier3_Purple && !_buffActive && _buffCdTimer <= 0f;
    public void Buff_Apply()
    {
        _buffActive = true;
        _buffTimeLeft = _buffDuration;
        if (TryGetComponent(out ActorContext ctx)) ctx.Hp?.ReceiveDamage(-_healAmount, _selfAsSource);
    }
    public void Buff_End() => EndManualCast("Buff", false);
}
