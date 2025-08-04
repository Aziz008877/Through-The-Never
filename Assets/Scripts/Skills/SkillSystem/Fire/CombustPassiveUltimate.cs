using UnityEngine;

public sealed class CombustPassiveUltimate : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [Header("Combust parameters")]
    [SerializeField] private float      _duration        = 2f;
    [SerializeField] private float      _amplifyPercent  = 0.35f;
    [SerializeField] private float      _explosionDamage = 40f;
    [SerializeField] private float      _explosionRadius = 3.5f;
    [SerializeField] private GameObject _vfxPrefab;

    private bool _addingBonus;             // защита от рекурсии

    public override void EnablePassive()  =>
        Context.RegisterOnDamageDealtModifier(this);

    public override void DisablePassive() =>
        Context.UnregisterOnDamageDealtModifier(this);

    public void OnDamageDealt(IDamageable target, float damage, SkillDamageType type, ActorContext ctx)
    {
        if (_addingBonus) return;                       // избегаем StackOverflow
        if (target is not BaseEnemyHP hp) return;

        /* ── достаём компонент, он уже есть на префабе ── */
        if (!hp.TryGetComponent(out CombustDebuff combust))
            return;                                     // защита на случай пропуска

        /* а) попытка активировать бомбу */
        if (combust.Activate(_duration, _explosionDamage, _explosionRadius, _vfxPrefab, ctx))
        {
            Debug.Log($"<color=orange>[Combust]</color> applied to {hp.name}");
            return;     // только что наложили – базовый урон уже прошёл
        }

        /* б) если бомба активна и в фазе Amplify → бонусный урон */
        if (combust.IsAmplifyPhase)
        {
            float extra = damage * _amplifyPercent;

            _addingBonus = true;
            hp.ReceiveDamage(extra, type);
            _addingBonus = false;

            ctx.FireOnDamageDealt(hp, extra, type);

            Debug.Log($"<color=orange>[Combust]</color> +{_amplifyPercent:P0} " +
                      $"({extra:F1}) bonus to {hp.name}");
        }
    }
}
