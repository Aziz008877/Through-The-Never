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

        Collider[] hits = Physics.OverlapSphere(Context.transform.position, _radius);

        foreach (var hit in hits)
        {
            if (hit.transform == Context.transform) continue;

            if (!hit.TryGetComponent(out IDamageable target)) continue;

            float dmg  = _damage;
            SkillDamageType type = SkillDamageType.Basic;
            Context.ApplyDamageModifiers(ref dmg, ref type);

            target.ReceiveDamage(dmg, type);
            Context.FireOnDamageDealt(target, dmg, type);
        }
    }
}