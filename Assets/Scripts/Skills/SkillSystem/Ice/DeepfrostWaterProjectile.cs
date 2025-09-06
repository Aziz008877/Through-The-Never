using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepfrostWaterProjectile : MonoBehaviour
{
    [Header("Beam Geometry")]
    [SerializeField] private float _range  = 9f;
    [SerializeField] private float _radius = 0.6f;

    [Header("Ticking")]
    [SerializeField] private float _tickRate = 15f;
    [SerializeField] private bool _stickToOwner = true;

    [Header("Freeze On Continuous Hit")]
    [SerializeField] private float _requiredContinuous = 1.5f;
    [SerializeField] private float _freezeDuration = 2f;

    [Header("VFX (опционально)")]
    [SerializeField] private ParticleSystem _loopVfx;
    
    private float _dps;
    private float _lifeTime;
    private SkillDamageType _type;
    private ActorContext _ownerCtx;
    private Transform _ownerXform;
    
    private readonly Dictionary<Component, float> _exposure = new();
    private readonly HashSet<Component> _hitThisTick = new();
    private readonly List<Component> _tmpList = new();

    private Coroutine _routine;
    
    public void Init(float damageAsDps, float lifeTime, SkillDamageType type, ActorContext owner)
    {
        _dps      = Mathf.Max(0f, damageAsDps);
        _lifeTime = Mathf.Max(0.05f, lifeTime);
        _type     = type;
        _ownerCtx = owner;
        _ownerXform = owner ? owner.transform : null;

        if (_loopVfx)
        {
            _loopVfx.transform.SetPositionAndRotation(transform.position, transform.rotation);
            _loopVfx.Play();
        }

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(BeamRoutine());
    }

    private IEnumerator BeamRoutine()
    {
        float tickDt = 1f / Mathf.Max(1f, _tickRate);
        float t = 0f;

        while (t < _lifeTime)
        {
            if (_stickToOwner && _ownerXform)
            {
                transform.position = _ownerXform.GetComponent<ActorContext>()?.CastPivot
                    ? _ownerCtx.CastPivot.position
                    : _ownerXform.position;
                transform.rotation = _ownerXform.rotation;
            }

            BeamTick(tickDt);
            yield return new WaitForSeconds(tickDt);
            t += tickDt;
        }

        CleanupAndDie();
    }

    private void BeamTick(float dt)
    {
        _hitThisTick.Clear();
        
        Vector3 a = transform.position;
        Vector3 b = a + transform.forward * _range;

        Collider[] hits = Physics.OverlapCapsule(a, b, _radius);
        if (hits != null && hits.Length > 0)
        {
            float dmgPerTick = _dps * dt;

            for (int i = 0; i < hits.Length; i++)
            {
                var col = hits[i];
                if (!col.TryGetComponent(out IDamageable enemy)) continue;
                
                var ctx = new DamageContext
                {
                    Attacker       = _ownerCtx,
                    Target         = enemy,
                    SkillBehaviour = null,
                    SkillDef       = null,
                    Slot           = SkillSlot.Undefined,
                    Type           = _type,
                    Damage         = dmgPerTick,
                    IsCrit         = false,
                    CritMultiplier = 1f,
                    HitPoint       = col.transform.position,
                    SourceGO       = gameObject
                };

                _ownerCtx.ApplyDamageContextModifiers(ref ctx);
                enemy.ReceiveDamage(ctx);
                
                if (col is Component co)
                {
                    _hitThisTick.Add(co);

                    float cur = 0f;
                    _exposure.TryGetValue(co, out cur);
                    cur += dt;
                    _exposure[co] = cur;

                    if (cur >= _requiredContinuous)
                    {
                        if (co.TryGetComponent<IFrostbiteReceivable>(out var frost))
                        {
                            frost.ApplyFrostbite(1f, 0f, _freezeDuration, 1);
                        }

                        _exposure[co] = 0f;
                    }
                }
            }
        }
        
        if (_exposure.Count > 0)
        {
            _tmpList.Clear();
            foreach (var k in _exposure.Keys) _tmpList.Add(k);

            foreach (var c in _tmpList)
            {
                if (_hitThisTick.Contains(c)) continue;
                _exposure.Remove(c);
            }
        }
    }

    private void CleanupAndDie()
    {
        if (_loopVfx) _loopVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (_routine != null) StopCoroutine(_routine);
        _exposure.Clear();
        _hitThisTick.Clear();
        _tmpList.Clear();
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        _exposure.Clear();
        _hitThisTick.Clear();
        _tmpList.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.6f, 0.8f, 1f, 0.35f);
        Vector3 a = transform.position;
        Vector3 b = a + transform.forward * _range;
        Gizmos.DrawWireSphere(a, _radius);
        Gizmos.DrawWireSphere(b, _radius);
    }
#endif
}
