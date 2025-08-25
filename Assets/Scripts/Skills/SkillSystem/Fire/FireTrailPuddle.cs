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
            // собираем контекст тикового урона
            var ctx = new DamageContext
            {
                Attacker       = _ctx,                               // источник ауры/трейла
                Target         = tgt,
                SkillBehaviour = null,                               // не ActiveSkillBehaviour — оставляем null
                SkillDef       = null,
                Slot           = SkillSlot.Dash,
                Type           = SkillDamageType.Basic,              // у тебя было Basic
                Damage         = _tickDmg,                           // урон за тик ДО модификаторов
                IsCrit         = false,
                CritMultiplier = 1f,
                HitPoint       = (tgt as Component)?.transform.position ?? transform.position,
                SourceGO       = gameObject
            };

            // применяем контекстные модификаторы вместо старого ApplyDamageModifiers
            _ctx.ApplyDamageContextModifiers(ref ctx);

            // наносим урон
            tgt.ReceiveDamage(ctx);

            // опционально: продлеваем/накладываем дот (как у тебя было)
            if (tgt is IDotReceivable dot)
                dot.ApplyDot(ctx.Damage, 1f); // используем уже модифицированное значение

            Debug.Log($"<color=orange>[Trailblazer]</color> tick {ctx.Damage:F1} to {tgt}");
        }
    }

}