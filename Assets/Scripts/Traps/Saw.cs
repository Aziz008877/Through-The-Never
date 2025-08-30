using System;
using DG.Tweening;
using UnityEngine;
public class Saw : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private DotweenSettings _dotweenSettings;

    private void Start()
    {
        transform
            .DOLocalRotate(new Vector3(359, 0, 0), _dotweenSettings.Duration, RotateMode.LocalAxisAdd)
            .SetEase(_dotweenSettings.AnimationType)
            .SetLoops(-1, LoopType.Incremental);
    }

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
