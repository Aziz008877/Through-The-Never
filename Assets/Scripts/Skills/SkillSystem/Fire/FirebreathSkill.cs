using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebreathSkill : ActiveSkillBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem _breathVfx;

    [Header("Shape")]
    [SerializeField] private float _coneAngle = 45f;
    [SerializeField] private float _coneRange = 8f;

    [Header("Timing & DPS")]
    [SerializeField] private float _tickDamage = 2f;
    [SerializeField] private float _tickRate   = 0.25f;

    private readonly HashSet<IDamageable> _hitThisCast = new();

    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();
        StartCoroutine(BreathRoutine());
        StartCooldown();
    }

    private IEnumerator BreathRoutine()
    {
        _breathVfx.Play();
        float elapsed = 0f;
        var wait = new WaitForSeconds(_tickRate);

        while (elapsed < Definition.Duration)
        {
            DealConeDamage();
            elapsed += _tickRate;
            yield return wait;
        }

        _breathVfx.Stop();
        _hitThisCast.Clear();
    }

    private void DealConeDamage()
    {
        var origin  = Context.transform.position;
        var forward = Context.transform.forward;

        var hits = Physics.OverlapSphere(origin, _coneRange);
        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (!col.TryGetComponent(out IDamageable target)) continue;
            if (_hitThisCast != null && _hitThisCast.Contains(target)) continue;

            Vector3 dir = col.transform.position - origin;
            dir.y = 0f;
            if (dir.sqrMagnitude <= 0f) continue;
            if (Vector3.Angle(forward, dir) > _coneAngle) continue;
            float dmgPerTick = _tickDamage * _tickRate;
            var ctx = BuildDamage(dmgPerTick, SkillDamageType.Basic, hitPoint: col.transform.position, hitNormal: Vector3.up, sourceGO: gameObject);
            ctx.Target = target;
            Context.ApplyDamageContextModifiers(ref ctx);

            target.ReceiveDamage(ctx);
            _hitThisCast?.Add(target);
        }
    }

}
