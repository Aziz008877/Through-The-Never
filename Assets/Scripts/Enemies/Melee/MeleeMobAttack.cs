using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public enum MeleeMobTier { Tier1_Blue = 1, Tier2_Cyan = 2, Tier3_Green = 3, Tier4_Red = 4 }
public class MeleeMobAttack : BaseEnemyAttack
{
    [Header("Tier")]
    [SerializeField] private MeleeMobTier _tier = MeleeMobTier.Tier1_Blue;
    [Header("General")]
    [SerializeField] private Transform _ownerRoot;
    [SerializeField] private float _farDistance = 7f;
    [SerializeField] private float _meleeRange = 2f;

    [Header("Damages")]
    [SerializeField] private float _meleeLowDamage = 8f;
    [SerializeField] private float _meleeAverageDamage = 12f;
    [SerializeField] private float _heavyHighDamage = 28f;
    [SerializeField] private float _tossAverageDamage = 14f;

    [Header("Heavy (Attack3)")]
    [SerializeField] private float _heavyRadius = 2.1f;
    [SerializeField] private float _heavyCd = 10f;

    [Header("Roar")]
    [SerializeField] private float _roarDuration = 5f;
    [SerializeField] private float _roarCd = 15f;
    [SerializeField] private float _roarMoveMul = 1.35f;
    [SerializeField] private float _roarDamageMul = 1.35f;

    [Header("Boulder Toss (Attack4)")]
    [SerializeField] private Rigidbody _boulderPrefab;
    [SerializeField] private Transform _tossSpawn;
    [SerializeField] private float _tossArcHeight = 2.5f;
    [SerializeField] private float _tossSpeed = 16f;
    [SerializeField] private float _tossExplosionRadius = 1.8f;
    [SerializeField] private float _tossCd = 10f;

    private BaseEnemyMove _move;
    private IDamageable _selfAsSource;

    private bool _isCasting;
    private Rigidbody _preparedProj;
    private Vector3 _preparedTargetPoint;
    public int GetTierOrDefault(int fallback = 1) => (int)_tier <= 0 ? fallback : (int)_tier;
    private float _heavyTimer, _roarTimer, _roarTimeLeft, _tossTimer;
    public void SetTier(MeleeMobTier tier)
    {
        _tier = tier;
        Debug.Log(_tier);
    }
    public bool IsManuallyCasting => _isCasting;
    public float FarDistance => _farDistance;
    public MeleeMobTier Tier => _tier;

    public float CurrentRoarSpeedMul => _roarTimeLeft > 0f ? _roarMoveMul : 1f;
    public float CurrentDamageMul => _roarTimeLeft > 0f ? _roarDamageMul : 1f;

    public event Action<string, bool> OnCastEnded; 

    private void Start()
    {
        _move = GetComponent<BaseEnemyMove>();
        TryGetComponent(out _selfAsSource);
    }

    private void Update()
    {
        if (_heavyTimer > 0f) _heavyTimer -= Time.deltaTime;
        if (_roarTimer  > 0f) _roarTimer  -= Time.deltaTime;
        if (_tossTimer  > 0f) _tossTimer  -= Time.deltaTime;
        if (_roarTimeLeft > 0f) _roarTimeLeft -= Time.deltaTime;
    }

    public void BeginManualCast(string skill)
    {
        if (_isCasting) return;
        _isCasting = true;
        _move.SetMoveState(false);
    }

    public void EndManualCast(string skill, bool interrupted)
    {
        if (!_isCasting) return;
        _isCasting = false;
        _move.SetMoveState(true);

        if (!interrupted)
        {
            switch (skill)
            {
                case "Heavy": _heavyTimer = _heavyCd; break;
                case "Roar":  _roarTimer  = _roarCd;  break;
                case "Toss":  _tossTimer  = _tossCd;  break;
            }
        }

        OnCastEnded?.Invoke(skill, interrupted);
    }

    public void InterruptCast()
    {
        if (!_isCasting) return;
        if (_preparedProj) { Destroy(_preparedProj.gameObject); _preparedProj = null; }
        StopAllCoroutines();
        EndManualCast("Any", true);
    }
    
    public void Melee_Hit()
    {
        float dmgBase = (_tier >= MeleeMobTier.Tier4_Red) ? _meleeAverageDamage : _meleeLowDamage;
        float dmg = dmgBase * CurrentDamageMul;

        Vector3 center = transform.position + transform.forward * Mathf.Max(0.6f, _meleeRange * 0.5f);
        var uniques = new HashSet<ActorContext>();
        foreach (var c in Physics.OverlapSphere(center, _meleeRange * 0.6f))
        {
            var ac = c.GetComponentInParent<ActorContext>();
            if (ac != null) uniques.Add(ac);
        }
        foreach (var ac in uniques)
            ac.Hp?.ReceiveDamage(dmg, _selfAsSource);
    }

    public void Melee_End() => EndManualCast("Melee", false);
    public bool Heavy_Available() => _tier >= MeleeMobTier.Tier2_Cyan && _heavyTimer <= 0f;
    public void Heavy_Impact()
    {
        float dmg = _heavyHighDamage * CurrentDamageMul;
        Vector3 pos = _ownerRoot ? _ownerRoot.position : transform.position;

        var uniques = new HashSet<ActorContext>();
        foreach (var c in Physics.OverlapSphere(pos, _heavyRadius))
        {
            var ac = c.GetComponentInParent<ActorContext>();
            if (ac != null) uniques.Add(ac);
        }
        foreach (var ac in uniques)
            ac.Hp?.ReceiveDamage(dmg, _selfAsSource);
    }

    public void Heavy_End() => EndManualCast("Heavy", false);
    public bool Roar_Available() => _tier >= MeleeMobTier.Tier3_Green && _roarTimer <= 0f && _roarTimeLeft <= 0f;
    public void Roar_Apply() => _roarTimeLeft = _roarDuration;
    public void Roar_End() => EndManualCast("Roar", false);
    public bool Toss_Available() => _tier >= MeleeMobTier.Tier4_Red && _tossTimer <= 0f;
    public void Toss_Create()
    {
        if (_boulderPrefab == null || _tossSpawn == null) { EndManualCast("Toss", true); return; }

        _preparedProj = Instantiate(_boulderPrefab, _tossSpawn, false);
        _preparedProj.isKinematic = true;
        _preparedProj.transform.localPosition = Vector3.zero;
        _preparedProj.transform.localRotation = Quaternion.identity;

        _preparedTargetPoint = _target ? _target.position : _tossSpawn.position;
    }

    public void Toss_Throw()
    {
        if (_preparedProj == null) { EndManualCast("Toss", true); return; }

        _preparedProj.transform.SetParent(null, true);
        Vector3 start = _preparedProj.transform.position;
        float dist = Vector3.Distance(start, _preparedTargetPoint);
        float dur = Mathf.Max(0.05f, dist / Mathf.Max(0.01f, _tossSpeed));

        DOTween.Sequence()
            .Append(_preparedProj.transform
                .DOJump(_preparedTargetPoint, _tossArcHeight, 1, dur)
                .SetEase(Ease.Linear).SetSpeedBased(false))
            .OnComplete(() =>
            {
                var uniques = new HashSet<ActorContext>();
                foreach (var c in Physics.OverlapSphere(_preparedProj.position, _tossExplosionRadius))
                {
                    var ac = c.GetComponentInParent<ActorContext>();
                    if (ac != null) uniques.Add(ac);
                }
                float dmg = _tossAverageDamage * CurrentDamageMul;
                foreach (var ac in uniques)
                    ac.Hp?.ReceiveDamage(dmg, _selfAsSource);

                Destroy(_preparedProj.gameObject);
                _preparedProj = null;
            });
    }

    public void Toss_End() => EndManualCast("Toss", false);
    public float GetFarDistance() => _farDistance;
}
