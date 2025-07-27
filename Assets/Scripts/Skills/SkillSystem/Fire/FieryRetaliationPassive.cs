using UnityEngine;
public class FieryRetaliationPassive : PassiveSkillBehaviour
{
    [SerializeField] private float _radius  = 3f;
    [SerializeField] private float _damage  = 10f;
    [SerializeField] private ParticleSystem _pulseVfx;
    public override void EnablePassive()
    {
        PlayerContext.PlayerHp.OnPlayerReceivedDamage += TriggerPulse;
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerHp.OnPlayerReceivedDamage -= TriggerPulse;
    }

    private void TriggerPulse(float incomingDamage, IDamageable source)
    {
        if (_pulseVfx) _pulseVfx.Play();

        Collider[] hits = Physics.OverlapSphere(PlayerContext.transform.position, _radius);

        foreach (var hit in hits)
        {
            if (hit.transform == PlayerContext.transform) continue;

            if (!hit.TryGetComponent(out IDamageable target)) continue;

            float dmg  = _damage;
            SkillDamageType type = SkillDamageType.Basic;
            PlayerContext.ApplyDamageModifiers(ref dmg, ref type);

            target.ReceiveDamage(dmg, type);
            PlayerContext.FireOnDamageDealt(target, dmg, type);
        }
    }
}