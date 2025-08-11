public interface IFrostbiteReceivable
{
    void ApplyFrostbite(float slowPerStack, float dmgRedPerStack, float duration, int maxStacks);
    int FrostStacks { get; }
    bool IsFrostActive { get; }
    float MoveSpeedMul { get; }
    float OutgoingDamageMul { get; }
}