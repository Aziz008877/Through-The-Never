using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public sealed class ImmolateUltimatePassive : PassiveSkillBehaviour
{
    [Header("Pulse")]
    [SerializeField] private float _radius = 4f;
    [SerializeField] private float _damage = 40f;
    [SerializeField] private float _pushForce = 15f;
    [SerializeField] private ParticleSystem _pulseVfx;

    [Header("Spell-cast")]
    [SerializeField] private int _hitsToCast = 3;
    [SerializeField] private float _castDelay = .2f;
    private int _hitCounter;

    public override void EnablePassive()
    {
        Context.Hp.OnIncomingDamage += OnIncomingDamage;
        _hitCounter = 0;
    }

    public override void DisablePassive()
    {
        Context.Hp.OnIncomingDamage -= OnIncomingDamage;
    }

    private void OnIncomingDamage(ref float dmg, IDamageable source)
    {
        TriggerPulse();
        _hitCounter++;
        
        if (_hitCounter >= _hitsToCast)
        {
            _hitCounter = 0;
            StartCoroutine(CastRandomSpecialDelayed(_castDelay));
        }
    }

    private void TriggerPulse()
    {
        if (_pulseVfx != null)
        {
            _pulseVfx.transform.position = Context.transform.position;
            _pulseVfx.Play();
        }

        Vector3 origin = Context.transform.position;
        var hits = Physics.OverlapSphere(origin, _radius);

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col.transform == Context.transform) continue;
            if (!col.TryGetComponent(out IDamageable enemy)) continue;

            var ctx = new DamageContext
            {
                Attacker       = Context,
                Target         = enemy,
                SkillBehaviour = null,
                SkillDef       = null,
                Slot           = SkillSlot.Passive,
                Type           = SkillDamageType.Basic,
                Damage         = _damage,
                IsCrit         = false,
                CritMultiplier = 1f,
                HitPoint       = col.transform.position,
                HitNormal      = Vector3.up,
                SourceGO       = gameObject
            };

            Context.ApplyDamageContextModifiers(ref ctx);
            enemy.ReceiveDamage(ctx);

            if (col.attachedRigidbody != null)
            {
                Vector3 dir = (col.transform.position - origin).normalized;
                col.attachedRigidbody.AddForce(dir * _pushForce, ForceMode.VelocityChange);
            }
        }
    }


    private IEnumerator CastRandomSpecialDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        List<ActiveSkillBehaviour> readySpecials = new();

        if (Context.SkillManager.GetActive(SkillSlot.Special) is { } sp0 && sp0.IsReady)
            readySpecials.Add(sp0);

        if (readySpecials.Count == 0) yield break;

        int idx = Random.Range(0, readySpecials.Count);
        readySpecials[idx].TryCast();
    }
}
