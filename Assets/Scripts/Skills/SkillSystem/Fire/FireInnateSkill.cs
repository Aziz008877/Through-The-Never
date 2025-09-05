using UnityEngine;

public class FireInnateSkill : PassiveSkillBehaviour, IOnDamageDealtContextModifier
{
    [SerializeField] private float _dotPercent = 0.25f;
    [SerializeField] private float _dotDuration = 3f;

    public override void EnablePassive()
    {
        ApplyMeta();
        Context.RegisterOnDamageDealtContextModifier(this);
    }

    public override void DisablePassive()
    {
        Context.UnregisterOnDamageDealtContextModifier(this);
    }

    private void ApplyMeta()
    {
        var meta = MetaProgressionService.Instance;
        if (!meta) return;
        _dotPercent  = meta.PhoenixDotPercent;
        _dotDuration = meta.PhoenixDotDuration;
    }

    public void OnDamageDealt(in DamageContext ctx)
    {
        if (ctx.Type != SkillDamageType.Basic) return;
        if (ctx.Target is IDotReceivable dot)
            dot.ApplyDot(ctx.Damage * _dotPercent, _dotDuration);
    }
}