
public interface ISkillModifier
{
    float Evaluate(SkillKey key, float currentValue);
}

public enum SkillStat { Cooldown, Damage, Speed, Radius, Duration, Range, TickRate, PushForce }

public readonly struct SkillKey
{
    public readonly SkillSlot Slot;
    public readonly SkillStat Stat;

    public SkillKey(SkillSlot slot, SkillStat stat)
    { Slot = slot; Stat = stat; }
}