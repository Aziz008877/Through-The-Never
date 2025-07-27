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
        PlayerContext.PlayerHp.OnIncomingDamage += OnIncomingDamage;
        _hitCounter = 0;
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerHp.OnIncomingDamage -= OnIncomingDamage;
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
            _pulseVfx.transform.position = PlayerContext.transform.position;
            _pulseVfx.Play();
        }

        Collider[] hits = Physics.OverlapSphere(PlayerContext.transform.position, _radius);

        foreach (Collider col in hits)
        {
            if (!col.TryGetComponent(out IDamageable enemy)) continue;
            
            float dmg  = _damage;
            SkillDamageType type = SkillDamageType.Basic;
            PlayerContext.ApplyDamageModifiers(ref dmg, ref type);
            enemy.ReceiveDamage(dmg, type);
            PlayerContext.FireOnDamageDealt(enemy, dmg, type);

            if (col.attachedRigidbody != null)
            {
                Vector3 dir = (col.transform.position - PlayerContext.transform.position).normalized;
                col.attachedRigidbody.AddForce(dir * _pushForce, ForceMode.VelocityChange);
            }
        }
    }

    private IEnumerator CastRandomSpecialDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        List<ActiveSkillBehaviour> readySpecials = new();

        if (PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Special) is { } sp0 && sp0.IsReady)
            readySpecials.Add(sp0);

        if (readySpecials.Count == 0) yield break;

        int idx = Random.Range(0, readySpecials.Count);
        readySpecials[idx].TryCast();
    }
}
