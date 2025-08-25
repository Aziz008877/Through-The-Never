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
                if (_seen.Add(damageable))
                {
                    var ctx = new DamageContext
                    {
                        Attacker       = Context,
                        Target         = damageable,
                        SkillBehaviour = this,
                        SkillDef       = Definition,
                        Slot           = Definition.Slot,
                        Type           = SkillDamageType.Basic,
                        Damage         = Definition.Damage,
                        IsCrit         = UnityEngine.Random.value < Context.CritChance,
                        CritMultiplier = Context.CritMultiplier,
                        HitPoint       = col.transform.position,
                        HitNormal      = Vector3.up,
                        SourceGO       = gameObject
                    };

                    if (ctx.IsCrit) ctx.Damage *= ctx.CritMultiplier;
                    Context.ApplyDamageContextModifiers(ref ctx);

                    damageable.ReceiveDamage(ctx);
                }
            }

            
            var stun = col.GetComponent<StunDebuff>();
            if (stun != null) stun.ApplyStun(_freezeDuration);
        }
    }
}