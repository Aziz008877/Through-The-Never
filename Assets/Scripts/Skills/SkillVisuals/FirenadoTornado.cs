using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FirenadoTornado : MonoBehaviour
{
    [SerializeField] private float _radius = 4f;
    [SerializeField] private float _dotPerSecond = 2f;
    [SerializeField] private float _dotDuration  = 3f;

    private float _damage;
    private float _pullForce;
    private ActorContext _ctx;
    private readonly HashSet<IDamageable> _alreadyHit = new();

    public void Init(float damage, float pullForce, float lifetime, ActorContext ctx)
    {
        _damage     = damage;
        _pullForce  = pullForce;
        _ctx        = ctx;

        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius    = _radius;

        Invoke(nameof(DestroySelf), lifetime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rb))
        {
            Vector3 dir = (transform.position - other.transform.position).normalized;
            rb.AddForce(dir * _pullForce, ForceMode.Acceleration);
        }
        
        if (!other.TryGetComponent(out IDamageable target) || _alreadyHit.Contains(target))
            return;

        var ctx = new DamageContext
        {
            Attacker       = _ctx,
            Target         = target,
            SkillBehaviour = null,
            SkillDef       = null,
            Slot           = SkillSlot.Undefined,
            Type           = SkillDamageType.Basic,
            Damage         = _damage,
            IsCrit         = false,
            CritMultiplier = 1f,
            HitPoint       = other.transform.position,
            HitNormal      = Vector3.up,
            SourceGO       = gameObject
        };

        _ctx.ApplyDamageContextModifiers(ref ctx);
        target.ReceiveDamage(ctx);
        
        if (other.TryGetComponent(out IDotReceivable dot))
            dot.ApplyDot(_dotPerSecond, _dotDuration);

        _alreadyHit.Add(target);
    }


    private void DestroySelf() => Destroy(gameObject);

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
#endif
}