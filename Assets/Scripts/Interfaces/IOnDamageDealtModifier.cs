public interface IOnDamageDealtModifier
{
    void OnDamageDealt(IDamageable target, float damage, SkillDamageType type, ActorContext context);
}