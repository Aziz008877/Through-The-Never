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

    /* ——— учёт входа/выхода целей ——— */
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable d)) _inside.Add(d);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamageable d)) _inside.Remove(d);
    }

    /* ——— тики урона ——— */
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _tickRate) return;
        _timer = 0f;

        foreach (var tgt in _inside)
        {
            float dmg  = _tickDmg;
            SkillDamageType type = SkillDamageType.Basic;   // у вас Basic/DOT

            _ctx.ApplyDamageModifiers(ref dmg, ref type);
            tgt.ReceiveDamage(dmg, type);

            if (tgt is IDotReceivable dot)                  // Ignite продлевать не обязательно, но можно
                dot.ApplyDot(dmg, 1f);

            Debug.Log(
                $"<color=orange>[Trailblazer]</color> tick {dmg:F1} to {tgt}");
        }
    }
}