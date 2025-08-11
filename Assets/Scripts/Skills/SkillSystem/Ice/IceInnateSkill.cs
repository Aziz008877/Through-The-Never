using UnityEngine;

public class IceInnateSkill : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [SerializeField, Range(0f,1f)] private float _slowPerStack = 0.08f;
    [SerializeField, Range(0f,1f)] private float _dmgRedPerStack = 0.06f;
    [SerializeField] private float _duration = 3f;
    [SerializeField] private int _maxStacks = 5;

    public override void EnablePassive() { Context.RegisterOnDamageDealtModifier(this); }
    public override void DisablePassive() { Context.UnregisterOnDamageDealtModifier(this); }

    public void OnDamageDealt(IDamageable target, float damage, SkillDamageType type, ActorContext c)
    {
        if (target is Component co && co.TryGetComponent<IFrostbiteReceivable>(out var f))
            f.ApplyFrostbite(_slowPerStack, _dmgRedPerStack, _duration, _maxStacks);
    }
}