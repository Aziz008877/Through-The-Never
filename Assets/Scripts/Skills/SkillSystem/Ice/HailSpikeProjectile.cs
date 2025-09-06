using DG.Tweening;
using UnityEngine;

public class HailSpikeProjectile : MonoBehaviour
{
    private ActorContext _actorContext;
    private Vector3 _targetPos;
    private float _damage;
    private float _impactRadius;
    private DotweenSettings _settings;
    public void Init(Vector3 targetPos, DotweenSettings dotweenSettings, float damage, float radius, ActorContext ctx)
    {
        _actorContext  = ctx;
        _targetPos     = targetPos;
        _settings      = dotweenSettings;
        _damage        = damage;
        _impactRadius  = radius;

        transform.DOMove(_targetPos, _settings.Duration)
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
                SkillBehaviour = null,
                SkillDef       = null,
                Slot           = SkillSlot.Undefined,
                Type           = SkillDamageType.Basic,
                Damage         = _damage,
                IsCrit         = (_actorContext && Random.value < _actorContext.CritChance),
                CritMultiplier = _actorContext ? _actorContext.CritMultiplier : 1f,
                HitPoint       = cols[i].transform.position,
                HitNormal      = Vector3.up,
                SourceGO       = gameObject
            };

            if (ctx.IsCrit) ctx.Damage *= ctx.CritMultiplier;
            _actorContext.ApplyDamageContextModifiers(ref ctx);
            d.ReceiveDamage(ctx);
        }

        Destroy(gameObject);
    }
}
