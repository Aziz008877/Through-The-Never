using UnityEngine;

public sealed class MeltdownPassiveUltimate : PassiveSkillBehaviour, IOnDamageDealtContextModifier
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

    private bool _addingBonus;

    public override void EnablePassive()  => Context.RegisterOnDamageDealtContextModifier(this);

    public override void DisablePassive() => Context.UnregisterOnDamageDealtContextModifier(this);

    public void OnDamageDealt(in DamageContext ctx)
    {
        if (ctx.Attacker != Context) return;
        if (ctx.Type != SkillDamageType.Basic) return;
        if (ctx.SkillDef == null || ctx.SkillDef.School != MagicSchool.Fire) return;
        if (_addingBonus) return;
        if (ctx.Target is not BaseEnemyHP hp) return;
        
        if (!hp.TryGetComponent(out MeltdownDebuff deb))
            deb = hp.gameObject.AddComponent<MeltdownDebuff>();

        deb.Configure(_stackLifeTime, _puddlePrefab, _puddleDps, _puddleRate, _puddleRadius, _puddleLife, Context);

        int prev = deb.StackCount;
        deb.AddStack();
        Debug.Log($"<color=orange>[Meltdown]</color> {hp.name} stacks {prev}→{deb.StackCount}");
        
        float extra = ctx.Damage * deb.StackCount * _ampPerStack;
        if (extra <= 0f) return;

        var bonusCtx = ctx;
        bonusCtx.Damage = extra;
        bonusCtx.Target = hp;
        bonusCtx.IsCrit = false;
        bonusCtx.CritMultiplier = 1f;

        _addingBonus = true;
        hp.ReceiveDamage(bonusCtx);
        _addingBonus = false;

        Debug.Log($"<color=orange>[Meltdown]</color> bonus +{_ampPerStack:P0}×{deb.StackCount} = {extra:F1} dmg");
    }
}
