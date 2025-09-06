using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
public enum SandBossSkill { Melee, Dash, Shatter, Toss }

[RequireComponent(typeof(BaseEnemyMove), typeof(NavMeshAgent))]
public class SandBossAttack : BaseEnemyAttack
{
    [Header("Context")]
    [SerializeField] private Transform _ownerRoot;

    [Header("Distances")]
    [SerializeField] private float _farDistance = 7f;

    [Header("Dash Attack")]
    [SerializeField] private float _dashMinRange = 3f;
    [SerializeField] private float _dashMaxRange = 12f;
    [SerializeField] private float _dashHitRadius = 1.8f;
    [SerializeField] private float _dashSpeed = 18f;
    [SerializeField] private float _dashDamage = 20f;

    [Header("Earth Shatter")]
    [SerializeField] private float _shatterRadius = 4f;
    [SerializeField] private float _stunDuration = 2f;
    [SerializeField] private float _shatterDamage = 20f;

    [Header("Boulder Toss")]
    [SerializeField] private Rigidbody _boulderPrefab;
    [SerializeField] private Transform _tossSpawn;
    [SerializeField] private float _boulderArcHeight = 3f;
    [SerializeField] private float _projectileSpeed = 18f;
    [SerializeField] private float _boulderExplosionRadius = 2.2f;
    [SerializeField] private float _boulderDamage = 35f;

    private BaseEnemyMove _move;
    private IDamageable _selfAsSource;
    private bool _isCasting;
    private SandBossSkill _castingSkill;
    private Rigidbody _preparedProj;
    private Vector3 _preparedTargetPoint;
    private Vector3 _dashTargetPoint;
    public bool IsManuallyCasting => _isCasting;
    public float FarDistance => _farDistance;
    public (float min, float max) DashRange => (_dashMinRange, _dashMaxRange);
    public event Action<SandBossSkill, bool> OnCastEnded;
    private void Start()
    {
        _move = GetComponent<BaseEnemyMove>();
        _attackerCtx = GetComponent<ActorContext>() ?? GetComponentInParent<ActorContext>();
        TryGetComponent(out _selfAsSource);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (_preparedProj) { Destroy(_preparedProj.gameObject); _preparedProj = null; }
        _isCasting = false;
    }
    public void BeginManualCast(SandBossSkill skill)
    {
        if (_isCasting) return;
        _isCasting = true;
        _castingSkill = skill;
        _move.SetMoveState(false);
    }

    public void EndManualCast(bool interrupted)
    {
        if (!_isCasting) return;
        _isCasting = false;
        _move.SetMoveState(true);
        OnCastEnded?.Invoke(_castingSkill, interrupted);
    }

    public void InterruptCast()
    {
        if (!_isCasting) return;
        if (_preparedProj) { Destroy(_preparedProj.gameObject); _preparedProj = null; }
        StopAllCoroutines();
        EndManualCast(true);
    }
    
    public void DoMeleeHit()
    {
        ApplyDamageInFront(_meleeDist, _meleeDamage);
    }

    private void ApplyDamageInFront(float range, float damage)
    {
        Vector3 center = transform.position + transform.forward * Mathf.Max(0.6f, range * 0.5f);
        var uniques = new HashSet<ActorContext>();

        foreach (var col in Physics.OverlapSphere(center, range * 0.6f))
        {
            var ac = col.GetComponentInParent<ActorContext>();
            if (ac != null) uniques.Add(ac);
        }

        foreach (var ac in uniques)
        {
            if (ac.Hp != null)
            {
                ac.Hp.ReceiveDamage(damage, _selfAsSource);
            }
        }
    }
    public void DashStart()
    {
        if (_target == null) { EndManualCast(true); return; }
        float d = Vector3.Distance(transform.position, _target.position);
        if (d < _dashMinRange || d > _dashMaxRange) { EndManualCast(true); return; }

        _dashTargetPoint = _target.position;

        Vector3 dir = (_dashTargetPoint - transform.position); dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(dir.normalized);

        StartCoroutine(DashRoutine(_dashTargetPoint));
    }

    private IEnumerator DashRoutine(Vector3 targetPos)
    {
        var agent = GetComponent<NavMeshAgent>();
        if (agent && agent.enabled) agent.enabled = false;

        Transform root = transform;
        Vector3 start = root.position;
        float dist = Vector3.Distance(start, targetPos);
        float dur  = Mathf.Max(0.05f, dist / Mathf.Max(0.01f, _dashSpeed));
        float t = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;
            root.position = Vector3.Lerp(start, targetPos, t / dur);
            yield return null;
        }
        
        var uniques = new HashSet<ActorContext>();
        foreach (var col in Physics.OverlapSphere(root.position + transform.forward * 0.9f, _dashHitRadius))
        {
            var ac = col.GetComponentInParent<ActorContext>();
            if (ac != null) uniques.Add(ac);
        }

        foreach (var ac in uniques)
        {
            if (ac.Hp != null)
            {
                ac.Hp.ReceiveDamage(_dashDamage, _selfAsSource);
            }
        }
        
        if (agent)
        {
            Vector3 navPos = root.position;
            if (NavMesh.SamplePosition(root.position, out var hit, 3f, NavMesh.AllAreas))
                navPos = hit.position;

            agent.enabled = true;
            agent.Warp(navPos);
            agent.isStopped = false;
        }

        EndManualCast(false);
    }
    
    public void EarthShatterImpact()
    {
        Vector3 pos = _ownerRoot ? _ownerRoot.position : transform.position;

        var uniques = new HashSet<ActorContext>();
        foreach (var col in Physics.OverlapSphere(pos, _shatterRadius))
        {
            var ac = col.GetComponentInParent<ActorContext>();
            if (ac != null) uniques.Add(ac);
        }

        int damaged = 0, stunned = 0;
        foreach (var ac in uniques)
        {
            if (ac.Hp != null)
            {
                ac.Hp.ReceiveDamage(_shatterDamage, _selfAsSource);
                damaged++;
            }
            if (ac.TryGetComponent(out IStunnable st))
            {
                st.ApplyStun(_stunDuration);
                stunned++;
            }
        }
    }
    
    public void PrepareBoulder()
    {
        if (_boulderPrefab == null) { EndManualCast(true); return; }
        if (_preparedProj != null) return;

        Vector3 start = _tossSpawn ? _tossSpawn.position
                                   : (_ownerRoot ? _ownerRoot.position : transform.position) + Vector3.up * 1.2f;

        _preparedProj = Instantiate(_boulderPrefab, start, Quaternion.identity);
        _preparedProj.transform.SetParent(_tossSpawn, false); 
        _preparedProj.transform.localPosition = Vector3.zero;
        _preparedProj.transform.localRotation = Quaternion.identity;
        _preparedProj.isKinematic = true;
        _preparedTargetPoint = _target ? _target.position : start;
    }
    public void ThrowPreparedBoulder()
    {
        if (_preparedProj == null) { EndManualCast(true); return; }

        _preparedProj.transform.SetParent(null);
        Vector3 start = _preparedProj.transform.position;
        float dist = Vector3.Distance(start, _preparedTargetPoint);
        float dur  = Mathf.Max(0.05f, dist / Mathf.Max(0.01f, _projectileSpeed));

        DOTween.Sequence()
            .Append(_preparedProj.transform
                .DOJump(_preparedTargetPoint, _boulderArcHeight, 1, dur)
                .SetEase(Ease.Linear)
                .SetSpeedBased(false))
            .OnComplete(() =>
            {
                ExplodeAt(_preparedProj.position); 
                //Destroy(_preparedProj.gameObject);
                _preparedProj = null;
            });
    }

    private void ExplodeAt(Vector3 pos)
    {
        var uniques = new HashSet<ActorContext>();
        foreach (var col in Physics.OverlapSphere(pos, _boulderExplosionRadius))
        {
            var ac = col.GetComponentInParent<ActorContext>();
            if (ac != null) uniques.Add(ac);
        }

        foreach (var ac in uniques)
        {
            if (ac.Hp != null)
            {
                ac.Hp.ReceiveDamage(_boulderDamage, _selfAsSource);
            }
        }
    }
    
    public bool IsFarFromTarget()
    {
        if (_target == null) return false;
        return Vector3.Distance(transform.position, _target.position) > _farDistance;
    }
}
