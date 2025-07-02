public interface IDamageModifier
{
    void Apply(ref float damage, ref SkillDamageType type);
}