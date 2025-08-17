using System.Collections.Generic;
using UnityEngine;
public class IcyRetaliationSkill : PassiveSkillBehaviour
{
    [SerializeField] private ParticleSystem _pulseVfx;
    [SerializeField] private float _freezeDuration = 2f;
    private readonly HashSet<IDamageable> _seen = new();
    public override void EnablePassive()
    {
        Context.Hp.OnActorReceivedDamage += TriggerPulse;
    }

    public override void DisablePassive()
    {
        Context.Hp.OnActorReceivedDamage -= TriggerPulse;
    }

    private void TriggerPulse(float incomingDamage, IDamageable source)
    {
        if (_pulseVfx) _pulseVfx.Play();

        var hits = Physics.OverlapSphere(Context.transform.position, Definition.Raduis);

        _seen.Clear();
        foreach (var col in hits)
        {
            if (col.TryGetComponent(out IDamageable damageable))
            {
                damageable.ReceiveDamage(Definition.Damage, SkillDamageType.Basic);
                if (!_seen.Add(damageable)) continue;
            }
            
            var stun = col.GetComponent<StunDebuff>();
            if (stun != null) stun.ApplyStun(_freezeDuration);
        }
    }
}