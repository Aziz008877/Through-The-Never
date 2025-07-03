using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FirenadoTornado : MonoBehaviour
{
    [SerializeField] private float _radius = 4f;

    private float _damage;
    private float _pullForce;
    private PlayerContext _ctx;
    private readonly HashSet<IDamageable> _hitAlready = new();

    public void Init(float damage,float pullForce,float lifetime,PlayerContext context)
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

        if (other.TryGetComponent(out IDamageable target) && !_hitAlready.Contains(target))
        {
            float dmg = _damage;
            SkillDamageType type = SkillDamageType.Basic;
            _ctx.ApplyDamageModifiers(ref dmg, ref type);
            target.ReceiveDamage(dmg, type);
            _hitAlready.Add(target);
        }
    }

    private void DestroySelf() => Destroy(gameObject);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}