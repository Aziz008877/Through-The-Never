using UnityEngine;

public sealed class MeltdownPassiveUltimate : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [Header("Stacks / Amplify")]
    [SerializeField] private float _ampPerStack   = 0.08f;   // +8 % за стак
    [SerializeField] private float _stackLifeTime = 4f;      // сек

    [Header("Trail (при 5 стаках)")]
    [SerializeField] private FireTrailPuddle _puddlePrefab;
    [SerializeField] private float _puddleDps    = 6f;
    [SerializeField] private float _puddleRate   = 0.5f;
    [SerializeField] private float _puddleRadius = 2f;
    [SerializeField] private float _puddleLife   = 2f;

    private bool _addingBonus;        // защита от рекурсии

    public override void EnablePassive()  => Context.RegisterOnDamageDealtModifier(this);

    public override void DisablePassive() => Context.UnregisterOnDamageDealtModifier(this);

    public void OnDamageDealt(IDamageable target,
                              float        dmg,
                              SkillDamageType type,
                              ActorContext ctx)
    {
        /* только огненный Basic-урон */
        if (type != SkillDamageType.Basic) return;
        if (_addingBonus) return;                 // внутренний вызов – игнор
        if (target is not BaseEnemyHP hp) return;

        /* компонент стаков */
        if (!hp.TryGetComponent(out MeltdownDebuff deb))
            deb = hp.gameObject.AddComponent<MeltdownDebuff>();

        deb.Configure(_stackLifeTime, _puddlePrefab,
                      _puddleDps, _puddleRate,
                      _puddleRadius, _puddleLife, ctx);

        int prev = deb.StackCount;
        deb.AddStack();
        Debug.Log($"<color=orange>[Meltdown]</color> {hp.name} stacks {prev}→{deb.StackCount}");

        /* бонус-урон */
        float extra = dmg * deb.StackCount * _ampPerStack;
        if (extra <= 0f) return;

        _addingBonus = true;                 // блокируем ре-вход
        hp.ReceiveDamage(extra, type);
        ctx.FireOnDamageDealt(hp, extra, type);
        _addingBonus = false;                // снимаем ТОЛЬКО теперь

        Debug.Log(
            $"<color=orange>[Meltdown]</color> bonus +{_ampPerStack:P0}×{deb.StackCount} = "
            + $"{extra:F1} dmg");
    }
}
