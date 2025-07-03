using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FirenadoTornado : MonoBehaviour
{
    [SerializeField] private float _radius = 4f;
    [SerializeField] private float _dotPerSecond = 2f;
    [SerializeField] private float _dotDuration = 3f;
    private float _damage;
    private float _pullForce;
    private PlayerContext _ctx;
    private readonly HashSet<IDamageable> _hit = new();
    public void Init(float damage, float pullForce, float lifetime, PlayerContext context)
    {
        _damage = damage;
        _pullForce = pullForce;
        _ctx = context;

        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = _radius;

        Invoke(nameof(DestroySelf), lifetime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rb))
        {
            Vector3 dir = (transform.position - other.transform.position).normalized;
            rb.AddForce(dir * _pullForce, ForceMode.Acceleration);
        }

        if (other.TryGetComponent(out IDamageable target) && !_hit.Contains(target))
        {
            float dmg  = _damage;
            SkillDamageType type = SkillDamageType.Basic;
            _ctx.ApplyDamageModifiers(ref dmg, ref type);
            target.ReceiveDamage(dmg, type);

            if (other.TryGetComponent(out IDotReceivable dotTarget))
                dotTarget.ApplyDot(_dotPerSecond, _dotDuration);

            _hit.Add(target);
        }
    }

    private void DestroySelf() => Destroy(gameObject);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}