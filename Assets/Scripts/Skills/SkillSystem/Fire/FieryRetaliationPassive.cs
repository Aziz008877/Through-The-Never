using UnityEngine;
public class FieryRetaliationPassive : PassiveSkillBehaviour
{
    [SerializeField] private float _radius  = 3f;
    [SerializeField] private float _damage  = 10f;
    [SerializeField] private ParticleSystem _pulseVfx;
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

        Vector3 origin = Context.transform.position;
        var hits = Physics.OverlapSphere(origin, _radius);

        for (int i = 0; i < hits.Length; i++)
        {
            var tr = hits[i].transform;
            if (tr == Context.transform) continue;
            if (!hits[i].TryGetComponent(out IDamageable target)) continue;

            var ctx = new DamageContext
            {
                Attacker       = Context,
                Target         = target,
                SkillBehaviour = null,
                SkillDef       = Definition,
                Slot           = Definition.Slot,
                Type           = SkillDamageType.Basic,
                Damage         = _damage,
                IsCrit         = false,
                CritMultiplier = 1f,
                HitPoint       = origin,
                SourceGO       = gameObject
            };

            Context.ApplyDamageContextModifiers(ref ctx);
            target.ReceiveDamage(ctx);
        }
    }

}