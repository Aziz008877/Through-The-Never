using UnityEngine;

public sealed class CombustPassiveUltimate : PassiveSkillBehaviour, IOnDamageDealtContextModifier
{
    [Header("Combust parameters")]
    [SerializeField] private float      _duration        = 2f;
    [SerializeField] private float      _amplifyPercent  = 0.35f;
    [SerializeField] private float      _explosionDamage = 40f;
    [SerializeField] private float      _explosionRadius = 3.5f;
    [SerializeField] private GameObject _vfxPrefab;

    private bool _addingBonus;             // защита от рекурсии

    public override void EnablePassive()  =>
        Context.RegisterOnDamageDealtContextModifier(this);

    public override void DisablePassive() =>
        Context.UnregisterOnDamageDealtContextModifier(this);

    public void OnDamageDealt(in DamageContext ctx)
    {
        if (_addingBonus) return;                       
        if (ctx.Target is not BaseEnemyHP hp) return;

        if (!hp.TryGetComponent(out CombustDebuff combust))
            return;

        // а) пробуем активировать бомбу
        if (combust.Activate(_duration, _explosionDamage, _explosionRadius, _vfxPrefab, ctx.Attacker))
        {
            Debug.Log($"<color=orange>[Combust]</color> applied to {hp.name}");
            return;
        }

        // б) если уже в фазе Amplify → накладываем бонусный урон
        if (combust.IsAmplifyPhase)
        {
            float extra = ctx.Damage * _amplifyPercent;

            var bonusCtx = new DamageContext
            {
                Attacker       = ctx.Attacker,
                Target         = hp,
                SkillBehaviour = ctx.SkillBehaviour,
                SkillDef       = ctx.SkillDef,
                Slot           = ctx.Slot,
                Type           = ctx.Type,
                Damage         = extra,
                IsCrit         = false,
                CritMultiplier = 1f,
                SourceGO       = ctx.SourceGO
            };

            _addingBonus = true;
            hp.ReceiveDamage(bonusCtx); // тут и HP спишется, и событие OnDamageDealt сработает
            _addingBonus = false;

            Debug.Log($"<color=orange>[Combust]</color> +{_amplifyPercent:P0} " +
                      $"({extra:F1}) bonus to {hp.name}");
        }
    }

}
