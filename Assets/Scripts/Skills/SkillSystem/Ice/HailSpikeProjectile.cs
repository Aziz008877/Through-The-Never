using DG.Tweening;
using UnityEngine;

public class HailSpikeProjectile : MonoBehaviour
{
    private ActorContext _actorContext;
    private Vector3 _targetPos;
    private float _damage;
    private float _impactRadius;
    private DotweenSettings _settings;
    public void Init(Vector3 position, DotweenSettings dotweenSettings, float damage, float radius, ActorContext ctx)
    {
        _actorContext = ctx;
        _targetPos = position;
        _settings = dotweenSettings;
        _damage = damage;
        _impactRadius = radius;
        
        transform.DOMoveY(0, _settings.Duration)
            .SetEase(_settings.AnimationType)
            .OnComplete(Impact);
    }

    private void Impact()
    {
        var cols = Physics.OverlapSphere(transform.position, _impactRadius, ~0, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < cols.Length; i++)
        {
            if (!cols[i].TryGetComponent<IDamageable>(out var d) || !d.CanBeDamaged) continue;

            var ctx = new DamageContext
            {
                Attacker       = _actorContext,
                Target         = d,
                SkillBehaviour = null,                     // не активный скилл
                SkillDef       = null,
                Slot           = SkillSlot.Undefined,
                Type           = SkillDamageType.Basic,    // как у тебя было
                Damage         = _damage,                  // базовый урон ДО модификаторов
                IsCrit         = (_actorContext != null && Random.value < _actorContext.CritChance),
                CritMultiplier = _actorContext != null ? _actorContext.CritMultiplier : 1f,
                HitPoint       = cols[i].transform.position,
                HitNormal      = Vector3.up,
                SourceGO       = gameObject
            };

            if (ctx.IsCrit) ctx.Damage *= ctx.CritMultiplier; // как в BuildDamage
            _actorContext.ApplyDamageContextModifiers(ref ctx);

            d.ReceiveDamage(ctx); // событие «урон нанесён» вызовется внутри цели
        }

        Destroy(gameObject);
    }

}
