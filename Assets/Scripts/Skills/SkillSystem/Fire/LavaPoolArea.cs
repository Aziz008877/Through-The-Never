using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LavaPoolArea : MonoBehaviour
{
    private float _dps;
    private float _radius;
    private float _lifeTime;
    private PlayerContext _context;
    private readonly HashSet<IDamageable> _affected = new();
    public void Init(float dps, float radius, float lifeTime, PlayerContext ctx)
    {
        _dps = dps;
        _radius = radius;
        _lifeTime = lifeTime;
        _context = ctx;

        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = _radius;
        
        Destroy(gameObject, _lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDamageable target)) return;
        if (_affected.Contains(target)) return;

        float tick = _dps;
        SkillDamageType type = SkillDamageType.Basic;
        _context.ApplyDamageModifiers(ref tick, ref type);
        target.ReceiveDamage(tick, type);
        
        if (other.TryGetComponent(out IDotReceivable dot))
            dot.ApplyDot(_dps, _lifeTime);

        _affected.Add(target);
    }
}