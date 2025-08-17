public readonly struct DamageContext
{
    public readonly float Damage;
    public readonly SkillDamageType DamageType;
    public readonly SkillSlot Slot;
    public readonly ActiveSkillBehaviour Source;

    public DamageContext(float damage, SkillDamageType type, SkillSlot slot = SkillSlot.Basic, ActiveSkillBehaviour source = null)
    {
        Damage = damage;
        DamageType = type;
        Slot = slot;
        Source = source;
    }
}