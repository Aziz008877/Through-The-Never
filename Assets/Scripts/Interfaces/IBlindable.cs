public interface IBlindable
{
    void  ApplyBlind(float duration, float missChance, float slowPercent, float dps);
    bool  IsBlinded();
    float CurrentMissChance { get; }
}