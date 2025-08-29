using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
public class Spikes : MonoBehaviour
{
    [SerializeField] private float _damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            var damageContext = new DamageContext
            {
                Attacker       = null,
                Target         = damageable,
                SkillBehaviour = null,
                SkillDef       = null,
                Slot           = SkillSlot.Undefined,
                Type           = SkillDamageType.Basic,
                Damage         = _damage,
                IsCrit         = false,
                CritMultiplier = 1f,
                HitPoint       = (damageable as Component)?.transform.position ?? transform.position,
                SourceGO       = gameObject
            };
            
            damageable.ReceiveDamage(damageContext);
        }

        if (other.TryGetComponent(out ActorContext context))
        {
            context.Hp.ReceiveDamage(_damage, null);
        }
    }
}
