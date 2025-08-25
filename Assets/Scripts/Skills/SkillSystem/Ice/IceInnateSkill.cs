using UnityEngine;

public class IceInnateSkill : PassiveSkillBehaviour, IOnDamageDealtContextModifier
{
    [SerializeField, Range(0f,1f)] private float _slowPerStack = 0.08f;
    [SerializeField, Range(0f,1f)] private float _dmgRedPerStack = 0.06f;
    [SerializeField] private float _duration = 3f;
    [SerializeField] private int _maxStacks = 5;

    public override void EnablePassive() { Context.RegisterOnDamageDealtContextModifier(this); }
    public override void DisablePassive() { Context.UnregisterOnDamageDealtContextModifier(this); }

    public void OnDamageDealt(in DamageContext ctx)
    {
        if (ctx.Target is not Component co) return;
        if (!co.TryGetComponent<IFrostbiteReceivable>(out var f)) return;

        f.ApplyFrostbite(_slowPerStack, _dmgRedPerStack, _duration, _maxStacks);
    }
}