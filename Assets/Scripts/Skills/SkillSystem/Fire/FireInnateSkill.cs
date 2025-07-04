using UnityEngine;

public class FireInnateSkill : PassiveSkillBehaviour, IDamageModifier
{
    [SerializeField] private float _dotDuration = 3f;
    [SerializeField] private float _dotPerSecond = 2f;
    public override void Enable()
    {
        PlayerContext.RegisterModifier(this);
    }

    public void Apply(ref float damage, ref SkillDamageType type)
    {
        float dotTotal = _dotPerSecond * _dotDuration;
        damage += dotTotal; 
    }

    public override void Disable()
    {
        PlayerContext.RegisterModifier(this);
    }
}