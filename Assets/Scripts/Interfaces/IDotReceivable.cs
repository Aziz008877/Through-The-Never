public interface IDotReceivable
{
    void ApplyDot(float dps, float duration);
    bool IsDotActive { get; set; }
    void RefreshDot(float duration);
}