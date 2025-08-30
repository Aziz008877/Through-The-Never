using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LavaPoolArea : MonoBehaviour
{
    private float _dps;
    private float _lifeTime;
    private ActorContext _ctx;
    private readonly HashSet<IDamageable> _inside = new();
    public void Init(float dps, float radius, float lifeTime, ActorContext ctx)
    {
        _dps = dps;
        _lifeTime = lifeTime;
        _ctx = ctx;

        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = radius;

        Destroy(gameObject, _lifeTime);
    }
    private void OnTriggerEnter(Collider other) => TryAdd(other);
    private void OnTriggerStay (Collider other) => TryAdd(other);
    private void OnTriggerExit (Collider other)
    {
        if (other.TryGetComponent(out IDamageable d)) _inside.Remove(d);
    }

    private void TryAdd(Collider col)
    {
        if (!col.TryGetComponent(out IDamageable d)) return;
        _inside.Add(d);
    }

    private float _tickTimer;
    private void Update()
    {
        _tickTimer += Time.deltaTime;
        if (_tickTimer < 0.5f) return;
        _tickTimer = 0f;

        foreach (var tgt in _inside)
        {
            var ctx = new DamageContext
            {
                Attacker       = _ctx,
                Target         = tgt,
                SkillBehaviour = null,
                SkillDef       = null,
                Slot           = SkillSlot.Undefined,
                Type           = SkillDamageType.Basic,
                Damage         = _dps * 0.5f,
                IsCrit         = false,
                CritMultiplier = 1f,
                HitPoint       = (tgt as Component)?.transform.position ?? transform.position,
                SourceGO       = gameObject
            };

            _ctx.ApplyDamageContextModifiers(ref ctx);
            tgt.ReceiveDamage(ctx);
            
            if (tgt is IDotReceivable dot)
                dot.ApplyDot(_dps, _lifeTime);
        }
    }

}