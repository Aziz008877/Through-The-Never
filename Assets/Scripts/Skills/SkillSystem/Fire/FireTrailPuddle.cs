using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FireTrailPuddle : MonoBehaviour
{
    private float _tickDmg;
    private float _tickRate;
    private ActorContext _ctx;

    private readonly HashSet<IDamageable> _inside = new();
    private float _timer;

    public void Init(float dmgPerTick, float tickRate, float radius, float lifeTime, ActorContext ctx)
    {
        _tickDmg  = dmgPerTick;
        _tickRate = Mathf.Max(0.1f, tickRate);
        _ctx      = ctx;

        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius    = radius;

        Destroy(gameObject, lifeTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable d)) _inside.Add(d);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamageable d)) _inside.Remove(d);
    }
    
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _tickRate) return;
        _timer = 0f;

        foreach (var tgt in _inside)
        {
            var ctx = new DamageContext
            {
                Attacker       = _ctx,
                Target         = tgt,
                SkillBehaviour = null,
                SkillDef       = null,
                Slot           = SkillSlot.Dash,
                Type           = SkillDamageType.Basic,
                Damage         = _tickDmg,
                IsCrit         = false,
                CritMultiplier = 1f,
                HitPoint       = (tgt as Component)?.transform.position ?? transform.position,
                SourceGO       = gameObject
            };
            
            _ctx.ApplyDamageContextModifiers(ref ctx);
            tgt.ReceiveDamage(ctx);
            if (tgt is IDotReceivable dot)
                dot.ApplyDot(ctx.Damage, 1f);

            Debug.Log($"<color=orange>[Trailblazer]</color> tick {ctx.Damage:F1} to {tgt}");
        }
    }

}