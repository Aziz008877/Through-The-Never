public interface IBlindable
{
    void ApplyBlind(float duration, float missChance, float slowPercent, float dps, ActorContext attacker, ActiveSkillBehaviour sourceSkill = null);
    bool IsBlinded();
    float CurrentMissChance { get; }
}