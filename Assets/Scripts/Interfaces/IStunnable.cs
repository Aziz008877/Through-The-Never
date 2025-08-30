public interface IStunnable
{
    bool IsStunned { get; }
    void ApplyStun(float duration);
}