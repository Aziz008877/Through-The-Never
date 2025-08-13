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

        SkillDamageType type = SkillDamageType.Basic;
        float baseDamage = _damage;
        
        _actorContext.ApplyDamageModifiers(ref baseDamage, ref type);
        
        float finalDamage = baseDamage;
        if (_actorContext != null && _actorContext.CritChance > 0f && Random.value < _actorContext.CritChance)
            finalDamage *= _actorContext.CritMultiplier;

        for (int i = 0; i < cols.Length; i++)
        {
            if (!cols[i].TryGetComponent<IDamageable>(out var d) || !d.CanBeDamaged) continue;

            d.ReceiveDamage(finalDamage, type);
            _actorContext.FireOnDamageDealt(d, finalDamage, type);
        }

        Destroy(gameObject);
    }
}
